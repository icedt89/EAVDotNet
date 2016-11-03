namespace JanHafner.EAVDotNet.Model
{
    /// <summary>
    /// Marks implementors or <see cref="IValueAssociation"/> as container of a primitive value so special handling can be applied.
    /// </summary>
    internal interface IPrimitiveValueAssociation : IValueAssociation
    {
    }
}