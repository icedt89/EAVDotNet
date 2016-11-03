namespace JanHafner.EAVDotNet.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;
    using JetBrains.Annotations;

    /// <summary>
    /// Represents a persisted instance.
    /// An instance is describes by a associated type and property values.
    /// </summary>
    [DebuggerDisplay("AssociatedType = {AssemblyQualifiedTypeName}; Values = {PropertyValues.Count}")]
    [Table("InstanceDescriptors", Schema = "DynamicTypeSystem")]
    public class InstanceDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceDescriptor"/> class.
        /// </summary>
        protected InstanceDescriptor()
        {
            this.Id = Guid.NewGuid();

            this.PropertyValues = new List<PropertyValue>();
            this.Aliases = new List<TypeAlias>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceDescriptor"/> class.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> which is associated with this <see cref="InstanceDescriptor"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="type"/>' cannot be null.</exception>
        public InstanceDescriptor([NotNull] Type type)
            : this()
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            this.AssemblyQualifiedTypeName = type.AssemblyQualifiedName;
        }

        /// <summary>
        /// Gets the id of the <see cref="InstanceDescriptor"/>.
        /// </summary>
        [Key]
        public Guid Id { get; internal set; }

        /// <summary>
        /// Gets the associated <see cref="Type"/> name of this <see cref="InstanceDescriptor"/>.
        /// </summary>
        [Required]
        [NotNull]
        [Index("IX_AssemblyQualifiedTypeName")]
        public string AssemblyQualifiedTypeName { get; private set; }

        /// <summary>
        /// Gets the list of <see cref="TypeAlias"/> instances.
        /// </summary>
        [NotNull]
        public virtual ICollection<TypeAlias> Aliases { get; private set; }

        /// <summary>
        /// Gets the list of associated <see cref="PropertyValue"/> instances.
        /// </summary>
        [NotNull]
        public virtual ICollection<PropertyValue> PropertyValues { get; private set; }
    }
}