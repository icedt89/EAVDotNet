namespace JanHafner.EAVDotNet.Model.Values
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;

    /// <summary>
    /// Describes a <see cref="DateTime"/> property value.
    /// </summary>
    //[Table("DateTimePropertyValues", Schema = "PropertyTypeSystem")]
    public class DateTimePropertyValue : PropertyValue, IPrimitiveValueAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimePropertyValue"/> class.
        /// </summary>
        protected DateTimePropertyValue()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimePropertyValue"/> class.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/></param>
        /// <param name="value">The value.</param>
        internal DateTimePropertyValue(DateTime value, PropertyInfo propertyInfo)
            : base(propertyInfo)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> value of the property value.
        /// </summary>
        [Column("DateTimeValue")]
        public DateTime Value { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        Object IValueAssociation.Value
        {
            get { return this.Value; }
            set { this.Value = (DateTime)value; }
        }
    }
}