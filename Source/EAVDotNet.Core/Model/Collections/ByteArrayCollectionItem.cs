namespace JanHafner.EAVDotNet.Model.Collections
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Describes the <see cref="CollectionItem"/> as <see cref="Byte[]"/>.
    /// </summary>
    //[Table("ByteArrayCollectionItems", Schema = "CollectionTypeSystem")]
    public class ByteArrayCollectionItem : CollectionItem, IPrimitiveValueAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ByteArrayCollectionItem"/> class.
        /// </summary>
        protected ByteArrayCollectionItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteArrayCollectionItem"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal ByteArrayCollectionItem(Byte[] value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Byte[]"/> value of the <see cref="CollectionItem"/>.
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