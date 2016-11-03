namespace JanHafner.EAVDotNet.Model
{
    using System;
    using JetBrains.Annotations;

    /// <summary>
    /// Provides a generic way of accessing a value.
    /// </summary>
    public interface IValueAssociation
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [CanBeNull]
        Object Value { get; set; }
    }
}