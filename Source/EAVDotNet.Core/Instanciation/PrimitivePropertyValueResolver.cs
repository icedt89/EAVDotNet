namespace JanHafner.EAVDotNet.Instanciation
{
    using System;
    using System.ComponentModel.Composition;
    using JanHafner.EAVDotNet.Model;

    /// <summary>
    /// Is responsible for resolving primitive property values.
    /// </summary>
    [Export(typeof(IPropertyValueResolver))]
    internal sealed class PrimitivePropertyValueResolver : IPropertyValueResolver
    {
        /// <summary>
        /// Checks if the supplied <see cref="IValueAssociation"/> is an <see cref="IPrimitiveValueAssociation"/>.
        /// </summary>
        /// <param name="propertyValue">The <see cref="IValueAssociation"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyValue"/>' cannot be null.</exception>
        /// <returns>A value indicating, whether, this instance can handle the <see cref="IValueAssociation"/>.</returns>
        public Boolean CanResolveValue(IValueAssociation propertyValue)
        {
            if (propertyValue == null)
            {
                throw new ArgumentNullException(nameof(propertyValue));
            }

            return propertyValue is IPrimitiveValueAssociation;
        }

        /// <summary>
        /// Resolves the primitive value from the <see cref="IValueAssociation"/>.
        /// </summary>
        /// <param name="propertyValueResolutionContext">The <see cref="PropertyValueResolutionContext"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyValueResolutionContext"/>' cannot be null.</exception>
        /// <returns>The resolved value.</returns>
        public Object ResolveValue(PropertyValueResolutionContext propertyValueResolutionContext)
        {
            if (propertyValueResolutionContext == null)
            {
                throw new ArgumentNullException(nameof(propertyValueResolutionContext));
            }

            var primitivePropertyValue = (IPrimitiveValueAssociation)propertyValueResolutionContext.PropertyValue;

            return primitivePropertyValue.Value;
        }
    }
}