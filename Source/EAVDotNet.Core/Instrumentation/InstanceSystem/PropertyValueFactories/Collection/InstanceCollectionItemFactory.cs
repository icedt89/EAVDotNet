using JanHafner.EAVDotNet.Classification;

namespace JanHafner.EAVDotNet.Instrumentation.InstanceSystem.PropertyValueFactories.Collection
{
    using System;
    using System.ComponentModel.Composition;
    using JanHafner.EAVDotNet.Model;
    using JanHafner.EAVDotNet.Model.Collections;
    using JetBrains.Annotations;

    /// <summary>
    /// Creates instances of <see cref="BooleanCollectionItem"/> instances.
    /// </summary>
    [Export(typeof(ICollectionItemFactory))]
    internal sealed class InstanceCollectionItemFactory : ICollectionItemFactory
    {
        [NotNull]
        private readonly Lazy<IInstanceDescriptorFactory> instanceDescriptorFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceCollectionItemFactory"/> class.
        /// </summary>
        /// <param name="instanceDescriptorFactory">The <see cref="IInstanceDescriptorFactory"/>.</param>
        [ImportingConstructor]
        public InstanceCollectionItemFactory([NotNull] Lazy<IInstanceDescriptorFactory> instanceDescriptorFactory)
        {
            if (instanceDescriptorFactory == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptorFactory));
            }

            this.instanceDescriptorFactory = instanceDescriptorFactory;
        }

        /// <summary>
        /// Checks if the <paramref name="itemType"/> is a complex <see cref="Type"/>.
        /// </summary>
        /// <param name="itemType">The <see cref="Type"/> of the item.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="itemType"/>' cannot be null.</exception>
        /// <returns>A value indicating, whether, this implementation can create a <see cref="CollectionItem"/> from the supplied instance.</returns>
        public Boolean CanCreateCollectionItem(Type itemType)
        {
            if (itemType == null)
            {
                throw new ArgumentNullException(nameof(itemType));
            }

            return itemType.Classify() == TypeClassifier.TypeClassification.Complex;
        }

        /// <summary>
        /// Creates the <see cref="CollectionItem"/> from information contained in the supplied <see cref="CollectionItemCreationContext"/>.
        /// </summary>
        /// <param name="collectionItemCreationContext">The <see cref="CollectionItemCreationContext"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="collectionItemCreationContext"/>' cannot be null.</exception>
        /// <returns>The newly created derivation of the <see cref="CollectionItem"/>.</returns>
        public CollectionItem CreateCollectionItem(CollectionItemCreationContext collectionItemCreationContext)
        {
            if (collectionItemCreationContext == null)
            {
                throw new ArgumentNullException(nameof(collectionItemCreationContext));
            }

            if (collectionItemCreationContext.ItemInstance == null)
            {
                return new IgnorableCollectionItem();
            }

            InstanceDescriptor instanceDescriptor;
            if (collectionItemCreationContext.InstanceDescriptorCreationContext.InstanceRelationStore.TryGetRelated(collectionItemCreationContext.ItemInstance, out instanceDescriptor))
            {
                if (instanceDescriptor == null)
                {
                    throw new InvalidOperationException();
                }
            }

            if (instanceDescriptor == null)
            {
                instanceDescriptor = this.instanceDescriptorFactory.Value.CreateInstanceDescriptor(collectionItemCreationContext.InstanceDescriptorCreationContext.CreateChildContext(collectionItemCreationContext.ItemInstance));
            }

            return new InstanceCollectionItemAssociation(instanceDescriptor);
        }
    }
}