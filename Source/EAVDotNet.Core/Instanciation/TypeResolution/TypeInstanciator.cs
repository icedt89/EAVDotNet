namespace JanHafner.EAVDotNet.Instanciation.TypeResolution
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;
    using Castle.DynamicProxy;
    using Instrumentation.InstanceSystem.PropertyValueFactories;
    using Instrumentation.InstanceSystem.PropertyValueFactories.Collection;
    using JanHafner.EAVDotNet.Instanciation.ChangeTracking;
    using JetBrains.Annotations;

    /// <summary>
    /// Instanciates a supplied <see cref="Type"/>.
    /// </summary>
    [Export(typeof (ITypeInstanciator))]
    internal sealed class TypeInstanciator : ITypeInstanciator
    {
        [NotNull]
        private readonly IPropertyValueFactoryProvider propertyValueFactoryProvider;

        [NotNull]
        private readonly ICollectionItemFactoryProvider collectionItemFactoryProvider;

        [NotNull]
        private readonly IEnumerable<Lazy<IPropertyValueResolver>> propertyValueResolvers;

        [NotNull]
        private readonly ProxyGenerator proxyGenerator;

        [NotNull]
        private readonly ProxyGenerationOptions proxyGenerationOptions;


        /// <summary>
        /// Initializes a new instance of the <see cref="TypeInstanciator"/> class.
        /// </summary>
        /// <param name="propertyValueFactoryProvider">The <see cref="IPropertyValueFactoryProvider"/>.</param>
        /// <param name="collectionItemFactoryProvider">The <see cref="ICollectionItemFactoryProvider"/>.</param>
        /// <param name="propertyValueResolvers">A list of <see cref="IPropertyValueResolver"/> instances.</param>
        [ImportingConstructor]
        public TypeInstanciator([NotNull] IPropertyValueFactoryProvider propertyValueFactoryProvider,
            [NotNull] ICollectionItemFactoryProvider collectionItemFactoryProvider,
            [NotNull] [ImportMany(typeof(IPropertyValueResolver))] IEnumerable<Lazy<IPropertyValueResolver>> propertyValueResolvers)
        {
            if (propertyValueFactoryProvider == null)
            {
                throw new ArgumentNullException(nameof(propertyValueFactoryProvider));
            }

            if (collectionItemFactoryProvider == null)
            {
                throw new ArgumentNullException(nameof(collectionItemFactoryProvider));
            }

            if (propertyValueResolvers == null)
            {
                throw new ArgumentNullException(nameof(propertyValueResolvers));
            }

            this.propertyValueFactoryProvider = propertyValueFactoryProvider;
            this.collectionItemFactoryProvider = collectionItemFactoryProvider;
            this.propertyValueResolvers = propertyValueResolvers;
            this.proxyGenerator = new ProxyGenerator();
            this.proxyGenerationOptions = new ProxyGenerationOptions
            {
                Selector = new PropertyInterceptorSelector()
            };
        }

        /// <summary>
        /// Creates an instance of the supplied <see cref="Type"/>.
        /// </summary>
        /// <param name="typeInstanciationContext">The <see cref="ITypeInstanciationContext"/> from which to create an instance.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>The created instance.</returns>
        public Object CreateInstance(ITypeInstanciationContext typeInstanciationContext)
        {
            if (typeInstanciationContext == null)
            {
                throw new ArgumentNullException(nameof(typeInstanciationContext));
            }

            var context = typeInstanciationContext as CollectionProxyingInstanciationContext;
            if (context != null)
            {
                return this.CreateProxiedCollectionInstance(context);
            }

            var instanciationContext = typeInstanciationContext as InstanceProxyingInstanciationContext;
            if (instanciationContext != null)
            {
                return this.CreateProxiedInstance(instanciationContext);
            }

            var defaultTypeInstanciationContext = typeInstanciationContext as DefaultTypeInstanciationContext;
            return this.CreateDefaultInstance(defaultTypeInstanciationContext ?? typeInstanciationContext);
        }

        /// <summary>
        /// Creates a non proxied default instance of the <see cref="Type"/>.
        /// </summary>
        /// <param name="defaultTypeInstanciationContext">The <see cref="DefaultTypeInstanciationContext"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="defaultTypeInstanciationContext"/>' cannot be null.</exception>
        /// <returns>The created instance.</returns>
        private Object CreateDefaultInstance([NotNull] ITypeInstanciationContext defaultTypeInstanciationContext)
        {
            if (defaultTypeInstanciationContext == null)
            {
                throw new ArgumentNullException(nameof(defaultTypeInstanciationContext));
            }

            return Activator.CreateInstance(defaultTypeInstanciationContext.RequestedType,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic, null, null, null);
        }

        /// <summary>
        /// Creates a proxied instance of the <see cref="Type"/>.
        /// </summary>
        /// <param name="instanceProxyingInstanciationContext">The <see cref="InstanceProxyingInstanciationContext"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instanceProxyingInstanciationContext"/>' cannot be null.</exception>
        /// <returns>The created instance.</returns>
        private Object CreateProxiedInstance(
            [NotNull] InstanceProxyingInstanciationContext instanceProxyingInstanciationContext)
        {
            if (instanceProxyingInstanciationContext == null)
            {
                throw new ArgumentNullException(nameof(instanceProxyingInstanciationContext));
            }

            var allowedInterceptors = new List<IInterceptor>();
            if (!instanceProxyingInstanciationContext.NoChangeTracking)
            {
                allowedInterceptors.AddRange(new IInterceptor[]
                {
                    new PrimitivePropertyChangeTrackingInterceptor(
                        instanceProxyingInstanciationContext.InstanceDescriptor, this.propertyValueFactoryProvider,
                        instanceProxyingInstanciationContext.InstanceRelationStore),
                    new ComplexPropertyChangeTrackingInterceptor(
                        instanceProxyingInstanciationContext.InstanceDescriptor, this.propertyValueFactoryProvider,
                        instanceProxyingInstanciationContext.InstanceRelationStore)
                });
            }

            if (!instanceProxyingInstanciationContext.NoLazyLoading)
            {
                allowedInterceptors.Add(
                    new LazyLoadingComplexPropertyInterceptor(instanceProxyingInstanciationContext.InstanceDescriptor,
                        instanceProxyingInstanciationContext.InstanceRelationStore,
                        this.propertyValueResolvers.Select(_ => _.Value)));
            }

            var proxiedInstance =
                this.proxyGenerator.CreateClassProxy(instanceProxyingInstanciationContext.RequestedType,
                    this.proxyGenerationOptions,
                    allowedInterceptors.ToArray()
                    );

            return proxiedInstance;
        }

        /// <summary>
        /// Creates a proxied instance of the <see cref="Type"/>, probably a collection.
        /// </summary>
        /// <param name="collectionProxyingInstanciationContext">The <see cref="CollectionProxyingInstanciationContext"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="collectionProxyingInstanciationContext"/>' cannot be null.</exception>
        /// <returns>The created instance.</returns>
        private Object CreateProxiedCollectionInstance(
            [NotNull] CollectionProxyingInstanciationContext collectionProxyingInstanciationContext)
        {
            if (collectionProxyingInstanciationContext == null)
            {
                throw new ArgumentNullException(nameof(collectionProxyingInstanciationContext));
            }

            var allowedInterceptors = new List<IInterceptor>();
            if (!collectionProxyingInstanciationContext.NoChangeTracking)
            {
                allowedInterceptors.AddRange(new IInterceptor[]
                {
                    new CollectionAddMethodInterceptor(collectionProxyingInstanciationContext.InstanceDescriptor,
                        collectionProxyingInstanciationContext.CollectionPropertyAssociation,
                        this.collectionItemFactoryProvider, collectionProxyingInstanciationContext.InstanceRelationStore),
                    new CollectionClearMethodInterceptor(
                        collectionProxyingInstanciationContext.CollectionPropertyAssociation),
                    new CollectionRemoveMethodInterceptor(
                        collectionProxyingInstanciationContext.CollectionPropertyAssociation)
                });
            }

            var proxiedInstance =
                this.proxyGenerator.CreateClassProxy(collectionProxyingInstanciationContext.RequestedType,
                    this.proxyGenerationOptions,
                    allowedInterceptors.ToArray());

            return proxiedInstance;
        }
    }
}