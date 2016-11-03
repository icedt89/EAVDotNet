namespace JanHafner.EAVDotNet.Model.ValueHandling
{
    using System;
    using System.ComponentModel.Composition;
    using System.Reflection;
    using JanHafner.EAVDotNet.Classification;

    /// <summary>
    /// Does nothing special besides persisting the <see cref="PropertyValue"/>.
    /// </summary>
    [Export(typeof(IPropertyValueHandler))]
    internal class DefaultPropertyValueHandler : IPropertyValueHandler
    {
        /// <summary>
        /// Can handle all <see cref="PropertyValue"/> instances.
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

            return propertyInfo.Classify() != PropertyClassifier.PropertyClassification.PrimaryKey;
        }

        /// <summary>
        /// Handles the supplied <see cref="PropertyValue"/>.
        /// Special case: if the supplied <see cref="PropertyValue"/> is an <see cref="IgnorablePropertyValue"/>, the method returns without doing anything.
        /// </summary>
        /// <param name="instanceDescriptor">The <see cref="InstanceDescriptor"/> of the instance containing the <see cref="PropertyValue"/>.</param>
        /// <param name="propertyValue">The <see cref="PropertyValue"/> to handle.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyInfo"/>', '<paramref name="instanceDescriptor"/>' and '<paramref name="propertyValue"/>' cannot be null.</exception>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/>.</param>
        public void HandlePropertyValue(InstanceDescriptor instanceDescriptor, PropertyValue propertyValue, PropertyInfo propertyInfo)
        {
            if (propertyValue == null)
            {
                throw new ArgumentNullException(nameof(propertyValue));
            }

            if (propertyValue is IgnorablePropertyValue)
            {
                return;
            }

            if (instanceDescriptor == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptor));
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            instanceDescriptor.PropertyValues.Add(propertyValue);
        }
    }
}