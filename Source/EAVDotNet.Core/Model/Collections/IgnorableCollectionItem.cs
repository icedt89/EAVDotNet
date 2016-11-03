namespace JanHafner.EAVDotNet.Model.Collections
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Indicates that the collection item should not be persisted.
    /// </summary>
    [NotMapped]
    internal sealed class IgnorableCollectionItem : CollectionItem
    {
    }
}