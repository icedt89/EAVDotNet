namespace JanHafner.EAVDotNet.Instrumentation.InstanceSystem.PropertyValueFactories.Collection
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using JetBrains.Annotations;

    /// <summary>
    /// Implementation of the <see cref="ICollectionItemFactoryProvider"/> interface.
    /// </summary>
    [Export(typeof(ICollectionItemFactoryProvider))]
    internal sealed class CollectionItemFactoryProvider : ICollectionItemFactoryProvider
    {
        [NotNull]
        private readonly IEnumerable<ICollectionItemFactory> collectionItemFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionItemFactoryProvider"/> class.
        /// </summary>
        /// <param name="collectionItemFactories">A list containing all <see cref="ICollectionItemFactory"/> instances.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="collectionItemFactories"/>' cannot be null.</exception>
        [ImportingConstructor]
        public CollectionItemFactoryProvider(
            [NotNull, ImportMany(typeof (ICollectionItemFactory))] IEnumerable<ICollectionItemFactory> collectionItemFactories)
        {
            if (collectionItemFactories == null)
            {
                throw new ArgumentNullException(nameof(collectionItemFactories));
            }

            this.collectionItemFactories = collectionItemFactories;
        }

        /// <summary>
        /// Returns an instance of the implementation of the <see cref="ICollectionItemFactory"/> which can handle the supplied item <see cref="Type"/>.
        /// </summary>
        /// <param name="itemType">The <see cref="Type"/> of the item.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="itemType"/>' cannot be null.</exception>
        /// <returns>An implementation of the <see cref="ICollectionItemFactory"/> interface.</returns>
        public ICollectionItemFactory GetCollectionItemFactory(Type itemType)
        {
            if (itemType == null)
            {
                throw new ArgumentNullException(nameof(itemType));
            }

            return this.collectionItemFactories.Single(cif => cif.CanCreateCollectionItem(itemType));
        }
    }
}
