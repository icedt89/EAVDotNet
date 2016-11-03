namespace JanHafner.EAVDotNet.Instrumentation.TypeSystem
{
    using System;
    using JanHafner.EAVDotNet.Model;

    /// <summary>
    /// Defines methods for creating <see cref="TypeAlias"/> instances for <see cref="InstanceDescriptor"/>s.
    /// </summary>
    internal interface ITypeAliasWalker
    {
        /// <summary>
        /// Creates <see cref="TypeAlias"/> instances for the supplied <see cref="InstanceDescriptor"/>.
        /// </summary>
        /// <param name="instanceDescriptor">The <see cref="InstanceDescriptor"/>.</param>
        /// <param name="type">The <see cref="Type"/>.</param>
        void CreateAliases(InstanceDescriptor instanceDescriptor, Type type);
    }
}