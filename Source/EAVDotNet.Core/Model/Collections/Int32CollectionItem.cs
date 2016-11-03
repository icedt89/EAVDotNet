namespace JanHafner.EAVDotNet.Model.Collections
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Describes the <see cref="CollectionItem"/> as <see cref="Int32"/>.
    /// </summary>
    //[Table("Int32CollectionItems", Schema = "CollectionTypeSystem")]
    public class Int32CollectionItem : CollectionItem, IPrimitiveValueAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Int32CollectionItem"/> class.
        /// </summary>
        protected Int32CollectionItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Int32CollectionItem"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal Int32CollectionItem(Int32 value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Int32"/> value of the <see cref="CollectionItem"/>.
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