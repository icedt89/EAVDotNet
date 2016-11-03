namespace JanHafner.EAVDotNet.Instanciation.TypeResolution
{
    using System;
    using JanHafner.EAVDotNet.Context.InstanceRelation;
    using JetBrains.Annotations;

    /// <summary>
    /// Defines the default resolution context which lacks information for proxying.
    /// </summary>
    internal sealed class DefaultTypeInstanciationContext : ITypeInstanciationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTypeInstanciationContext"/> class.
        /// </summary>
        /// <param name="requestedType">The <see cref="Type"/> that is requested.</param>
        /// <param name="instanceRelationStore">The <see cref="IInstanceRelationStore"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="requestedType"/>' and '<paramref name="instanceRelationStore"/>' cannot be null.</exception>
        public DefaultTypeInstanciationContext([NotNull] Type requestedType,
            [NotNull] IInstanceRelationStore instanceRelationStore)
        {
            if (requestedType == null)
            {
                throw new ArgumentNullException(nameof(requestedType));
            }

            if (requestedType == null)
            {
                throw new ArgumentNullException(nameof(requestedType));
            }

            this.RequestedType = requestedType;
            this.InstanceRelationStore = instanceRelationStore;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> that is requested.
        /// </summary>
        public Type RequestedType { get; private set; }

        /// <summary>
        /// Gets the <see cref="IInstanceRelationStore"/>.
        /// </summary>
        public IInstanceRelationStore InstanceRelationStore { get; private set; }
    }
}