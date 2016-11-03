namespace JanHafner.EAVDotNet.Instanciation
{
    using System;
    using JanHafner.EAVDotNet.Model;
    using JetBrains.Annotations;

    /// <summary>
    /// Holds information about the resolution process of a <see cref="IValueAssociation"/>.
    /// </summary>
    public sealed class PropertyValueResolutionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValueResolutionContext"/> class.
        /// </summary>
        /// <param name="propertyValue">The <see cref="IValueAssociation"/>.</param>
        /// <param name="InstanceResolutionContext">The parent <see cref="InstanceResolutionContext"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyValue"/>' and '<paramref name="InstanceResolutionContext"/>' cannot be null.</exception>
        public PropertyValueResolutionContext([NotNull] IValueAssociation propertyValue,
            [NotNull] InstanceResolutionContext InstanceResolutionContext)
        {
            if (propertyValue == null)
            {
                throw new ArgumentNullException(nameof(propertyValue));
            }

            if (InstanceResolutionContext == null)
            {
                throw new ArgumentNullException(nameof(InstanceResolutionContext));
            }

            this.PropertyValue = propertyValue;
            this.InstanceResolutionContext = InstanceResolutionContext;
        }

        /// <summary>
        /// Gets the <see cref="IValueAssociation"/> which value needs to be reoslved.
        /// </summary>
        [NotNull]
        public IValueAssociation PropertyValue { get; private set; }

        /// <summary>
        /// Gets the parent <see cref="InstanceResolutionContext"/>.
        /// </summary>
        [NotNull]
        public InstanceResolutionContext InstanceResolutionContext { get; private set; }
    }
}
