namespace JanHafner.EAVDotNet.Instrumentation.InstanceSystem.PropertyValueFactories.Collection
{
    using System;
    using JetBrains.Annotations;

    /// <summary>
    /// Provides implementations of the <see cref="ICollectionItemFactory"/> interface.
    /// </summary>
    internal interface ICollectionItemFactoryProvider
    {
        /// <summary>
        /// Returns an instance of the implementation of the <see cref="ICollectionItemFactory"/> which can handle the supplied item <see cref="Type"/>.
        /// </summary>
        /// <param name="itemType">The <see cref="Type"/> of the item.</param>
        /// <returns>An implementation of the <see cref="ICollectionItemFactory"/> interface.</returns>
        [NotNull]
        ICollectionItemFactory GetCollectionItemFactory([NotNull] Type itemType);
    }
}