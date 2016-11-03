namespace JanHafner.EAVDotNet.Instanciation
{
    using System;
    using JanHafner.EAVDotNet.Context.InstanceRelation;
    using JanHafner.EAVDotNet.Model;
    using JetBrains.Annotations;

    /// <summary>
    /// Holds information about the instance beeing resolved.
    /// </summary>
    public sealed class InstanceResolutionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceResolutionContext"/> class.
        /// </summary>
        /// <param name="instanceDescriptor">The <see cref="InstanceDescriptor"/>.</param>
        /// <param name="instanceRelationStore">The <see cref="IInstanceRelationStore"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instanceDescriptor"/>' and '<paramref name="instanceRelationStore"/>' cannot be null.</exception>
        public InstanceResolutionContext([NotNull] InstanceDescriptor instanceDescriptor, [NotNull] IInstanceRelationStore instanceRelationStore)
        {
            if (instanceDescriptor == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptor));
            }

            if (instanceRelationStore == null)
            {
                throw new ArgumentNullException(nameof(instanceRelationStore));
            }

            this.InstanceDescriptor = instanceDescriptor;
            this.InstanceRelationStore = instanceRelationStore;
        }

        /// <summary>
        /// Gets the <see cref="InstanceDescriptor"/> which is about to create.
        /// </summary>
        [NotNull]
        public InstanceDescriptor InstanceDescriptor { get; private set; }

        /// <summary>
        /// Gets the list of all created instances during the recursive creation process.
        /// Is necessary for self-reference detection.
        /// </summary>
        [NotNull]
        public IInstanceRelationStore InstanceRelationStore { get; private set; }

        /// <summary>
        /// Gets a value indicating, whether, change tracking proxies should be generated.
        /// </summary>
        public Boolean NoChangeTracking { get; set; }

        /// <summary>
        /// Gets a value indicating, whether, lazy loading proxies should be generated.
        /// </summary>
        public Boolean NoLazyLoading { get; set; }

        /// <summary>
        /// Creates a new child <see cref="InstanceResolutionContext"/>.
        /// </summary>
        /// <param name="instanceDescriptor">The new <see cref="InstanceDescriptor"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instanceDescriptor"/>' cannot be null.</exception>
        /// <returns></returns>
        [NotNull]
        public InstanceResolutionContext CreateChildContext([NotNull] InstanceDescriptor instanceDescriptor)
        {
            if (instanceDescriptor == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptor));
            }

            return new InstanceResolutionContext(instanceDescriptor, this.InstanceRelationStore)
            {
                NoChangeTracking = this.NoChangeTracking,
                NoLazyLoading = this.NoLazyLoading
            };
        }

        /// <summary>
        /// Creates a new <see cref="PropertyValueResolutionContext"/> for the supplied <see cref="IValueAssociation"/>.
        /// </summary>
        /// <param name="propertyValue">The <see cref="IValueAssociation"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyValue"/>' cannot be null.</exception>
        /// <returns></returns>
        [NotNull]
        public PropertyValueResolutionContext CreatePropertyValueResolutionContext([NotNull] IValueAssociation propertyValue)
        {
            if (propertyValue == null)
            {
                throw new ArgumentNullException(nameof(propertyValue));
            }

            return new PropertyValueResolutionContext(propertyValue, this);
        }
    }
}
