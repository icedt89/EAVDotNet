namespace JanHafner.EAVDotNet.Model.Values
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;

    /// <summary>
    /// Describes a <see cref="Byte"/> property value.
    /// </summary>
    //[Table("BytePArrayropertyValues", Schema = "PropertyTypeSystem")]
    public class ByteArrayPropertyValue : PropertyValue, IPrimitiveValueAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ByteArrayPropertyValue"/> class.
        /// </summary>
        protected ByteArrayPropertyValue()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteArrayPropertyValue"/> class.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/></param>
        /// <param name="value">The value.</param>
        internal ByteArrayPropertyValue(Byte[] value, PropertyInfo propertyInfo)
            : base(propertyInfo)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Byte[]"/> value of the property value.
        /// </summary>
        [Column("BlobValue")]
        public Byte[] Value { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        Object IValueAssociation.Value
        {
            get { return this.Value; }
            set { this.Value = (Byte[])value; }
        }
    }
}