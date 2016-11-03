namespace JanHafner.EAVDotNet.Instrumentation.InstanceSystem.PropertyValueFactories
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using JetBrains.Annotations;

    [Export(typeof(IPropertyValueFactoryProvider))]
    internal sealed class PropertyValueFactoryProvider : IPropertyValueFactoryProvider
    {
        private readonly IEnumerable<IPropertyValueFactory> propertyValueFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValueFactoryProvider"/> class.
        /// </summary>
        /// <param name="propertyValueFactories">The <see cref="IPropertyValueFactory"/> instances.</param>
        [ImportingConstructor]
        public PropertyValueFactoryProvider(
            [NotNull, ImportMany(typeof (IPropertyValueFactory))] IEnumerable<IPropertyValueFactory> propertyValueFactories)
        {
            if (propertyValueFactories == null)
            {
                throw new ArgumentNullException(nameof(propertyValueFactories));
            }

            this.propertyValueFactories = propertyValueFactories;
        }

        /// <summary>
        /// Returns an implementation of the <see cref="IPropertyValueFactory"/> interface which can handle the supplied <see cref="Type"/>.
        /// </summary>
        /// <param name="propertyType">The <see cref="Type"/> of the property.</param>
        /// <returns>The <see cref="IPropertyValueFactory"/> which can handle the <see cref="Type"/>.</returns>
        public IPropertyValueFactory GetPropertyValueFactory(Type propertyType)
        {
            if (propertyType == null)
            {
                throw new ArgumentNullException(nameof(propertyType));
            }

            return this.propertyValueFactories.First(pvf => pvf.CanCreatePropertyValue(propertyType));
        }
    }
}
