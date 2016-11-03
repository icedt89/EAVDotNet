namespace JanHafner.EAVDotNet.Model.ValueHandling
{
    using System.Reflection;
    using JetBrains.Annotations;

    /// <summary>
    /// The implementation decides which <see cref="IPropertyValueHandler"/> should be used.
    /// </summary>
    internal interface IPropertyValueHandlerFactory
    {
        /// <summary>
        /// Decides which <see cref="IPropertyValueHandler"/> can handle the supplied <see cref="PropertyValue"/>.
        /// </summary>
        /// <param name="propertyValue">The <see cref="PropertyValue"/>.</param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/>.</param>
        /// <returns>The <see cref="IPropertyValueHandler"/>.</returns>
        [NotNull]
        IPropertyValueHandler GetPropertyValueHandler([NotNull] PropertyValue propertyValue, [NotNull] PropertyInfo propertyInfo);
    }
}
