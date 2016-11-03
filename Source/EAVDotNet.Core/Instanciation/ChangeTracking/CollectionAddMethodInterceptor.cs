namespace JanHafner.EAVDotNet.Instanciation.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using Castle.DynamicProxy;
    using JanHafner.EAVDotNet.Classification;
    using JanHafner.EAVDotNet.Context.InstanceRelation;
    using JanHafner.EAVDotNet.Instrumentation.InstanceSystem;
    using JanHafner.EAVDotNet.Instrumentation.InstanceSystem.PropertyValueFactories.Collection;
    using JanHafner.EAVDotNet.Model;
    using JanHafner.EAVDotNet.Model.Values;
    using JetBrains.Annotations;

    /// <summary>
    /// Intercepts the Add-method of an <see cref="ICollection{T}"/>.
    /// </summary>
    internal sealed class CollectionAddMethodInterceptor : IInterceptor
    {
        [NotNull]
        private readonly InstanceDescriptor instanceDescriptor;

        [NotNull]
        private readonly CollectionPropertyAssociation collectionPropertyAssociation;

        [NotNull]
        private readonly ICollectionItemFactoryProvider collectionItemFactoryProvider;

        [NotNull]
        private readonly IInstanceRelationStore instanceRelationStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionAddMethodInterceptor"/> class.
        /// </summary>
        /// <param name="instanceDescriptor">The <see cref="Model.InstanceDescriptor"/>.</param>
        /// <param name="collectionPropertyAssociation">The <see cref="Model.Values.CollectionPropertyAssociation"/>.</param>
        /// <param name="collectionItemFactoryProvider">The <see cref="ICollectionItemFactoryProvider"/>.</param>
        /// <param name="instanceRelationStore">The <see cref="IInstanceRelationStore"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="collectionPropertyAssociation"/>', '<paramref name="collectionItemFactoryProvider"/>', '<paramref name="instanceDescriptor"/>' and '<paramref name="instanceRelationStore"/>' cannot be null.</exception>
        public CollectionAddMethodInterceptor([NotNull] InstanceDescriptor instanceDescriptor, [NotNull] CollectionPropertyAssociation collectionPropertyAssociation, [NotNull] ICollectionItemFactoryProvider collectionItemFactoryProvider, [NotNull] IInstanceRelationStore instanceRelationStore)
        {
            if (instanceDescriptor == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptor));
            }

            if (collectionPropertyAssociation == null)
            {
                throw new ArgumentNullException(nameof(collectionPropertyAssociation));
            }

            if (collectionItemFactoryProvider == null)
            {
                throw new ArgumentNullException(nameof(collectionItemFactoryProvider));
            }

            if (instanceRelationStore == null)
            {
                throw new ArgumentNullException(nameof(instanceRelationStore));
            }

            this.instanceDescriptor = instanceDescriptor;
            this.collectionPropertyAssociation = collectionPropertyAssociation;
            this.collectionItemFactoryProvider = collectionItemFactoryProvider;
            this.instanceRelationStore = instanceRelationStore;
        }

        /// <summary>
        /// Intercepts the method.
        /// </summary>
        /// <param name="invocation">Information about the invocation.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="invocation"/>' cannot be null.</exception>
        public void Intercept([NotNull] IInvocation invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            var collectionItemFactory = this.collectionItemFactoryProvider.GetCollectionItemFactory(invocation.Arguments[0].GetType().TryUnwrapProxiedType());
            var collectionItem =
                collectionItemFactory.CreateCollectionItem(new CollectionItemCreationContext(invocation.Arguments[0],
                    new InstanceDescriptorCreationContext(this.instanceDescriptor, this.instanceRelationStore)));

            this.collectionPropertyAssociation.CollectionItemDescriptors.Add(collectionItem);

            invocation.Proceed();
        }
    }
}
