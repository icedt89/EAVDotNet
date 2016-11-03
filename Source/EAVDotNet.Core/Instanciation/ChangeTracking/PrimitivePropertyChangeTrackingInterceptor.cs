namespace JanHafner.EAVDotNet.Instanciation.ChangeTracking
{
    using System;
    using System.Reflection;
    using JanHafner.EAVDotNet.Context.InstanceRelation;
    using JanHafner.EAVDotNet.Instrumentation.InstanceSystem;
    using JanHafner.EAVDotNet.Instrumentation.InstanceSystem.PropertyValueFactories;
    using JanHafner.EAVDotNet.Model;
    using JetBrains.Annotations;

    /// <summary>
    /// Intercepts the property set of a, as Primitive classified, property.
    /// </summary>
    internal sealed class PrimitivePropertyChangeTrackingInterceptor : PropertyChangeTrackingInterceptor
    {
        [NotNull]
        private readonly IInstanceRelationStore instanceRelationStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitivePropertyChangeTrackingInterceptor"/> class.
        /// </summary>
        /// <param name="instanceDescriptor">The associated <see cref="InstanceDescriptor"/>.</param>
        /// <param name="instanceRelationStore">The <see cref="IInstanceRelationStore"/>.</param>
        /// <param name="propertyValueFactoryProvider">The <see cref="IPropertyValueFactoryProvider"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instanceRelationStore"/>', '<paramref name="propertyValueFactoryProvider"/>' and '<paramref name="instanceDescriptor"/>' cannot be null.</exception>
        public PrimitivePropertyChangeTrackingInterceptor([NotNull] InstanceDescriptor instanceDescriptor, [NotNull] IPropertyValueFactoryProvider propertyValueFactoryProvider,
            [NotNull] IInstanceRelationStore instanceRelationStore) : base(instanceDescriptor, propertyValueFactoryProvider)
        {
            if (instanceRelationStore == null)
            {
                throw new ArgumentNullException(nameof(instanceRelationStore));
            }

            this.instanceRelationStore = instanceRelationStore;
        }

        /// <summary>
        /// Creates a new <see cref="PropertyValue"/>.
        /// </summary>
        /// <param name="propertyValueFactory">The <see cref="IPropertyValueFactory"/>.</param>
        /// <param name="instance">The instance of the complex object.</param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> which is intercepted.</param>
        /// <param name="value">The intercepted property value.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyValueFactory" />', '<paramref name="instance" />', '<paramref name="propertyInfo"/>' and '<paramref name="value"/>' cannot be null.</exception>
        /// <returns>The created <see cref="PropertyValue"/>.</returns>
        [NotNull]
        protected override PropertyValue CreateNewPropertyValue([NotNull] IPropertyValueFactory propertyValueFactory,
            [NotNull] Object instance, [NotNull] PropertyInfo propertyInfo, [NotNull] Object value)
        {
            if (propertyValueFactory == null)
            {
                throw new ArgumentNullException(nameof(propertyValueFactory));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return propertyValueFactory.CreatePropertyValue(
                PropertyValueCreationContext.ForExistingPropertyValue(propertyInfo,
                    new InstanceDescriptorCreationContext(instance, this.instanceRelationStore), value));
        }

        /// <summary>
        /// Returns the value of an existing <see cref="PropertyValue"/>.
        /// </summary>
        /// <param name="propertyValueFactory">The <see cref="IPropertyValueFactory"/>.</param>
        /// <param name="instance">The instance of the complex object.</param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> which is intercepted.</param>
        /// <param name="value">The intercepted property value.</param>
        /// <returns>The value of the existing <see cref="PropertyValue"/>.</returns>
        [CanBeNull]
        protected override Object CreateValueForExistingPropertyValue(
            [CanBeNull] IPropertyValueFactory propertyValueFactory, [CanBeNull] Object instance,
            [CanBeNull] PropertyInfo propertyInfo, [CanBeNull] Object value)
        {
            return value;
        }
    }
}
