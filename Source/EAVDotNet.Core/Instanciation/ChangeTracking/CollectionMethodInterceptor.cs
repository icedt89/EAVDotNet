namespace JanHafner.EAVDotNet.Instanciation.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using Castle.DynamicProxy;
    using JanHafner.EAVDotNet.Context.InstanceRelation;
    using JanHafner.EAVDotNet.Instrumentation.InstanceSystem.PropertyValueFactories.Collection;
    using JanHafner.EAVDotNet.Model;
    using JanHafner.EAVDotNet.Model.Values;
    using JetBrains.Annotations;

    /// <summary>
    /// Base class for interceptors which intercept calls on a <see cref="ICollection{T}"/>.
    /// </summary>
    internal abstract class CollectionMethodInterceptor : IInterceptor
    {
        [NotNull]
        protected readonly InstanceDescriptor InstanceDescriptor;

        [NotNull]
        protected readonly CollectionPropertyAssociation CollectionPropertyAssociation;

        [NotNull]
        protected readonly ICollectionItemFactoryProvider CollectionItemFactoryProvider;

        [NotNull]
        protected readonly IInstanceRelationStore InstanceRelationStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionItemsBindingInterceptor"/> class.
        /// </summary>
        /// <param name="instanceDescriptor">The <see cref="Model.InstanceDescriptor"/>.</param>
        /// <param name="collectionPropertyAssociation">The <see cref="Model.Values.CollectionPropertyAssociation"/>.</param>
        /// <param name="collectionItemFactoryProvider">The <see cref="ICollectionItemFactoryProvider"/>.</param>
        /// <param name="instanceRelationStore">The <see cref="IInstanceRelationStore"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="collectionPropertyAssociation"/>', '<paramref name="collectionItemFactoryProvider"/>', '<paramref name="instanceDescriptor"/>' and '<paramref name="instanceRelationStore"/>' cannot be null.</exception>
        protected CollectionMethodInterceptor([NotNull] InstanceDescriptor instanceDescriptor, [NotNull] CollectionPropertyAssociation collectionPropertyAssociation, [NotNull] ICollectionItemFactoryProvider collectionItemFactoryProvider, [NotNull] IInstanceRelationStore instanceRelationStore)
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

            this.InstanceDescriptor = instanceDescriptor;
            this.CollectionPropertyAssociation = collectionPropertyAssociation;
            this.CollectionItemFactoryProvider = collectionItemFactoryProvider;
            this.InstanceRelationStore = instanceRelationStore;
        }

        /// <summary>
        /// Intercepts the method.
        /// </summary>
        /// <param name="invocation">Information about the invocation.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="invocation"/>' cannot be null.</exception>
        public abstract void Intercept(IInvocation invocation);
    }
}
