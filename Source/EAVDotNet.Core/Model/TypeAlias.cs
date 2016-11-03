namespace JanHafner.EAVDotNet.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;
    using JetBrains.Annotations;

    /// <summary>
    /// Describes a simple alias for a <see cref="InstanceDescriptor"/>. An alias can be anything but is primarily used to describe which interfaces are implemented or which base types are in the inheritance hierarchy.
    /// </summary>
    [DebuggerDisplay("AssemblyQualifiedTypeName = {AssemblyQualifiedTypeName}; IsInterface = {IsInterface}")]
    [Table("TypeAliases", Schema = "DynamicTypeSystem")]
    public sealed class TypeAlias
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeAlias"/> class.
        /// </summary>
        private TypeAlias()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeAlias"/> class.
        /// </summary>
        /// <param name="type">The type for which this <see cref="TypeAlias"/> is created.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="type"/>' cannot be null.</exception>
        internal TypeAlias([NotNull] Type type)
            : this()
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            this.AssemblyQualifiedTypeName = type.AssemblyQualifiedName;
            this.IsInterface = type.IsInterface;
        }

        /// <summary>
        /// Gets the id of the <see cref="TypeAlias"/>.
        /// </summary>
        [Key]
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets or sets the AssemblyQualifiedTypeName of the <see cref="TypeAlias"/>.
        /// Index with name IX_AssemblyQualifiedTypeName will be created.
        /// </summary>
        [Required]
        [NotNull]
        [Index("IX_AssemblyQualifiedTypeName")]
        public String AssemblyQualifiedTypeName { get; private set; }

        /// <summary>
        /// Gets a value indicating, whether, this <see cref="TypeAlias"/> is an interface, <c>false</c> it is a base type.
        /// </summary>
        public Boolean IsInterface { get; private set; }
    }
}
