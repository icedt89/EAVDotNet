namespace JanHafner.EAVDotNet.Instrumentation.InstanceSystem.PropertyValueFactories
{
    using System;
    using System.ComponentModel.Composition;
    using System.Reflection;
    using JanHafner.EAVDotNet.Model;
    using JanHafner.EAVDotNet.Model.Values;

    /// <summary>
    /// Creates instances of <see cref="BooleanPropertyValue"/> instances.
    /// </summary>
    [Export(typeof(IPropertyValueFactory))]
    internal sealed class BooleanPropertyValueFactory : PropertyValueFactory
    {
        /// <summary>
        /// Checks if the supplied <see cref="Type"/> represents a <see cref="Boolean"/>.
        /// </summary>
        /// <param name="propertyType">The <see cref="Type"/> of the property.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyType"/>' cannot be null.</exception>
        /// <returns>A value indicating if the implementor can create the instance.</returns>
        public override Boolean CanCreatePropertyValue(Type propertyType)
        {
            if (propertyType == null)
            {
                throw new ArgumentNullException(nameof(propertyType));
            }

            return propertyType == typeof(Boolean) || propertyType == typeof(Boolean?);
        }

        /// <summary>
        /// Additionally supplies the value of the <see cref="PropertyInfo"/> to the implementor.
        /// </summary>
        /// <param name="propertyValueCreationContext">The <see cref="PropertyValueCreationContext"/>.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <returns>A derivation of the <see cref="PropertyValue"/>.</returns>
        protected override PropertyValue CreatePropertyValueCore(PropertyValueCreationContext propertyValueCreationContext, Object propertyValue)
        {
            if (propertyValue == null)
            {
                return new IgnorablePropertyValue();
            }

            var value = Boolean.Parse(propertyValue.ToString());

            return new BooleanPropertyValue(value, propertyValueCreationContext.PropertyInfo);
        }
    }
}