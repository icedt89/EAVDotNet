namespace JanHafner.EAVDotNet.Context.InstanceRelation
{
    using System;
    using JanHafner.EAVDotNet.Model;
    using JetBrains.Annotations;

    /// <summary>
    /// Event arguments for the Instance Relation Changed event.
    /// </summary>
    public sealed class InstanceRelationChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceRelationChangedEventArgs"/> class.
        /// </summary>
        /// <param name="instanceDescriptor">The associated <see cref="InstanceDescriptor"/>.</param>
        /// <param name="instance">The associated instance.</param>
        /// <param name="changeType">The type of changed occured.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instanceDescriptor"/>' and '<paramref name="instance"/>' cannot be null.</exception>
        public InstanceRelationChangedEventArgs([NotNull] InstanceDescriptor instanceDescriptor,
            [NotNull] Object instance, InstanceRelationChangeType changeType)
        {
            if (instanceDescriptor == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptor));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            this.InstanceDescriptor = instanceDescriptor;
            this.Instance = instance;
            this.ChangeType = changeType;
        }

        /// <summary>
        /// Gets the associated <see cref="InstanceDescriptor"/>.
        /// </summary>
        [NotNull]
        public InstanceDescriptor InstanceDescriptor { get; private set; }

        /// <summary>
        /// Gets the associated <see cref="InstanceDescriptor"/>.
        /// </summary>
        [NotNull]
        public Object Instance { get; private set; }

        /// <summary>
        /// Gets the type of change occured.
        /// </summary>
        public InstanceRelationChangeType ChangeType { get; private set; }
    }
}
