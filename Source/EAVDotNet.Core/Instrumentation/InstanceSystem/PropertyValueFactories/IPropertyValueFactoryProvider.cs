namespace JanHafner.EAVDotNet.Instrumentation.InstanceSystem.PropertyValueFactories
{
    using System;
    using JetBrains.Annotations;

    /// <summary>
    /// The implementation provides instances of <see cref="IPropertyValueFactory"/> implementations.
    /// </summary>
    internal interface IPropertyValueFactoryProvider
    {
        /// <summary>
        /// Returns an implementation of the <see cref="IPropertyValueFactory"/> interface which can handle the supplied <see cref="Type"/>.
        /// </summary>
        /// <param name="propertyType">The <see cref="Type"/> of the property.</param>
        /// <returns>The <see cref="IPropertyValueFactory"/> which can handle the <see cref="Type"/>.</returns>
        [NotNull]
        IPropertyValueFactory GetPropertyValueFactory([NotNull] Type propertyType);
    }
}