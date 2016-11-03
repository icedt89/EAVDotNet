namespace JanHafner.EAVDotNet.Context.Adapter
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Linq;
    using JanHafner.EAVDotNet.Model;
    using JetBrains.Annotations;

    /// <summary>
    /// Properties with tihs property type are used by the <see cref="DynamicDbContextAdapter"/>.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public interface IDynamicDbSetAdapter<T> : IDbSet<T> where T : class
    {
        /// <summary>
        /// Defines a method which is capable of adding the supplied entities in bulk.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <returns>The entities which are added.</returns>
        [NotNull]
        IEnumerable<T> AddRange([NotNull] IEnumerable<T> entities);

        /// <summary>
        /// Defines a method which is capable of removing the supplied entities in bulk.
        /// </summary>
        /// <param name="entities">The entities to remove.</param>
        /// <returns>The entities which are removed.</returns>
        [NotNull]
        IEnumerable<T> RemoveRange([NotNull]IEnumerable<T> entities);

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
        ObservableCollection<T> Local { get; }

        /// <summary>
        /// Finds an entity with the given primary key values.
        ///             If an entity with the given primary key values exists in the context, then it is
        ///             returned immediately without making a request to the store.  Otherwise, a request
        ///             is made to the store for an entity with the given primary key values and this entity,
        ///             if found, is attached to the context and returned.  If no entity is found in the
        ///             context or the store, then null is returned.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// The ordering of composite key values is as defined in the EDM, which is in turn as defined in
        ///             the designer, by the Code First fluent API, or by the DataMember attribute.
        /// 
        /// </remarks>
        /// <param name="keyValues">The values of the primary key for the entity to be found. </param>
        /// <returns>
        /// The entity found, or null.
        /// </returns>
        T Find(params object[] keyValues);

        /// <summary>
        /// Adds the given entity to the context underlying the set in the Added state such that it will
        ///             be inserted into the database when SaveChanges is called.
        /// 
        /// </summary>
        /// <param name="entity">The entity to add. </param>
        /// <returns>
        /// The entity.
        /// </returns>
        /// 
        /// <remarks>
        /// Note that entities that are already in the context in some other state will have their state set
        ///             to Added.  Add is a no-op if the entity is already in the context in the Added state.
        /// 
        /// </remarks>
        T Add(T entity);

        /// <summary>
        /// Marks the given entity as Deleted such that it will be deleted from the database when SaveChanges
        ///             is called.  Note that the entity must exist in the context in some other state before this method
        ///             is called.
        /// 
        /// </summary>
        /// <param name="entity">The entity to remove. </param>
        /// <returns>
        /// The entity.
        /// </returns>
        /// 
        /// <remarks>
        /// Note that if the entity exists in the context in the Added state, then this method
        ///             will cause it to be detached from the context.  This is because an Added entity is assumed not to
        ///             exist in the database such that trying to delete it does not make sense.
        /// 
        /// </remarks>
        T Remove(T entity);

        /// <summary>
        /// Attaches the given entity to the context underlying the set.  That is, the entity is placed
        ///             into the context in the Unchanged state, just as if it had been read from the database.
        /// 
        /// </summary>
        /// <param name="entity">The entity to attach. </param>
        /// <returns>
        /// The entity.
        /// </returns>
        /// 
        /// <remarks>
        /// Attach is used to repopulate a context with an entity that is known to already exist in the database.
        ///             SaveChanges will therefore not attempt to insert an attached entity into the database because
        ///             it is assumed to already be there.
        ///             Note that entities that are already in the context in some other state will have their state set
        ///             to Unchanged.  Attach is a no-op if the entity is already in the context in the Unchanged state.
        /// 
        /// </remarks>
        T Attach(T entity);

        /// <summary>
        /// Creates a new instance of an entity for the type of this set.
        ///             Note that this instance is NOT added or attached to the set.
        ///             The instance returned will be a proxy if the underlying context is configured to create
        ///             proxies and the entity type meets the requirements for creating a proxy.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// The entity instance, which may be a proxy.
        /// </returns>
        T Create();

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
        TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T;

        /// <summary>
        /// Creates an <see cref="IQueryable{InstanceDescriptor}"/> based on the underlying <see cref="IDbSet{InstanceDescriptor}"/>.
        /// </summary>
        /// <returns>An <see cref="IQueryable{InstanceDescriptor}"/>.</returns>
        IQueryable<InstanceDescriptor> CreateDataSourceQuery();
    }
}