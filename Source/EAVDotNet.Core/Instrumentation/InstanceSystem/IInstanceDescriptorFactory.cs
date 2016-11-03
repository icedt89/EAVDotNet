namespace JanHafner.EAVDotNet.Instrumentation.InstanceSystem
{
    using JanHafner.EAVDotNet.Model;
    using JetBrains.Annotations;

    /// <summary>
    /// The implementation is responsible for the creation of <see cref="InstanceDescriptor"/> instances.
    /// </summary>
    public interface IInstanceDescriptorFactory
    {
        /// <summary>
        /// Creates new instances of <see cref="InstanceDescriptor"/> instances based on the information contained by the supplied <see cref="InstanceDescriptorCreationContext"/>.
        /// </summary>
        /// <param name="instanceDescriptorCreationContext">The <see cref="InstanceDescriptorCreationContext"/>.</param>
        /// <returns>The newly created <see cref="InstanceDescriptor"/>.</returns>
        [NotNull]
        InstanceDescriptor CreateInstanceDescriptor([NotNull] InstanceDescriptorCreationContext instanceDescriptorCreationContext);
    }
}