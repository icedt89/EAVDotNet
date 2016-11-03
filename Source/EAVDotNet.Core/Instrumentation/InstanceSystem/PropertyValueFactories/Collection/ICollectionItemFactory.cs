namespace JanHafner.EAVDotNet.Instrumentation.InstanceSystem.PropertyValueFactories.Collection
{
    using System;
    using JanHafner.EAVDotNet.Model.Collections;
    using JetBrains.Annotations;

    /// <summary>
    /// Implementations are responsible for creating <see cref="CollectionItem"/> derivations.
    /// </summary>
    internal interface ICollectionItemFactory
    {
        /// <summary>
        /// The implementation checks if the supplied <paramref name="itemType"/> is supported by this factory.
        /// </summary>
        /// <param name="itemType">The <see cref="Type"/> of the item.</param>
        /// <returns>A value indicating, whether, this implementation can create a <see cref="CollectionItem"/> from the supplied instance.</returns>
        Boolean CanCreateCollectionItem([NotNull] Type itemType);

        /// <summary>
        /// Creates the <see cref="CollectionItem"/> from information contained in the supplied <see cref="CollectionItemCreationContext"/>.
        /// </summary>
        /// <param name="collectionItemCreationContext">The <see cref="CollectionItemCreationContext"/>.</param>
        /// <returns>The newly created derivation of the <see cref="CollectionItem"/>.</returns>
        [NotNull]
        CollectionItem CreateCollectionItem([NotNull] CollectionItemCreationContext collectionItemCreationContext);
    }
}