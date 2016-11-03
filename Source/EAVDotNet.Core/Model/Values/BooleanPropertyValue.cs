namespace JanHafner.EAVDotNet.Model.Values
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;
    using JetBrains.Annotations;

    /// <summary>
    /// Describes a <see cref="Boolean"/> property value.
    /// </summary>
    //[Table("BooleanPropertyValues", Schema = "PropertyTypeSystem")]
    public class BooleanPropertyValue : PropertyValue, IPrimitiveValueAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanPropertyValue"/> class.
        /// </summary>
        protected BooleanPropertyValue()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanPropertyValue"/> class.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/></param>
        /// <param name="value">The value.</param>
        internal BooleanPropertyValue(Boolean value, PropertyInfo propertyInfo)
            : base(propertyInfo)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Boolean"/> value of the property value.
        /// </summary>
        [Column("BooleanValue")]
        public Boolean Value { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        [NotNull]
        Object IValueAssociation.Value
        {
            get { return this.Value; }
            set { this.Value = (Boolean)value; }
        }
    }
}