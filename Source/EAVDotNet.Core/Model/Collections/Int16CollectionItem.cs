namespace JanHafner.EAVDotNet.Model.Collections
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Describes the <see cref="CollectionItem"/> as <see cref="Int16"/>.
    /// </summary>
    //[Table("Int16CollectionItems", Schema = "CollectionTypeSystem")]
    public class Int16CollectionItem : CollectionItem, IPrimitiveValueAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Int16CollectionItem"/> class.
        /// </summary>
        protected Int16CollectionItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Int16CollectionItem"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal Int16CollectionItem(Int16 value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Int16"/> value of the <see cref="CollectionItem"/>.
        /// </summary>
        [Column("Int16Value")]
        public Int16 Value { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        Object IValueAssociation.Value
        {
            get { return this.Value; }
            set { this.Value = (Int16)value; }
        }
    }
}