namespace JanHafner.EAVDotNet.Instanciation.TypeResolution
{
    using System;
    using JanHafner.EAVDotNet.Context.InstanceRelation;
    using JetBrains.Annotations;

    /// <summary>
    /// Defines a common interface for information regarding the resolution of a <see cref="Type"/>.
    /// </summary>
    public interface ITypeInstanciationContext
    {
        /// <summary>
        /// Gets the <see cref="Type"/> that is requested.
        /// </summary>
        [NotNull]
        Type RequestedType { get; }

        /// <summary>
        /// Gets the <see cref="IInstanceRelationStore"/>.
        /// </summary>
        [NotNull]
        IInstanceRelationStore InstanceRelationStore { get; }
    }
}