namespace JanHafner.EAVDotNet.Instrumentation.InstanceSystem.PropertyValueFactories
{
    using System;
    using System.Reflection;
    using JanHafner.EAVDotNet.Model;
    using JetBrains.Annotations;

    /// <summary>
    /// Base class for all <see cref="IPropertyValueFactory"/> implementations.
    /// </summary>
    internal abstract class PropertyValueFactory : IPropertyValueFactory
    {
        /// <summary>
        /// Checks if the implementor can create a <see cref="PropertyValue"/> derived intance from the supplied <see cref="Type"/>.
        /// </summary>
        /// <param name="propertyType">The <see cref="Type"/> of the property.</param>
        /// <returns>A value indicating if the implementor can create the instance.</returns>
        public abstract Boolean CanCreatePropertyValue(Type propertyType);

        /// <summary>
        /// Finally creates the <see cref="PropertyValue"/> derivation using the information contained in the supplied <see cref="PropertyValueCreationContext"/>.
        /// </summary>
        /// <param name="propertyValueCreationContext">The <see cref="PropertyValueCreationContext"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyValueCreationContext"/>' cannot be null.</exception>
        /// <returns>A derivation of the <see cref="PropertyValue"/>.</returns>
        public PropertyValue CreatePropertyValue(PropertyValueCreationContext propertyValueCreationContext)
        {
            if (propertyValueCreationContext == null)
            {
                throw new ArgumentNullException(nameof(propertyValueCreationContext));
            }

            var propertyValue = propertyValueCreationContext.GetValue(propertyValueCreationContext.InstanceDescriptorCreationContext.CurrentInstance);

            return this.CreatePropertyValueCore(propertyValueCreationContext, propertyValue);
        }

        /// <summary>
        /// Additionally supplies the value of the <see cref="PropertyInfo"/> to the implementor.
        /// </summary>
        /// <param name="propertyValueCreationContext">The <see cref="PropertyValueCreationContext"/>.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <returns>A derivation of the <see cref="PropertyValue"/>.</returns>
        [NotNull]
        protected abstract PropertyValue CreatePropertyValueCore([NotNull] PropertyValueCreationContext propertyValueCreationContext, [CanBeNull] Object propertyValue);
    }
}