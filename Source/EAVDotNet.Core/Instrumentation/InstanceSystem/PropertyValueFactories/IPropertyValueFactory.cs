namespace JanHafner.EAVDotNet.Instrumentation.InstanceSystem.PropertyValueFactories
{
    using System;
    using JanHafner.EAVDotNet.Model;
    using JetBrains.Annotations;

    /// <summary>
    /// The implementation is responsible for creation of a <see cref="PropertyValue"/> derivation.
    /// </summary>
    internal interface IPropertyValueFactory
    {
        /// <summary>
        /// Checks if the implementor can create a <see cref="PropertyValue"/> derived intance from the supplied <see cref="Type"/>.
        /// </summary>
        /// <param name="propertyType">The <see cref="Type"/> of the property.</param>
        /// <returns>A value indicating if the implementor can create the instance.</returns>
        bool CanCreatePropertyValue([NotNull] Type propertyType);
        
        /// <summary>
        /// Finally creates the <see cref="PropertyValue"/> derivation using the information contained in the supplied <see cref="PropertyValueCreationContext"/>.
        /// </summary>
        /// <param name="propertyValueCreationContext">The <see cref="PropertyValueCreationContext"/>.</param>
        /// <returns>A derivation of the <see cref="PropertyValue"/>.</returns>
        [NotNull]
        PropertyValue CreatePropertyValue([NotNull] PropertyValueCreationContext propertyValueCreationContext);
    }
}