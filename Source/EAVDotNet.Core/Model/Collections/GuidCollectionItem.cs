namespace JanHafner.EAVDotNet.Model.Collections
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Describes the <see cref="CollectionItem"/> as <see cref="Guid"/>.
    /// </summary>
    //[Table("GuidCollectionItems", Schema = "CollectionTypeSystem")]
    public class GuidCollectionItem : CollectionItem, IPrimitiveValueAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuidCollectionItem"/> class.
        /// </summary>
        protected GuidCollectionItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GuidCollectionItem"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal GuidCollectionItem(Guid value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Guid"/> value of the <see cref="CollectionItem"/>.
        /// </summary>
        [Column("GuidValue")]
        public Guid Value { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        Object IValueAssociation.Value
        {
            get { return this.Value; }
            set { this.Value = (Guid)value; }
        }
    }
}