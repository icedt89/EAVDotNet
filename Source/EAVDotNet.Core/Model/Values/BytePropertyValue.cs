namespace JanHafner.EAVDotNet.Model.Values
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;

    /// <summary>
    /// Describes a <see cref="Byte"/> property value.
    /// </summary>
    //[Table("BytePropertyValues", Schema = "PropertyTypeSystem")]
    public class BytePropertyValue : PropertyValue, IPrimitiveValueAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BytePropertyValue"/> class.
        /// </summary>
        protected BytePropertyValue()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BytePropertyValue"/> class.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/></param>
        /// <param name="value">The value.</param>
        internal BytePropertyValue(Byte value, PropertyInfo propertyInfo)
            : base(propertyInfo)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Byte"/> value of the property value.
        /// </summary>
        [Column("ByteValue")]
        public Byte Value { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        Object IValueAssociation.Value
        {
            get { return this.Value; }
            set { this.Value = (Byte)value; }
        }
    }
}