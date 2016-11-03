namespace JanHafner.EAVDotNet.Model.Collections
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Describes the <see cref="CollectionItem"/> as <see cref="Byte"/>.
    /// </summary>
    //[Table("ByteCollectionItems", Schema = "CollectionTypeSystem")]
    public class ByteCollectionItem : CollectionItem, IPrimitiveValueAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ByteCollectionItem"/> class.
        /// </summary>
        protected ByteCollectionItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteCollectionItem"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal ByteCollectionItem(Byte value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Byte"/> value of the <see cref="CollectionItem"/>.
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