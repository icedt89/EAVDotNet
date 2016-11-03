namespace JanHafner.EAVDotNet.Model.ValueHandling
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using JanHafner.EAVDotNet.Classification;
    using JanHafner.EAVDotNet.Model.Values;

    /// <summary>
    /// If the supplied <see cref="PropertyValue"/> can be mapped to the [Key]Id property of the <see cref="InstanceDescriptor"/>
    /// instance then the value is copied to the <see cref="InstanceDescriptor"/>.
    /// This behavior makes it possible to query an <see cref="InstanceDescriptor"/> by the "primary key"-value of the persisted object.
    /// </summary>
    [Export(typeof(IPropertyValueHandler))]
    internal sealed class KeyRelatingPropertyValueHandler : IPropertyValueHandler
    {
        /// <summary>
        /// The decides if the supplied <see cref="PropertyValue"/> can be handled.
        /// This decision is made by inspecting the supplied parameters:
        /// <paramref name="propertyValue"/> is an instance of <see cref="GuidPropertyValue"/> 
        /// AND the <see cref="KeyAttribute"/> is present (like you would do if you used EF)
        /// AND the name of the <see cref="PropertyInfo"/> is "Id"
        /// AND finally the <see cref="Type"/> of the <see cref="PropertyInfo"/> is <see cref="Guid"/>
        /// </summary>
        /// <param name="propertyValue">The <see cref="PropertyValue"/>.</param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyInfo"/>' and '<paramref name="propertyValue"/>' cannot be null.</exception>
        /// <returns>A value indicating, whether, the <see cref="PropertyValue"/> can be handled.</returns>
        public Boolean CanHandlePropertyValue(PropertyValue propertyValue, PropertyInfo propertyInfo)
        {
            if (propertyValue == null)
            {
                throw new ArgumentNullException(nameof(propertyValue));
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            return propertyInfo.Classify() == PropertyClassifier.PropertyClassification.PrimaryKey;
        }

        /// <summary>
        /// Handles the supplied <see cref="PropertyValue"/>.
        /// </summary>
        /// <param name="instanceDescriptor">The <see cref="InstanceDescriptor"/> of the instance containing the <see cref="PropertyValue"/>.</param>
        /// <param name="propertyValue">The <see cref="PropertyValue"/> to handle.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyInfo"/>', '<paramref name="instanceDescriptor"/>' and '<paramref name="propertyValue"/>' cannot be null.</exception>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/>.</param>
        public void HandlePropertyValue(InstanceDescriptor instanceDescriptor, PropertyValue propertyValue, PropertyInfo propertyInfo)
        {
            if (instanceDescriptor == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptor));
            }

            if (propertyValue == null)
            {
                throw new ArgumentNullException(nameof(propertyValue));
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            instanceDescriptor.PropertyValues.Add(propertyValue);
            instanceDescriptor.Id = ((GuidPropertyValue)propertyValue).Value;
        }
    }
}