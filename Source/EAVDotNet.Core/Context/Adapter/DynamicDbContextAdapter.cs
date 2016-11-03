namespace JanHafner.EAVDotNet.Context.Adapter
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Data.Entity;
    using System.Reflection;
    using System.Threading.Tasks;
    using JanHafner.EAVDotNet.Context;
    using JanHafner.EAVDotNet.Context.InstanceRelation;
    using JanHafner.EAVDotNet.Instanciation;
    using JanHafner.EAVDotNet.Instrumentation.InstanceSystem;
    using JanHafner.EAVDotNet.Properties;
    using JanHafner.EAVDotNet.Query.Expression.Translation;
    using JanHafner.EAVDotNet.Query.ResultHandling;
    using JetBrains.Annotations;

    public abstract class DynamicDbContextAdapter : IDisposable
    {
        [NotNull]
        private readonly IDynamicDbContext dynamicDbContext;

        [NotNull]
        private readonly IInstanceDescriptorFactory instanceDescriptorFactory;

        [NotNull]
        private readonly ExportFactory<IInstanceResolutionWalker> instanceResolutionWalkerFactory;

        [NotNull]
        private readonly IInstanceRelationStore instanceRelationStore;

        [NotNull]
        private readonly IQueryResultHandlerProvider queryResultHandlerProvider;

        [NotNull]
        private readonly IQueryableCallTreeTranslator queryableCallTreeTranslator;

        [NotNull]
        private readonly ConcurrentDictionary<Type, Object> createdDynamicDbSetAdapters;

        [NotNull]
        private static readonly ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> observedDynamicDbSetProperties;

        static DynamicDbContextAdapter()
        {
            DynamicDbContextAdapter.observedDynamicDbSetProperties = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDbContextAdapter"/> class.
        /// </summary>
        /// <param name="dynamicDbContextFactory">The <see cref="ExportFactory{IDynamicDbContext}"/>.</param>
        /// <param name="instanceDescriptorFactory">The <see cref="IInstanceDescriptorFactory"/>.</param>
        /// <param name="instanceResolutionWalkerFactory">The <see cref="IInstanceResolutionWalker"/> factory.</param>
        /// <param name="queryResultHandlerProvider">The <see cref="IQueryResultHandlerProvider"/></param>
        /// <param name="queryableCallTreeTranslator">The <see cref="QueryableCallTreeTranslator"/>.</param>
        /// <param name="instanceRelationStore">The <see cref="IInstanceRelationStore"/>.</param>
        /// <param name="dynamicDbContextConfiguration">The configuration for the <see cref="IDynamicDbContext"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="queryableCallTreeTranslator"/>', '<paramref name="instanceRelationStore"/>', '<paramref name="queryResultHandlerProvider"/>', '<paramref name="instanceDescriptorFactory"/>', '<paramref name="queryableCallTreeTranslator"/>' ,'<paramref name="dynamicDbContextFactory"/>' and '<paramref name="instanceResolutionWalkerFactory"/>' cannot be null.</exception>
        protected DynamicDbContextAdapter([NotNull] IDynamicDbContextFactory dynamicDbContextFactory,
            [NotNull] IInstanceDescriptorFactory instanceDescriptorFactory,
            [NotNull] ExportFactory<IInstanceResolutionWalker> instanceResolutionWalkerFactory,
            [NotNull] IQueryResultHandlerProvider queryResultHandlerProvider,
            [NotNull] IQueryableCallTreeTranslator queryableCallTreeTranslator,
            [NotNull] IInstanceRelationStore instanceRelationStore,
            [NotNull] DynamicDbContextConfiguration dynamicDbContextConfiguration)
        {
            if (dynamicDbContextFactory == null)
            {
                throw new ArgumentNullException(nameof(dynamicDbContextFactory));
            }

            if (instanceDescriptorFactory == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptorFactory));
            }

            if (instanceResolutionWalkerFactory == null)
            {
                throw new ArgumentNullException(nameof(instanceResolutionWalkerFactory));
            }

            if (queryResultHandlerProvider == null)
            {
                throw new ArgumentNullException(nameof(queryResultHandlerProvider));
            }

            if (queryableCallTreeTranslator == null)
            {
                throw new ArgumentNullException(nameof(queryableCallTreeTranslator));
            }

            if (instanceRelationStore == null)
            {
                throw new ArgumentNullException(nameof(instanceRelationStore));
            }

            if (dynamicDbContextConfiguration == null)
            {
                throw new ArgumentNullException(nameof(dynamicDbContextConfiguration));
            }

            this.instanceDescriptorFactory = instanceDescriptorFactory;
            this.instanceResolutionWalkerFactory = instanceResolutionWalkerFactory;
            this.queryResultHandlerProvider = queryResultHandlerProvider;
            this.queryableCallTreeTranslator = queryableCallTreeTranslator;
            this.instanceRelationStore = instanceRelationStore;
            this.dynamicDbContext = dynamicDbContextFactory.CreateDynamicDbContext(dynamicDbContextConfiguration);
            this.createdDynamicDbSetAdapters = new ConcurrentDictionary<Type, Object>();

            this.CreateDynamidDbSetAdapters();
        }

        /// <summary>
        /// Creates an <see cref="IDynamicDbSetAdapter{T}"/> for an entity type which is not present as instance property of the <see cref="DynamicDbContextAdapter"/>.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>The <see cref="IDynamicDbSetAdapter{T}"/>.</returns>
        protected IDynamicDbSetAdapter<T> CreateDynamicSetAccessor<T>() where T : class
        {
            return (IDynamicDbSetAdapter<T>)this.CreateDynamidDbSetAdapter(typeof (T));
        }

        /// <summary>
        /// Führt anwendungsspezifische Aufgaben aus, die mit dem Freigeben, Zurückgeben oder Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.dynamicDbContext.Dispose();
        }

        /// <summary>
        /// Saves the changes and returns the count of affected rows, asynchronously.
        /// </summary>
        /// <returns>The count of affected rows.</returns>
        public async Task<Int32> SaveChangesAsync()
        {
            return await this.dynamicDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Creates <see cref="DynamicDbSetAdapter{T}"/> instances for the property if this context.
        /// </summary>
        private void CreateDynamidDbSetAdapters()
        {
            var dbSetProperties = observedDynamicDbSetProperties.GetOrAdd(this.GetType(), _ =>
            {
                var result = new List<PropertyInfo>();

                Parallel.ForEach(_.GetProperties(BindingFlags.Instance | BindingFlags.Public), property =>
                {
                    if (!property.PropertyType.IsConstructedGenericType)
                    {
                        return;
                    }

                    if (typeof(IDbSet<>).IsAssignableFrom(property.PropertyType.GetGenericTypeDefinition())
                        || typeof(DbSet<>).IsAssignableFrom(property.PropertyType.GetGenericTypeDefinition()))
                    {
                        throw new NotSupportedException(ExceptionMessages.OnlyDynamicDbSetIsSupportedExceptionMessage);
                    }

                    if (!property.PropertyType.IsDynamicDbSet())
                    {
                        return;
                    }

                    result.Add(property);

                    var dbSetInstance = this.CreateDynamidDbSetAdapter(property.PropertyType.GetGenericArguments()[0]);

                    property.SetValue(this, dbSetInstance);
                });

                return result;
            });

            Parallel.ForEach(dbSetProperties, property =>
            {
                var dbSetInstance = this.CreateDynamidDbSetAdapter(property.PropertyType.GetGenericArguments()[0]);

                property.SetValue(this, dbSetInstance);
            });
        }

        /// <summary>
        /// Creates an instance of <see cref="DynamicDbSetAdapter{T}"/> for the supplied entity type.
        /// </summary>
        /// <param name="entityType">The <see cref="Type"/> of the entity.</param>
        /// <returns>An instance of the <see cref="DynamicDbSetAdapter{T}"/>.</returns>
        private Object CreateDynamidDbSetAdapter(Type entityType)
        {
            return this.createdDynamicDbSetAdapters.GetOrAdd(entityType, type =>
            {
                var realDbSetType = typeof(DynamicDbSetAdapter<>).MakeGenericType(type);

                return Activator.CreateInstance(realDbSetType,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Object[]
                    {
                    this.dynamicDbContext.Instances,
                    this.instanceDescriptorFactory,
                    this.instanceResolutionWalkerFactory,
                    this.queryResultHandlerProvider,
                    this.instanceRelationStore,
                    this.queryableCallTreeTranslator
                    }, null);
            });
        }
    }
}