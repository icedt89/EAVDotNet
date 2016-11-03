namespace JanHafner.EAVDotNet.Model
{
    using System;
    using JetBrains.Annotations;

    /// <summary>
    /// Indicates that the implementing <see cref="Type"/> provides an <see cref="InstanceDescriptor"/>.
    /// </summary>
    internal interface IInstanceValueAssociation : IValueAssociation
    { 
        /// <summary>
        /// Gets or sets the <see cref="InstanceDescriptor"/>
        /// </summary>
        [CanBeNull]
        InstanceDescriptor Instance { get; set; }
    }
}