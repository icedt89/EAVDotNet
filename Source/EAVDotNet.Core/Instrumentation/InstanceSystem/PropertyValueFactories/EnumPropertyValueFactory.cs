namespace JanHafner.EAVDotNet.Instrumentation.InstanceSystem.PropertyValueFactories
{
    using System;
    using System.ComponentModel.Composition;
    using System.Reflection;
    using JanHafner.EAVDotNet.Model;
    using JanHafner.Toolkit.Common.ExtensionMethods;
    using JetBrains.Annotations;

    /// <summary>
    /// Creates instances of <see cref="Enum"/> instances (uses underlying type).
    /// </summary>
    [Export(typeof(IPropertyValueFactory))]
    internal sealed class EnumPropertyValueFactory : PropertyValueFactory
    {
        [NotNull]
        private readonly Lazy<IPropertyValueFactoryProvider> propertyValueFactoryProvider;

        [ImportingConstructor]
        public EnumPropertyValueFactory(
            [NotNull] Lazy<IPropertyValueFactoryProvider> propertyValueFactoryProvider)
        {
            if (propertyValueFactoryProvider == null)
            {
                throw new ArgumentNullException(nameof(propertyValueFactoryProvider));
            }

            this.propertyValueFactoryProvider = propertyValueFactoryProvider;
        }

        /// <summary>
        /// Checks if the supplied <see cref="Type"/> represents an <see cref="Enum"/>.
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

            return propertyType.TryUnwrapIfNullableType().IsEnum;
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

            var underlyingEnumType = propertyValueCreationContext.PropertyInfo.PropertyType.GetEnumUnderlyingType();

            var propertyValueFactory = this.propertyValueFactoryProvider.Value.GetPropertyValueFactory(underlyingEnumType);

            return propertyValueFactory.CreatePropertyValue(PropertyValueCreationContext.ForExistingPropertyValue(propertyValueCreationContext.PropertyInfo, propertyValueCreationContext.InstanceDescriptorCreationContext, Convert.ChangeType(propertyValue, underlyingEnumType)));
        }
    }
}