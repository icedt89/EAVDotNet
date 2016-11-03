namespace JanHafner.EAVDotNet.Context.InstanceRelation
{
    using System;
    using JanHafner.EAVDotNet.Model;
    using JetBrains.Annotations;

    /// <summary>
    /// Defines a common entry point for created instances and <see cref="InstanceDescriptor"/>s and their relation between the dynamic data model and the user defined data model.
    /// </summary>
    public interface IInstanceRelationStore
    {
        /// <summary>
        /// Creates a new relation between the supplied <see cref="InstanceDescriptor"/> and instance.
        /// </summary>
        /// <param name="instanceDescriptor">The <see cref="InstanceDescriptor"/>.</param>
        /// <param name="instance">The instance.</param>
        void CreateRelation([NotNull] InstanceDescriptor instanceDescriptor, [NotNull] Object instance);

        /// <summary>
        /// Tries to get the related instance by supplying the <see cref="InstanceDescriptor"/>.
        /// </summary>
        /// <param name="instanceDescriptor">The <see cref="InstanceDescriptor"/>.</param>
        /// <param name="instance">The related instance.</param>
        /// <returns>A value indicating if a relation exists.</returns>
        Boolean TryGetRelated([NotNull] InstanceDescriptor instanceDescriptor, [CanBeNull] out Object instance);

        /// <summary>
        /// Tries to get the related <see cref="InstanceDescriptor"/> by supplying the instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="instanceDescriptor">The related <see cref="InstanceDescriptor"/>.</param>
        /// <returns>A value indicating if a relation exists.</returns>
        Boolean TryGetRelated([NotNull] Object instance, [CanBeNull] out InstanceDescriptor instanceDescriptor);

        /// <summary>
        /// Destroys the relation between the supplied <see cref="InstanceDescriptor"/> and the instance associated with.
        /// </summary>
        /// <param name="instanceDescriptor">The <see cref="InstanceDescriptor"/></param>
        void DestroyRelation([NotNull] InstanceDescriptor instanceDescriptor);

        /// <summary>
        /// Destroys the relation between the supplied instance and the <see cref="InstanceDescriptor"/> associated with.
        /// </summary>
        /// <param name="instance">The instance.</param>
        void DestroyRelation([NotNull] Object instance);

        /// <summary>
        /// Is fired if the relation is changed.
        /// </summary>
        event EventHandler<InstanceRelationChangedEventArgs> InstanceRelationChanged;
    }
}