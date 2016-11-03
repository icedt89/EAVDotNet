namespace JanHafner.EAVDotNet.Instanciation.TypeResolution
{
    using System;
    using JanHafner.EAVDotNet.Context.InstanceRelation;
    using JanHafner.EAVDotNet.Model;
    using JanHafner.EAVDotNet.Model.Values;
    using JetBrains.Annotations;

    /// <summary>
    /// Holds information which encapsulates the parameters for resolving and proxying a collection.
    /// </summary>
    internal sealed class CollectionProxyingInstanciationContext : ITypeInstanciationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceProxyingInstanciationContext"/> class.
        /// </summary>
        /// <param name="instanceDescriptor">The <see cref="InstanceDescriptor"/>.</param>
        /// <param name="collectionPropertyAssociation">The <see cref="CollectionPropertyAssociation"/>.</param>
        /// <param name="instanceRelationStore">The <see cref="IInstanceRelationStore"/>.</param>
        /// <param name="requestedType">The <see cref="Type"/> which is requested.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="requestedType"/>', '<paramref name="collectionPropertyAssociation"/>', '<paramref name="instanceDescriptor"/>' and '<paramref name="instanceRelationStore"/>' cannot be null.</exception>
        public CollectionProxyingInstanciationContext([NotNull] Type requestedType,
            [NotNull] IInstanceRelationStore instanceRelationStore, [NotNull] InstanceDescriptor instanceDescriptor,
            [NotNull] CollectionPropertyAssociation collectionPropertyAssociation)
        {
            if (instanceDescriptor == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptor));
            }

            if (collectionPropertyAssociation == null)
            {
                throw new ArgumentNullException(nameof(collectionPropertyAssociation));
            }

            if (requestedType == null)
            {
                throw new ArgumentNullException(nameof(requestedType));
            }

            if (instanceRelationStore == null)
            {
                throw new ArgumentNullException(nameof(instanceRelationStore));
            }

            this.InstanceDescriptor = instanceDescriptor;
            this.CollectionPropertyAssociation = collectionPropertyAssociation;
            this.RequestedType = requestedType;
            this.InstanceRelationStore = instanceRelationStore;
        }

        /// <summary>
        /// Gets the <see cref="InstanceDescriptor"/>.
        /// </summary>
        public InstanceDescriptor InstanceDescriptor { get; private set; }

        /// <summary>
        /// Gets the <see cref="Type"/> that is requested.
        /// </summary>
        public Type RequestedType { get; private set; }

        /// <summary>
        /// Gets the <see cref="IInstanceRelationStore"/>.
        /// </summary>
        public IInstanceRelationStore InstanceRelationStore { get; private set; }

        /// <summary>
        /// Gets a value indicating, whether, change tracking proxies should be generated.
        /// </summary>
        public Boolean NoChangeTracking { get; set; }

        /// <summary>
        /// Gets the <see cref="CollectionPropertyAssociation"/>.
        /// </summary>
        public CollectionPropertyAssociation CollectionPropertyAssociation { get; private set; }
    }
}