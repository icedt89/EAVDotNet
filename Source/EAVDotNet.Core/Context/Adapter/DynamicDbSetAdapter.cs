namespace JanHafner.EAVDotNet.Context.Adapter
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using JanHafner.EAVDotNet.Context.InstanceRelation;
    using JanHafner.EAVDotNet.Instanciation;
    using JanHafner.EAVDotNet.Instrumentation.InstanceSystem;
    using JanHafner.EAVDotNet.Model;
    using JanHafner.EAVDotNet.Model.Identity;
    using JanHafner.EAVDotNet.Query;
    using JanHafner.EAVDotNet.Query.Expression.Inlining;
    using JanHafner.EAVDotNet.Query.Expression.Translation;
    using JanHafner.EAVDotNet.Query.ResultHandling;
    using JetBrains.Annotations;

    /// <summary>
    /// Provides an abstraction between populated entity instances and instance descriptors.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    internal sealed class DynamicDbSetAdapter<T> : IDynamicDbSetAdapter<T>
        where T : class
    {
        [NotNull]
        private readonly DbSet<InstanceDescriptor> instances;

        [NotNull]
        private readonly IInstanceDescriptorFactory instanceDescriptorFactory;

        [NotNull]
        private readonly ExportFactory<IInstanceResolutionWalker> instanceResolutionWalkerFactory;

        [NotNull]
        private readonly IQueryResultHandlerProvider queryResultHandlerProvider;

        [NotNull]
        private readonly IInstanceRelationStore instanceRelationStore;

        [NotNull]
        private readonly IQueryableCallTreeTranslator queryableCallTreeTranslator;

        [NotNull]
        private readonly ObservableCollection<T> local;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDbSetAdapter{T}"/> class.
        /// </summary>
        /// <param name="instances">The underlying <see cref="DbSet{InstanceDescriptor}"/>.</param>
        /// <param name="instanceDescriptorFactory">The <see cref="IInstanceDescriptorFactory"/>.</param>
        /// <param name="instanceResolutionWalkerFactory">The <see cref="ExportFactory{IInstanceResolutionWalker}"/> which create new instances of <see cref="IInstanceResolutionWalker"/>.</param>
        /// <param name="queryResultHandlerProvider">The <see cref="IQueryResultHandlerProvider"/>.</param>
        /// <param name="instanceRelationStore">The <see cref="IInstanceRelationStore"/>.</param>
        /// <param name="queryableCallTreeTranslator">The <see cref="IQueryableCallTreeTranslator"/>.</param>
        public DynamicDbSetAdapter([NotNull] DbSet<InstanceDescriptor> instances, [NotNull] IInstanceDescriptorFactory instanceDescriptorFactory, [NotNull] ExportFactory<IInstanceResolutionWalker> instanceResolutionWalkerFactory, [NotNull] IQueryResultHandlerProvider queryResultHandlerProvider, [NotNull] IInstanceRelationStore instanceRelationStore, [NotNull] IQueryableCallTreeTranslator queryableCallTreeTranslator)
        {
            if (instances == null)
            {
                throw new ArgumentNullException(nameof(instances));
            }

            if (instanceDescriptorFactory == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptorFactory));
            }

            if (instanceResolutionWalkerFactory == null)
            {
                throw new ArgumentNullException(nameof(instanceResolutionWalkerFactory));
            }

            if (instanceRelationStore == null)
            {
                throw new ArgumentNullException(nameof(instanceRelationStore));
            }

            if (queryResultHandlerProvider == null)
            {
                throw new ArgumentNullException(nameof(queryResultHandlerProvider));
            }

            if (queryableCallTreeTranslator == null)
            {
                throw new ArgumentNullException(nameof(queryableCallTreeTranslator));
            }

            this.instances = instances;
            this.instanceDescriptorFactory = instanceDescriptorFactory;
            this.instanceResolutionWalkerFactory = instanceResolutionWalkerFactory;
            this.queryResultHandlerProvider = queryResultHandlerProvider;
            this.queryableCallTreeTranslator = queryableCallTreeTranslator;
            this.instanceRelationStore = instanceRelationStore;

            this.instanceRelationStore.InstanceRelationChanged += this.InstanceRelationStoreOnInstanceRelationChanged;

            this.local = new ObservableCollection<T>();
        }

        /// <summary>
        /// Synchronizes the <see cref="IInstanceRelationStore"/> which holds all populated instances and related <see cref="InstanceDescriptors"/> with the local <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <param name="sender">The sender <see cref="IInstanceRelationStore"/>.</param>
        /// <param name="instanceRelationChangedEventArgs">Event arguments which encapsulates information about the type of change.</param>
        private void InstanceRelationStoreOnInstanceRelationChanged(Object sender, InstanceRelationChangedEventArgs instanceRelationChangedEventArgs)
        {
            if (sender != this.instanceRelationStore || !(instanceRelationChangedEventArgs.Instance is T))
            {
                return;
            }

            switch (instanceRelationChangedEventArgs.ChangeType)
            {
                case InstanceRelationChangeType.Created:
                    this.local.Add((T) instanceRelationChangedEventArgs.Instance);
                    break;
                case InstanceRelationChangeType.Removed:
                    this.local.Remove((T)instanceRelationChangedEventArgs.Instance);
                    break;
            }
        }

        /// <summary>
        /// Unlike the implementation of the <see cref="DbContext"/>, this implementation searches for an <see cref="InstanceDescriptor"/> where the PrimaryKey (Id) IS ONE OF the supplied values!
        /// The first match is returned.
        /// Finds an entity with the given primary key values.
        ///             If an entity with the given primary key values exists in the context, then it is
        ///             returned immediately without making a request to the store.  Otherwise, a request
        ///             is made to the store for an entity with the given primary key values and this entity,
        ///             if found, is attached to the context and returned.  If no entity is found in the
        ///             context or the store, then null is returned.
        /// </summary>
        /// <remarks>
        /// The ordering of composite key values is as defined in the EDM, which is in turn as defined in
        ///             the designer, by the Code First fluent API, or by the DataMember attribute.
        /// </remarks>
        /// <param name="keyValues">The values of the primary key for the entity to be found. </param>
        /// <returns>
        /// The entity found, or null. 
        /// </returns>
        public T Find(params Object[] keyValues)
        {
            var instanceDescriptor = this.instances.Find(keyValues);
            if (instanceDescriptor == null)
            {
                return default(T);
            }

            using (var instanceResolutionWalker = this.instanceResolutionWalkerFactory.CreateExport())
            {
                return (T)instanceResolutionWalker.Value.ResolveInstance(new InstanceResolutionContext(instanceDescriptor, this.instanceRelationStore));
            }
        }

        /// <summary>
        /// Adds the given entity to the context underlying the set in the Added state such that it will
        ///             be inserted into the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to add. </param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="entity"/>' cannot be null.</exception>
        /// <returns>
        /// The entity. 
        /// </returns>
        /// <remarks>
        /// Note that entities that are already in the context in some other state will have their state set
        ///             to Added.  Add is a no-op if the entity is already in the context in the Added state.
        /// </remarks>
        public T Add([NotNull] T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            InstanceDescriptor instanceDescriptor;
            if (!this.instanceRelationStore.TryGetRelated(entity, out instanceDescriptor))
            {
                instanceDescriptor = this.instanceDescriptorFactory.CreateInstanceDescriptor(new InstanceDescriptorCreationContext(
                    entity, this.instanceRelationStore));

                this.instances.Add(instanceDescriptor);
            }
            
            return entity;
        }

        /// <summary>
        /// Defines a method which is capable of adding the supplied entities in bulk.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="entities"/>' cannot be null.</exception>
        /// <returns>The entities which are added.</returns>
        public IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var instanceDescriptorsToAdd = new Dictionary<InstanceDescriptor, T>();
            foreach (var entity in entities)
            {
                InstanceDescriptor instanceDescriptor;
                if (!this.instanceRelationStore.TryGetRelated(entity, out instanceDescriptor))
                {
                    instanceDescriptor = this.instanceDescriptorFactory.CreateInstanceDescriptor(new InstanceDescriptorCreationContext(entity, this.instanceRelationStore));
                }

                instanceDescriptorsToAdd.Add(instanceDescriptor, entity);
            }

            this.instances.AddRange(instanceDescriptorsToAdd.Keys);

            return instanceDescriptorsToAdd.Values;
        }

        /// <summary>
        /// Marks the given entity as Deleted such that it will be deleted from the database when SaveChanges
        ///             is called.  Note that the entity must exist in the context in some other state before this method
        ///             is called.
        /// </summary>
        /// <param name="entity">The entity to remove. </param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="entity"/>' cannot be null.</exception>
        /// <returns>
        /// The entity. 
        /// </returns>
        /// <remarks>
        /// Note that if the entity exists in the context in the Added state, then this method
        ///             will cause it to be detached from the context.  This is because an Added entity is assumed not to
        ///             exist in the database such that trying to delete it does not make sense.
        /// </remarks>
        public T Remove([NotNull] T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            InstanceDescriptor instanceDescriptor;
            if (!this.instanceRelationStore.TryGetRelated(entity, out instanceDescriptor))
            {
                var keyValue = entity.GetIdentityValue();

                instanceDescriptor = this.instances.First(id => id.Id.Equals((Guid) keyValue));
            }

            this.instances.Remove(instanceDescriptor);
            
            return entity;
        }

        /// <summary>
        /// Defines a method which is capable of removing the supplied entities in bulk.
        /// </summary>
        /// <param name="entities">The entities to remove.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="entities"/>' cannot be null.</exception>
        /// <returns>The entities which are removed.</returns>
        public IEnumerable<T> RemoveRange(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var instanceDescriptorsToRemove = new Dictionary<InstanceDescriptor, T>();
            foreach (var entity in entities)
            {
                InstanceDescriptor instanceDescriptor;
                if (!this.instanceRelationStore.TryGetRelated(entity, out instanceDescriptor))
                {
                    var keyValue = entity.GetIdentityValue();

                    instanceDescriptor = this.instances.First(id => id.Id.Equals((Guid)keyValue));
                }

                instanceDescriptorsToRemove.Add(instanceDescriptor, entity);
            }

            this.instances.RemoveRange(instanceDescriptorsToRemove.Keys);

            return instanceDescriptorsToRemove.Values;
        }

        /// <summary>
        /// Attaches the given entity to the context underlying the set.  That is, the entity is placed
        ///             into the context in the Unchanged state, just as if it had been read from the database.
        /// </summary>
        /// <param name="entity">The entity to attach. </param>
        /// <returns>
        /// The entity. 
        /// </returns>
        /// <remarks>
        /// Attach is used to repopulate a context with an entity that is known to already exist in the database.
        ///             SaveChanges will therefore not attempt to insert an attached entity into the database because
        ///             it is assumed to already be there.
        ///             Note that entities that are already in the context in some other state will have their state set
        ///             to Unchanged.  Attach is a no-op if the entity is already in the context in the Unchanged state.
        /// </remarks>
        public T Attach(T entity)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates a new instance of an entity for the type of this set.
        ///             Note that this instance is NOT added or attached to the set.
        ///             The instance returned will be a proxy if the underlying context is configured to create
        ///             proxies and the entity type meets the requirements for creating a proxy.
        /// </summary>
        /// <returns>
        /// The entity instance, which may be a proxy. 
        /// </returns>
        public T Create()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new instance of an entity for the type of this set or for a type derived
        ///             from the type of this set.
        ///             Note that this instance is NOT added or attached to the set.
        ///             The instance returned will be a proxy if the underlying context is configured to create
        ///             proxies and the entity type meets the requirements for creating a proxy.
        /// 
        /// </summary>
        /// <typeparam name="TDerivedEntity">The type of entity to create. </typeparam>
        /// <returns>
        /// The entity instance, which may be a proxy.
        /// </returns>
        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Disables change tracking.
        /// </summary>
        /// <returns>A new query.</returns>
        public DynamicQueryable<T> AsNoTracking()
        {
            var provider = new DynamicQueryProvider<T>(this.instances.AsNoTracking(), this.instanceRelationStore, this.queryResultHandlerProvider, this.queryableCallTreeTranslator);

            return new DynamicQueryable<T>(provider.NoChangeTracking(), this.Expression);
        }

        /// <summary>
        /// Disables lazy loading.
        /// </summary>
        /// <returns>A new query.</returns>
        public DynamicQueryable<T> WithoutLazyLoading()
        {
            var provider = new DynamicQueryProvider<T>(this.instances, this.instanceRelationStore, this.queryResultHandlerProvider, this.queryableCallTreeTranslator);

            return new DynamicQueryable<T>(provider.NoLazyLoading(), this.Expression);
        }

        /// <summary>
        /// Creates an <see cref="IQueryable{InstanceDescriptor}"/> based on the underlying <see cref="IDbSet{InstanceDescriptor}"/>.
        /// </summary>
        /// <returns>An <see cref="IQueryable{InstanceDescriptor}"/>.</returns>
        public IQueryable<InstanceDescriptor> CreateDataSourceQuery()
        {
            var expression = Expression.Constant(this.instances.OfType(typeof(T)));
            var provider =  new DynamicQueryProvider<InstanceDescriptor>(this.instances, this.instanceRelationStore, this.queryResultHandlerProvider, this.queryableCallTreeTranslator);

            return new DynamicQueryable<InstanceDescriptor>(provider, expression);
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.ObjectModel.ObservableCollection`1"/> that represents a local view of all Added, Unchanged,
        ///             and Modified entities in this set.  This local view will stay in sync as entities are added or
        ///             removed from the context.  Likewise, entities added to or removed from the local view will automatically
        ///             be added to or removed from the context.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// This property can be used for data binding by populating the set with data, for example by using the Load
        ///             extension method, and then binding to the local data through this property.  For WPF bind to this property
        ///             directly.  For Windows Forms bind to the result of calling ToBindingList on this property
        /// 
        /// </remarks>
        /// 
        /// <value>
        /// The local view.
        /// </value>
        public ObservableCollection<T> Local
        {
            get { return this.local; }
        }

        /// <summary>
        /// Gibt einen Enumerator zurück, der die Auflistung durchläuft.
        /// </summary>
        /// <returns>
        /// Ein <see cref="T:System.Collections.Generic.IEnumerator`1"/>, der zum Durchlaufen der Auflistung verwendet werden kann.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            var result = (IEnumerator)this.Provider.Execute(this.Expression);

            while (result.MoveNext())
            {
                yield return (T)result.Current;
            }
        }

        /// <summary>
        /// Gibt einen Enumerator zurück, der eine Auflistung durchläuft.
        /// </summary>
        /// <returns>
        /// Ein <see cref="T:System.Collections.IEnumerator"/>-Objekt, das zum Durchlaufen der Auflistung verwendet werden kann.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Ruft die Ausdrucksbaumstruktur ab, die mit der Instanz von <see cref="T:System.Linq.IQueryable"/> verknüpft ist.
        /// </summary>
        /// <returns>
        /// Der <see cref="T:System.Linq.Expressions.Expression"/>, der mit dieser Instanz von <see cref="T:System.Linq.IQueryable"/> verknüpft ist.
        /// </returns>
        public Expression Expression
        {
            get { return Expression.Constant(this, typeof(IQueryable<T>)); }
        }

        /// <summary>
        /// Ruft den Typ der Elemente ab, die zurückgegeben werden, wenn die Ausdrucksbaumstruktur ausgeführt wird, die mit dieser Instanz von <see cref="T:System.Linq.IQueryable"/> verknüpft ist.
        /// </summary>
        /// <returns>
        /// Ein <see cref="T:System.Type"/>, der den Typ der Elemente darstellt, die zurückgegeben werden, wenn die Ausdrucksbaumstruktur ausgeführt wird, die mit diesem Objekt verknüpft ist.
        /// </returns>
        public Type ElementType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Ruft den Abfrageanbieter ab, der dieser Datenquelle zugeordnet ist.
        /// </summary>
        /// <returns>
        /// Der <see cref="T:System.Linq.IQueryProvider"/>, der dieser Datenquelle zugeordnet ist.
        /// </returns>
        public IQueryProvider Provider
        {
            get
            {
                return new DynamicQueryProvider<T>(this.instances, this.instanceRelationStore, this.queryResultHandlerProvider, this.queryableCallTreeTranslator);
            }
        }
    }
}