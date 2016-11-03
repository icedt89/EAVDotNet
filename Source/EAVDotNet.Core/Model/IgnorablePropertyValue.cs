namespace JanHafner.EAVDotNet.Model
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Indicates that the property value should not be persisted.
    /// </summary>
    [NotMapped]
    internal sealed class IgnorablePropertyValue : PropertyValue
    {
    }
}
