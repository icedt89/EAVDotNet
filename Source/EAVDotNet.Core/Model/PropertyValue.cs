namespace JanHafner.EAVDotNet.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;
    using System.Reflection;
    using JetBrains.Annotations;

    /// <summary>
    /// Defines a base class for all property values.
    /// The concrete type and value of the property value must be specified by inheritance to make sure the correct Entity Framework type is used.
    /// </summary>
    [DebuggerDisplay("Concrete Type = {this.GetType().Name}; AssociatedInstance = {AssociatedInstance.AssemblyQualifiedTypeName}")]
    [Table("PropertyValues", Schema = "PropertyTypeSystem")]
    public abstract class PropertyValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValue"/> class.
        /// </summary>
        protected PropertyValue()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="propertyInfo"/> class.
        /// </summary>
        /// <param name="propertyInfo">The type for which this <see cref="propertyInfo"/> is created.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyInfo" />' cannot be null.</exception>
        protected internal PropertyValue([NotNull] PropertyInfo propertyInfo)
            : this()
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            this.Name = propertyInfo.Name;
            this.AssemblyQualifiedPropertyTypeName = propertyInfo.PropertyType.AssemblyQualifiedName;
        }

        /// <summary>
        /// Gets the id of the <see cref="PropertyValue"/>.
        /// </summary>
        [Key]
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="InstanceDescriptor"/> this <see cref="PropertyValue"/> is assigned to.
        /// </summary>
        [Required]
        [CanBeNull]
        public virtual InstanceDescriptor AssociatedInstance { get; set; }

        internal Guid AssociatedInstanceId { get; set; }

        /// <summary>
        /// Gets or sets the name of the <see cref="PropertyInfo"/>, which is mostly the name of the property.
        /// Index with name IX_Name will be created.
        /// </summary>
        [Required]
        [Index("IX_Name")]
        [NotNull]
        public String Name { get; private set; }

        /// <summary>
        /// Gets or sets the AssemblyQualifiedPropertyTypeName of the <see cref="PropertyInfo"/>.
        /// Index with name IX_AssemblyQualifiedPropertyTypeName will be created.
        /// </summary>
        [Required]
        [Index("IX_AssemblyQualifiedPropertyTypeName")]
        [NotNull]
        public String AssemblyQualifiedPropertyTypeName { get; private set; }
    }
}