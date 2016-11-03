namespace JanHafner.EAVDotNet.Instrumentation.InstanceSystem.PropertyValueFactories.Collection
{
    using System;
    using System.ComponentModel.Composition;
    using JanHafner.EAVDotNet.Model.Collections;
    using JanHafner.Toolkit.Common.ExtensionMethods;
    using JetBrains.Annotations;

    /// <summary>
    /// Creates instances of <see cref="Enum"/> instances (uses underlying type).
    /// </summary>
    [Export(typeof(ICollectionItemFactory))]
    internal sealed class EnumCollectionItemFactory : ICollectionItemFactory
    {
        [NotNull]
        private readonly Lazy<ICollectionItemFactoryProvider> collectionItemFactoryProvider;

        [ImportingConstructor]
        public EnumCollectionItemFactory([NotNull] Lazy<ICollectionItemFactoryProvider> collectionItemFactoryProvider)
        {
            if (collectionItemFactoryProvider == null)
            {
                throw new ArgumentNullException(nameof(collectionItemFactoryProvider));
            }

            this.collectionItemFactoryProvider = collectionItemFactoryProvider;
        }

        /// <summary>
        /// Checks if the <paramref name="itemType"/> is an <see cref="Enum"/>.
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

            return itemType.TryUnwrapIfNullableType().IsEnum;
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

            var underlyingEnumType = collectionItemCreationContext.ItemInstance.GetType().GetEnumUnderlyingType();

            var collectionItemFactory =
                this.collectionItemFactoryProvider.Value.GetCollectionItemFactory(underlyingEnumType);

            return collectionItemFactory.CreateCollectionItem(new CollectionItemCreationContext(Convert.ChangeType(collectionItemCreationContext.ItemInstance, underlyingEnumType), collectionItemCreationContext.InstanceDescriptorCreationContext));
        }
    }
}