namespace JanHafner.EAVDotNet.Model.Values
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;

    /// <summary>
    /// Describes a <see cref="Int16"/> property value.
    /// </summary>
    //[Table("Int16PropertyValues", Schema = "PropertyTypeSystem")]
    public class Int16PropertyValue : PropertyValue, IPrimitiveValueAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Int16PropertyValue"/> class.
        /// </summary>
        protected Int16PropertyValue()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Int16PropertyValue"/> class.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/></param>
        /// <param name="value">The value.</param>
        internal Int16PropertyValue(Int16 value, PropertyInfo propertyInfo)
            : base(propertyInfo)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Int16"/> value of the property value.
        /// </summary>
        [Column("Int16Value")]
        public Int16 Value { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        Object IValueAssociation.Value
        {
            get { return this.Value; }
            set { this.Value = (Int16)value; }
        }
    }
}