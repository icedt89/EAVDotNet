namespace JanHafner.EAVDotNet.Instrumentation.InstanceSystem
{
    using System;
    using JetBrains.Annotations;

    /// <summary>
    /// Holds information about the collection item to resolve during persistance.
    /// </summary>
    public sealed class CollectionItemCreationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionItemCreationContext"/> class.
        /// </summary>
        /// <param name="itemInstance">The instance of the collection item.</param>
        /// <param name="instanceDescriptorCreationContext">The parent <see cref="InstanceDescriptorCreationContext"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instanceDescriptorCreationContext"/>' cannot be null.</exception>
        public CollectionItemCreationContext([CanBeNull] Object itemInstance,
            [NotNull] InstanceDescriptorCreationContext instanceDescriptorCreationContext)
        {
            if (instanceDescriptorCreationContext == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptorCreationContext));
            }

            this.ItemInstance = itemInstance;
            this.InstanceDescriptorCreationContext = instanceDescriptorCreationContext;
        }
        
        /// <summary>
        /// Gets the instance of the collection item.
        /// </summary>
        [CanBeNull]
        public Object ItemInstance { get; private set; }

        /// <summary>
        /// Gets the parent <see cref="InstanceDescriptorCreationContext"/>.
        /// </summary>
        [NotNull]
        public InstanceDescriptorCreationContext InstanceDescriptorCreationContext { get; private set; }
    }
}
