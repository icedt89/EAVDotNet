namespace JanHafner.EAVDotNet.Model.Collections
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Describes the <see cref="CollectionItem"/> as <see cref="Boolean"/>.
    /// </summary>
    //[Table("BooleanCollectionItem", Schema = "CollectionTypeSystem")]
    public class BooleanCollectionItem : CollectionItem, IPrimitiveValueAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanCollectionItem"/> class.
        /// </summary>
        protected BooleanCollectionItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanCollectionItem"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal BooleanCollectionItem(Boolean value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Boolean"/> value of the <see cref="CollectionItem"/>.
        /// </summary>
        [Column("BooleanValue")]
        public Boolean Value { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        Object IValueAssociation.Value
        {
            get { return this.Value; }
            set { this.Value = (Boolean)value; }
        }
    }
}