namespace JanHafner.EAVDotNet.Instrumentation.InstanceSystem
{
    using System;
    using System.Reflection;
    using JetBrains.Annotations;

    /// <summary>
    /// Holds information about the property value to resolve during persistance.
    /// </summary>
    public sealed class PropertyValueCreationContext
    {
        [CanBeNull]
        private Object contextValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValueCreationContext"/> class.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/>.</param>
        /// <param name="instanceDescriptorCreationContext">The parent <see cref="InstanceDescriptorCreationContext"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyInfo"/>' and '<paramref name="instanceDescriptorCreationContext"/>' cannot be null.</exception>
        public PropertyValueCreationContext([NotNull] PropertyInfo propertyInfo,
            [NotNull] InstanceDescriptorCreationContext instanceDescriptorCreationContext)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (instanceDescriptorCreationContext == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptorCreationContext));
            }

            this.PropertyInfo = propertyInfo;
            this.InstanceDescriptorCreationContext = instanceDescriptorCreationContext;
        }

        /// <summary>
        /// Gets the parent <see cref="InstanceDescriptorCreationContext"/>.
        /// </summary>
        [NotNull]
        public InstanceDescriptorCreationContext InstanceDescriptorCreationContext { get; private set; }

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> which value needs to be persisted.
        /// </summary>
        [NotNull]
        public PropertyInfo PropertyInfo { get; private set; }

        /// <summary>
        /// Gets the value of the <see cref="PropertyInfo"/> in association with the supplied instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>The property value.</returns>
        [CanBeNull]
        public Object GetValue([NotNull] Object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return this.contextValue ?? (this.contextValue = this.PropertyInfo.GetValue(instance));
        }

        /// <summary>
        /// Creates a new <see cref="PropertyValueCreationContext"/> by using the supplied pre computed value.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/>.</param>
        /// <param name="instanceDescriptorCreationContext">The parent <see cref="InstanceDescriptorCreationContext"/>.</param>
        /// <param name="value">The pre computed value.</param>
        /// <returns>The newly created context.</returns>
        [NotNull]
        internal static PropertyValueCreationContext ForExistingPropertyValue([NotNull] PropertyInfo propertyInfo,
            [NotNull] InstanceDescriptorCreationContext instanceDescriptorCreationContext, [NotNull] Object value)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (instanceDescriptorCreationContext == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptorCreationContext));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return new PropertyValueCreationContext(propertyInfo, instanceDescriptorCreationContext)
            {
                contextValue = value
            };
        }
    }
}
