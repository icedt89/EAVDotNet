namespace JanHafner.EAVDotNet.Model.ValueHandling
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;
    using JetBrains.Annotations;

    /// <summary>
    /// Default implementation of the <see cref="IPropertyValueHandlerFactory"/> interface.
    /// </summary>
    [Export(typeof(IPropertyValueHandlerFactory))]
    internal sealed class PropertyValueHandlerFactory : IPropertyValueHandlerFactory
    {
        [NotNull]
        private readonly IEnumerable<IPropertyValueHandler> propertyValueHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValueHandlerFactory"/> class.
        /// </summary>
        /// <param name="propertyValueHandlers">A list of <see cref="IPropertyValueHandler"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyValueHandlers"/>' cannot be null.</exception>
        [ImportingConstructor]
        public PropertyValueHandlerFactory([NotNull] [ImportMany(typeof(IPropertyValueHandler))] IEnumerable<IPropertyValueHandler> propertyValueHandlers)
        {
            if (propertyValueHandlers == null)
            {
                throw new ArgumentNullException(nameof(propertyValueHandlers));
            }

            this.propertyValueHandlers = propertyValueHandlers;
        }

        /// <summary>
        /// Decides which <see cref="IPropertyValueHandler"/> can handle the supplied <see cref="PropertyValue"/>.
        /// </summary>
        /// <param name="propertyValue">The <see cref="PropertyValue"/>.</param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyInfo"/>' and '<paramref name="propertyValue"/>' cannot be null.</exception>
        /// <returns>The <see cref="IPropertyValueHandler"/>.</returns>
        public IPropertyValueHandler GetPropertyValueHandler(PropertyValue propertyValue, PropertyInfo propertyInfo)
        {
            if (propertyValue == null)
            {
                throw new ArgumentNullException(nameof(propertyValue));
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            return this.propertyValueHandlers.Single(pvh => pvh.CanHandlePropertyValue(propertyValue, propertyInfo));
        }
    }
}