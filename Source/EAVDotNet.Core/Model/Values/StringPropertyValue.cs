namespace JanHafner.EAVDotNet.Model.Values
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;

    /// <summary>
    /// Describes a <see cref="String"/> property value.
    /// </summary>
    //[Table("StringPropertyValues", Schema = "PropertyTypeSystem")]
    public class StringPropertyValue : PropertyValue, IPrimitiveValueAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringPropertyValue"/> class.
        /// </summary>
        protected StringPropertyValue()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringPropertyValue"/> class.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/></param>
        /// <param name="value">The value.</param>
        internal StringPropertyValue(String value, PropertyInfo propertyInfo)
            : base(propertyInfo)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));    
            }

            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="String"/> value of the property value.
        /// </summary>
        [Required]
        [Column("StringValue")]
        public String Value { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        Object IValueAssociation.Value
        {
            get { return this.Value; }
            set { this.Value = (String)value; }
        }
    }
}