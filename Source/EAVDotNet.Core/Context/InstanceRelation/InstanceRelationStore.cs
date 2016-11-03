namespace JanHafner.EAVDotNet.Context.InstanceRelation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using JanHafner.EAVDotNet.Model;
    using JanHafner.EAVDotNet.Properties;
    using JetBrains.Annotations;

    /// <summary>
    /// Defines a common entry point for created instances and <see cref="InstanceDescriptor"/>s and their relatoin between the dynamic data model and the user defined data model.
    /// </summary>
    [Export(typeof(IInstanceRelationStore))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal sealed class InstanceRelationStore : IInstanceRelationStore
    {
        [NotNull]
        private readonly ICollection<InstanceDescriptorInstanceRelation> instanceDescriptorInstanceRelations;

        /// <summary>
        /// Creates a new instance of the <see cref="InstanceRelationStore"/> class.
        /// </summary>
        public InstanceRelationStore()
        {
            this.instanceDescriptorInstanceRelations = new List<InstanceDescriptorInstanceRelation>();
        }

        /// <summary>
        /// Tries to get the related instance by supplying the <see cref="InstanceDescriptor"/>.
        /// </summary>
        /// <param name="instanceDescriptor">The <see cref="InstanceDescriptor"/>.</param>
        /// <param name="instance">The related instance.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instanceDescriptor"/>' cannot be null.</exception>
        /// <returns>A value indicating if a relation exists.</returns>
        public Boolean TryGetRelated(InstanceDescriptor instanceDescriptor, out Object instance)
        {
            if (instanceDescriptor == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptor));
            }

            instance = null;
            var relation = this.instanceDescriptorInstanceRelations.FirstOrDefault(idir => idir.InstanceDescriptor == instanceDescriptor);
            if (relation != null)
            {
                instance = relation.Instance;
            }

            return relation != null;
        }

        /// <summary>
        /// Tries to get the related <see cref="InstanceDescriptor"/> by supplying the instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="instanceDescriptor">The related <see cref="InstanceDescriptor"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instance"/>' cannot be null.</exception>
        /// <returns>A value indicating if a relation exists.</returns>
        public Boolean TryGetRelated(Object instance, out InstanceDescriptor instanceDescriptor)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            instanceDescriptor = null;
            var relation = this.instanceDescriptorInstanceRelations.FirstOrDefault(idir => idir.Instance == instance);
            if (relation != null)
            {
                instanceDescriptor = relation.InstanceDescriptor;
            }

            return relation != null;
        }

        private EventHandler<InstanceRelationChangedEventArgs> instanceRelationChanged;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<InstanceRelationChangedEventArgs> InstanceRelationChanged
        {
            add { this.instanceRelationChanged += value; }
            remove { this.instanceRelationChanged -= value; }
        }

        /// <summary>
        /// Raises the InstanceRelationChanged event.
        /// </summary>
        /// <param name="relation">The <see cref="InstanceDescriptorInstanceRelation"/> relation.</param>
        /// <param name="changeType">The type of change occured.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="relation"/>' cannot be null.</exception>
        private void OnInstanceRelationChanged([NotNull] InstanceDescriptorInstanceRelation relation,
            InstanceRelationChangeType changeType)
        {
            if (relation == null)
            {
                throw new ArgumentNullException(nameof(relation));
            }

            var handler = this.instanceRelationChanged;
            handler?.Invoke(this, new InstanceRelationChangedEventArgs(relation.InstanceDescriptor, relation.Instance, changeType));
        }

        /// <summary>
        /// Creates a new relation between the supplied <see cref="InstanceDescriptor"/> and instance.
        /// </summary>
        /// <param name="instanceDescriptor">The <see cref="InstanceDescriptor"/>.</param>
        /// <param name="instance">The instance.</param>
        /// <exception cref="InvalidOperationException">The relation between the supplied <see cref="InstanceDescriptor"/> and instance already exists.</exception>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instanceDescriptor"/>' and '<paramref name="instance"/>' cannot be null.</exception>
        public void CreateRelation(InstanceDescriptor instanceDescriptor, Object instance)
        {
            if (instanceDescriptor == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptor));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (
                this.instanceDescriptorInstanceRelations.Any(
                    idir => idir.InstanceDescriptor == instanceDescriptor && idir.Instance == instance))
            {
                throw new InvalidOperationException(String.Format(ExceptionMessages.RelationAlreadyPresentExceptionMessage, instanceDescriptor.AssemblyQualifiedTypeName, instance.GetType().Name));
            }

            var newInstanceRelation = new InstanceDescriptorInstanceRelation(instanceDescriptor, instance);
            this.instanceDescriptorInstanceRelations.Add(newInstanceRelation);

            this.OnInstanceRelationChanged(newInstanceRelation, InstanceRelationChangeType.Created);
        }

        /// <summary>
        /// Destroys the relation between the supplied <see cref="InstanceDescriptor"/> and the instance associated with.
        /// </summary>
        /// <param name="instanceDescriptor">The <see cref="InstanceDescriptor"/></param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instanceDescriptor"/>' cannot be null.</exception>
        public void DestroyRelation(InstanceDescriptor instanceDescriptor)
        {
            if (instanceDescriptor == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptor));
            }

            var instanceRelation = this.instanceDescriptorInstanceRelations.FirstOrDefault(idir => idir.InstanceDescriptor == instanceDescriptor);
            this.instanceDescriptorInstanceRelations.Remove(instanceRelation);

            this.OnInstanceRelationChanged(instanceRelation, InstanceRelationChangeType.Removed);
        }

        /// <summary>
        /// Destroys the relation between the supplied instance and the <see cref="InstanceDescriptor"/> associated with.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instance"/>' cannot be null.</exception>
        public void DestroyRelation(Object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var instanceRelation = this.instanceDescriptorInstanceRelations.FirstOrDefault(idir => idir.Instance == instance);
            this.instanceDescriptorInstanceRelations.Remove(instanceRelation);

            this.OnInstanceRelationChanged(instanceRelation, InstanceRelationChangeType.Removed);
        }

        /// <summary>
        /// Defines a relation between an <see cref="InstanceDescriptor"/> and an <see cref="Object"/>.
        /// </summary>
        private sealed class InstanceDescriptorInstanceRelation
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InstanceDescriptorInstanceRelation"/> class.
            /// </summary>
            /// <param name="instanceDescriptor">The related <see cref="InstanceDescriptor"/>.</param>
            /// <param name="instance">The related <see cref="Object"/>.</param>
            /// <exception cref="ArgumentNullException">The value of '<paramref name="instanceDescriptor"/>' and '<paramref name="instance"/>' cannot be null.</exception>
            public InstanceDescriptorInstanceRelation([NotNull] InstanceDescriptor instanceDescriptor, [NotNull] Object instance)
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
            }

            /// <summary>
            /// Gets the <see cref="InstanceDescriptor"/> part of the relation.
            /// </summary>
            [NotNull]
            public InstanceDescriptor InstanceDescriptor { get; private set; }

            /// <summary>
            /// Gets the <see cref="Object"/> part of the relation.
            /// </summary>
            [NotNull]
            public Object Instance { get; private set; }
        }
    }
}
