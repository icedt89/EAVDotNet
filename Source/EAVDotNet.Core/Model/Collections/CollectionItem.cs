namespace JanHafner.EAVDotNet.Model.Collections
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using JanHafner.EAVDotNet.Model.Values;

    /// <summary>
    /// Acts as the base class for all collection items.
    /// </summary>
    [Table("CollectionItems", Schema = "CollectionTypeSystem")]
    public abstract class CollectionItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionItem"/> class.
        /// </summary>
        protected CollectionItem()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// Gets the id of the <see cref="CollectionItem"/>.
        /// </summary>
        [Key]
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the collection this <see cref="BooleanCollectionItem"/> is contained in.
        /// </summary>
        public virtual CollectionPropertyAssociation CollectionAssociation { get; private set; }
    }
}
