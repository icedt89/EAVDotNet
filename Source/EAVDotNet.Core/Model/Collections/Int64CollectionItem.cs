namespace JanHafner.EAVDotNet.Model.Collections
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Describes the <see cref="CollectionItem"/> as <see cref="Int64"/>.
    /// </summary>
    //[Table("Int64CollectionItems", Schema = "CollectionTypeSystem")]
    public class Int64CollectionItem : CollectionItem, IPrimitiveValueAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Int64CollectionItem"/> class.
        /// </summary>
        protected Int64CollectionItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Int64CollectionItem"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal Int64CollectionItem(Int64 value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Int64"/> value of the <see cref="CollectionItem"/>.
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