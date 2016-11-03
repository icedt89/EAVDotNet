namespace JanHafner.EAVDotNet.Instanciation
{
    using System;
    using JanHafner.EAVDotNet.Model;
    using JetBrains.Annotations;

    /// <summary>
    /// The implementation resolves instances of properties.
    /// </summary>
    internal interface IPropertyValueResolver
    {
        /// <summary>
        /// The implementation checks if the <see cref="IValueAssociation"/> can be handled.
        /// </summary>
        /// <param name="propertyValue">The <see cref="IValueAssociation"/>.</param>
        /// <returns>A value indicating, whether, this instance can handle the <see cref="IValueAssociation"/>.</returns>
        Boolean CanResolveValue([NotNull] IValueAssociation propertyValue);

        /// <summary>
        /// The implementation resolves the value by using the information contained in the supplied <see cref="PropertyValueResolutionContext"/>.
        /// </summary>
        /// <param name="propertyValueResolutionContext">The <see cref="PropertyValueResolutionContext"/>.</param>
        /// <returns>The resolved value.</returns>
        [NotNull]
        Object ResolveValue([NotNull] PropertyValueResolutionContext propertyValueResolutionContext);
    }
}