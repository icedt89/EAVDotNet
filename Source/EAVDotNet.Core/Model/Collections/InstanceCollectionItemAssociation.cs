namespace JanHafner.EAVDotNet.Model.Collections
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using JetBrains.Annotations;

    /// <summary>
    /// Describes the <see cref="CollectionItem"/> as <see cref="InstanceDescriptor"/>.
    /// </summary>
    //[Table("InstanceCollectionItemAssociations", Schema = "CollectionTypeSystem")]
    public class InstanceCollectionItemAssociation : CollectionItem, IInstanceValueAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceCollectionItemAssociation"/> class.
        /// </summary>
        protected InstanceCollectionItemAssociation()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceCollectionItemAssociation"/> class.
        /// </summary>
        /// <param name="instanceDescriptor">The <see cref="InstanceDescriptor"/>.</param>
        internal InstanceCollectionItemAssociation([NotNull] InstanceDescriptor instanceDescriptor)
        {
            if (instanceDescriptor == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptor));
            }

            this.Instance = instanceDescriptor;
        }

        /// <summary>
        /// Gets or sets the <see cref="InstanceDescriptor"/> value of the <see cref="CollectionItem"/>.
        /// </summary>
        [Required]
        public virtual InstanceDescriptor Instance { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public Object Value
        {
            get { return this.Instance; }
            set { this.Instance = (InstanceDescriptor)value; }
        }
    }
}
