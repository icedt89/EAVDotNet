namespace JanHafner.EAVDotNet.Context.InstanceRelation
{
    /// <summary>
    /// Defines the type of change a relation is subject.
    /// </summary>
    public enum InstanceRelationChangeType
    {
        /// <summary>
        /// The relation was created.
        /// </summary>
        Created = 0,

        /// <summary>
        /// The relation was removed.
        /// </summary>
        Removed = 1,

        /// <summary>
        /// The relation is changed, that means one end of the relation ends is changed.
        /// </summary>
        Reconnected = 2
    }
}
