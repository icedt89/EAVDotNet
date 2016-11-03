namespace JanHafner.EAVDotNet.Query.ResultHandling
{
    using System;
    using JanHafner.EAVDotNet.Context.InstanceRelation;
    using JetBrains.Annotations;

    public sealed class ResultHandlerContext
    {
        public ResultHandlerContext([CanBeNull] Object result, [NotNull] IInstanceRelationStore instanceRelationStore)
            : this(result, instanceRelationStore, false, false)
        {
        }

        public ResultHandlerContext([CanBeNull] Object result, [NotNull] IInstanceRelationStore instanceRelationStore, Boolean noChangeTracking, Boolean noLazyLoading)
        {
            if (instanceRelationStore == null)
            {
                throw new ArgumentNullException(nameof(instanceRelationStore));
            }

            this.Result = result;
            this.InstanceRelationStore = instanceRelationStore;
            this.NoChangeTracking = noChangeTracking;
            this.NoLazyLoading = noLazyLoading;
        }

        public Object Result { get; private set; }

        public IInstanceRelationStore InstanceRelationStore { get; private set; }

        public Boolean NoChangeTracking { get; private set; }

        public Boolean NoLazyLoading { get; private set; }
    }
}
