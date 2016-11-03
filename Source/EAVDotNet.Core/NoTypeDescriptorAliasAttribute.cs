namespace JanHafner.EAVDotNet
{
    using System;
    using JanHafner.EAVDotNet.Model;

    /// <summary>
    /// Indicates that the decorated interface or inherited class should never be treated as a <see cref="TypeAlias"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public sealed class NoTypeDescriptorAliasAttribute : Attribute
    {
    }
}
