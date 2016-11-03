namespace JanHafner.EAVDotNet.Model.Values
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;

    /// <summary>
    /// Describes a <see cref="Guid"/> property value.
    /// </summary>
    //[Table("GuidPropertyValues", Schema = "PropertyTypeSystem")]
    public class GuidPropertyValue : PropertyValue, IPrimitiveValueAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuidPropertyValue"/> class.
        /// </summary>
        protected GuidPropertyValue()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GuidPropertyValue"/> class.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/></param>
        /// <param name="value">The value.</param>
        internal GuidPropertyValue(Guid value, PropertyInfo propertyInfo)
            : base(propertyInfo)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Guid"/> value of the property value.
        /// </summary>
        [Column("GuidValue")]
        public Guid Value { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        Object IValueAssociation.Value
        {
            get { return this.Value; }
            set { this.Value = (Guid)value; }
        }
    }
}