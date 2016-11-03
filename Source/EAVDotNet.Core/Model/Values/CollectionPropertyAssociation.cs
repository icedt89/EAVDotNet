namespace JanHafner.EAVDotNet.Model.Values
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;
    using JanHafner.EAVDotNet.Model.Collections;
    using JetBrains.Annotations;

    /// <summary>
    /// Describes a property value association to an <see cref="ICollection&lt;T&gt;"/>"/>.
    /// </summary>
    //[Table("CollectionPropertyAssociations", Schema = "PropertyTypeSystem")]
    public class CollectionPropertyAssociation : PropertyValue, IValueAssociation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionPropertyAssociation"/> class.
        /// </summary>
        protected CollectionPropertyAssociation()
        {
            this.CollectionItemDescriptors = new List<CollectionItem>();    
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionPropertyAssociation"/> class.
        /// </summary>
        /// <param name="collectionItemType">The <see cref="Type"/> of the items.</param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/></param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="collectionItemType"/>' cannot be null.</exception>
        internal CollectionPropertyAssociation([NotNull] Type collectionItemType, PropertyInfo propertyInfo)
            : base(propertyInfo)
        {
            if (collectionItemType == null)
            {
                throw new ArgumentNullException(nameof(collectionItemType));
            }

            this.CollectionItemDescriptors = new List<CollectionItem>();
            this.AssemblyQualifiedItemTypeName = collectionItemType.AssemblyQualifiedName;
        }

        /// <summary>
        /// Gets the assembly qualified type name of the item type.
        /// </summary>
        [NotNull]
        [Required]
        [Index("IX_AssemblyQualifiedItemTypeName")]
        public String AssemblyQualifiedItemTypeName { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        Object IValueAssociation.Value
        {
            get { return this.CollectionItemDescriptors; }
            set { this.CollectionItemDescriptors = (ICollection<CollectionItem>)value; }
        }

        /// <summary>
        /// Gets the list of <see cref="CollectionItem"/> instances.
        /// </summary>
        [NotNull]
        public virtual ICollection<CollectionItem> CollectionItemDescriptors { get; set; }
    }
}
