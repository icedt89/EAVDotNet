namespace JanHafner.EAVDotNet.Model.Collections
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Describes the <see cref="CollectionItem"/> as <see cref="String"/>.
    /// </summary>
    //[Table("StringCollectionItems", Schema = "CollectionTypeSystem")]
    public class StringCollectionItem : CollectionItem, IPrimitiveValueAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringCollectionItem"/> class.
        /// </summary>
        protected StringCollectionItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringCollectionItem"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal StringCollectionItem(String value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="String"/> value of the <see cref="CollectionItem"/>.
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