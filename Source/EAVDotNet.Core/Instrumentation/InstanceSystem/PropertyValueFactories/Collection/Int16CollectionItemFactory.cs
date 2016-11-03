namespace JanHafner.EAVDotNet.Instrumentation.InstanceSystem.PropertyValueFactories.Collection
{
    using System;
    using System.ComponentModel.Composition;
    using JanHafner.EAVDotNet.Model.Collections;

    /// <summary>
    /// Creates instances of <see cref="Int16CollectionItem"/> instances.
    /// </summary>
    [Export(typeof(ICollectionItemFactory))]
    internal sealed class Int16CollectionItemFactory : ICollectionItemFactory
    {
        /// <summary>
        /// Checks if the <paramref name="itemType"/> is a <see cref="Int16"/>.
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

            return itemType == typeof(Int16) || itemType == typeof(Int16?);
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

            var value = Int16.Parse(collectionItemCreationContext.ItemInstance.ToString());

            return new Int16CollectionItem(value);
        }
    }
}