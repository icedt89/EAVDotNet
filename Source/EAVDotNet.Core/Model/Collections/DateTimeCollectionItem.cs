namespace JanHafner.EAVDotNet.Model.Collections
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Describes the <see cref="CollectionItem"/> as <see cref="DateTime"/>.
    /// </summary>
    //[Table("DateTimeCollectionItems", Schema = "CollectionTypeSystem")]
    public class DateTimeCollectionItem : CollectionItem, IPrimitiveValueAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeCollectionItem"/> class.
        /// </summary>
        protected DateTimeCollectionItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeCollectionItem"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal DateTimeCollectionItem(DateTime value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> value of the <see cref="CollectionItem"/>.
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