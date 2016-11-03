namespace JanHafner.EAVDotNet.Instrumentation.InstanceSystem
{
    using System;
    using System.Reflection;
    using JanHafner.EAVDotNet.Context.InstanceRelation;
    using JanHafner.EAVDotNet.Model;
    using JetBrains.Annotations;

    /// <summary>
    /// Holds information about the object which is processed during persistance.
    /// </summary>
    public sealed class InstanceDescriptorCreationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceDescriptorCreationContext"/> class.
        /// </summary>
        /// <param name="currentInstance">The current object.</param>
        /// <param name="instanceRelationStore">The <see cref="IInstanceRelationStore"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instanceRelationStore"/>' and '<paramref name="currentInstance"/>' cannot be null.</exception>
        public InstanceDescriptorCreationContext([NotNull] Object currentInstance, [NotNull] IInstanceRelationStore instanceRelationStore)
        {
            if (currentInstance == null)
            {
                throw new ArgumentNullException(nameof(currentInstance));
            }
            
            if (instanceRelationStore == null)
            {
                throw new ArgumentNullException(nameof(instanceRelationStore));
            }

            this.CurrentInstance = currentInstance;
            this.InstanceRelationStore = instanceRelationStore;
        }

        /// <summary>
        /// Gets the object from which the <see cref="InstanceDescriptor"/> will be created.
        /// </summary>
        [NotNull]
        public Object CurrentInstance { get; private set; }
        
        /// <summary>
        /// Gets the list of all created <see cref="InstanceDescriptor"/> instances during the recursive creation process.
        /// Is necessary for self-reference detection.
        /// </summary>
        [NotNull]
        public IInstanceRelationStore InstanceRelationStore { get; private set; }

        /// <summary>
        /// Creates a new <see cref="InstanceDescriptorCreationContext"/> which is based on this <see cref="InstanceDescriptorCreationContext"/>.
        /// Only the current instance is reset.
        /// </summary>
        /// <param name="instance">The new instance.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instance"/>' cannot be null.</exception>
        /// <returns>The new <see cref="InstanceDescriptorCreationContext"/>.</returns>
        [NotNull]
        public InstanceDescriptorCreationContext CreateChildContext([NotNull] Object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return new InstanceDescriptorCreationContext(instance, this.InstanceRelationStore);
        }

        /// <summary>
        /// Creates a new <see cref="CollectionItemCreationContext"/> from this context.
        /// Additionally connects the new <see cref="CollectionItemCreationContext"/> to this <see cref="InstanceDescriptorCreationContext"/>.
        /// </summary>
        /// <param name="item">The instance of the collection item.</param>
        /// <returns>The new <see cref="CollectionItemCreationContext"/>.</returns>
        [NotNull]
        public CollectionItemCreationContext CreateCollectionItemCreationContext([CanBeNull] Object item)
        {
            return new CollectionItemCreationContext(item, this);
        }

        /// <summary>
        /// Creates a new <see cref="PropertyValueCreationContext"/> which is based on this <see cref="InstanceDescriptorCreationContext"/>.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyInfo"/>' cannot be null.</exception>
        /// <returns>The new <see cref="PropertyValueCreationContext"/>.</returns>
        [NotNull]
        public PropertyValueCreationContext CreatePropertyValueCreationContext([NotNull] PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            return new PropertyValueCreationContext(propertyInfo, this);
        }
    }
}
