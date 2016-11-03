namespace JanHafner.EAVDotNet.Model.Values
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;
    using JetBrains.Annotations;

    /// <summary>
    /// Describes a <see cref="InstanceDescriptor"/> property value.
    /// </summary>
    [Table("InstancePropertyAssociations", Schema = "PropertyTypeSystem")]
    public class  InstancePropertyAssociation : PropertyValue, IInstanceValueAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstancePropertyAssociation"/> class.
        /// </summary>
        protected InstancePropertyAssociation()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstancePropertyAssociation"/> class.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/></param>
        /// <param name="instanceDescriptor">The <see cref="InstanceDescriptor"/>.</param>
        internal InstancePropertyAssociation([NotNull] InstanceDescriptor instanceDescriptor, PropertyInfo propertyInfo)
            : base(propertyInfo)
        {
            if (instanceDescriptor == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptor));
            }

            this.Instance = instanceDescriptor;
        }

        /// <summary>
        /// Gets or sets the <see cref="InstanceDescriptor"/> value of the property value.
        /// </summary>
        [Required]
        public virtual InstanceDescriptor Instance { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        Object IValueAssociation.Value
        {
            get { return this.Instance; }
            set { this.Instance = (InstanceDescriptor)value; }
        }
    }
}