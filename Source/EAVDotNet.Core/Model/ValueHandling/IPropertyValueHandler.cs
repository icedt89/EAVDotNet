namespace JanHafner.EAVDotNet.Model.ValueHandling
{
    using System;
    using System.Reflection;
    using JetBrains.Annotations;

    /// <summary>
    /// The implementation is responsible for handling the resolved <see cref="PropertyValue"/>
    /// </summary>
    internal interface IPropertyValueHandler
    {
        /// <summary>
        /// The decides if the supplied <see cref="PropertyValue"/> can be handled.
        /// </summary>
        /// <param name="propertyValue">The <see cref="PropertyValue"/>.</param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/>.</param>
        /// <returns>A value indicating, whether, the <see cref="PropertyValue"/> can be handled.</returns>
        Boolean CanHandlePropertyValue([NotNull] PropertyValue propertyValue, [NotNull] PropertyInfo propertyInfo);

        /// <summary>
        /// Handles the supplied <see cref="PropertyValue"/>.
        /// </summary>
        /// <param name="instanceDescriptor">The <see cref="InstanceDescriptor"/> of the instance containing the <see cref="PropertyValue"/>.</param>
        /// <param name="propertyValue">The <see cref="PropertyValue"/> to handle.</param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/>.</param>
        void HandlePropertyValue([NotNull] InstanceDescriptor instanceDescriptor, [NotNull] PropertyValue propertyValue, [NotNull] PropertyInfo propertyInfo);
    }
}