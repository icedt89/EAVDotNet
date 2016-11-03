namespace JanHafner.EAVDotNet.Model.Values
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;

    /// <summary>
    /// Describes a <see cref="Int32"/> property value.
    /// </summary>
    //[Table("Int32PropertyValues", Schema = "PropertyTypeSystem")]
    public class Int32PropertyValue : PropertyValue, IPrimitiveValueAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Int32PropertyValue"/> class.
        /// </summary>
        protected Int32PropertyValue()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Int32PropertyValue"/> class.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/></param>
        /// <param name="value">The value.</param>
        internal Int32PropertyValue(Int32 value, PropertyInfo propertyInfo)
            : base(propertyInfo)
        {
            this.Value = value;
        }
        
        /// <summary>
        /// Gets or sets the <see cref="Int32"/> value of the property value.
        /// </summary>
        [Column("Int32Value")]
        public Int32 Value { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        Object IValueAssociation.Value
        {
            get { return this.Value; }
            set { this.Value = (Int32)value; }
        }
    }
}