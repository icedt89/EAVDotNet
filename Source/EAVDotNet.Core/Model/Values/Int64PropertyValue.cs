namespace JanHafner.EAVDotNet.Model.Values
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;

    /// <summary>
    /// Describes a <see cref="Int64"/> property value.
    /// </summary>
    //[Table("Int64PropertyValues", Schema = "PropertyTypeSystem")]
    public class Int64PropertyValue : PropertyValue, IPrimitiveValueAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Int64PropertyValue"/> class.
        /// </summary>
        protected Int64PropertyValue()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Int64PropertyValue"/> class.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/></param>
        /// <param name="value">The value.</param>
        internal Int64PropertyValue(Int64 value, PropertyInfo propertyInfo)
            : base(propertyInfo)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Int64"/> value of the property value.
        /// </summary>
        [Column("Int64Value")]
        public Int64 Value { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        Object IValueAssociation.Value
        {
            get { return this.Value; }
            set { this.Value = (Int64)value; }
        }
    }
}