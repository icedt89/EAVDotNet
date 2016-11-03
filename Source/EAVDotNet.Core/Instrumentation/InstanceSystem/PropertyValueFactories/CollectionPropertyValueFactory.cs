using JanHafner.EAVDotNet.Classification;

namespace JanHafner.EAVDotNet.Instrumentation.InstanceSystem.PropertyValueFactories
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Reflection;
    using Collection;
    using JanHafner.EAVDotNet.Model;
    using JanHafner.EAVDotNet.Model.Collections;
    using JanHafner.EAVDotNet.Model.Values;
    using JetBrains.Annotations;

    /// <summary>
    /// Creates instances of <see cref="CollectionPropertyAssociation"/> instances.
    /// </summary>
    [Export(typeof(IPropertyValueFactory))]
    internal sealed class CollectionPropertyValueFactory : PropertyValueFactory
    {
        [NotNull]
        private readonly ICollectionItemFactoryProvider collectionItemFactoryProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionPropertyValueFactory"/> class.
        /// </summary>
        /// <param name="collectionItemFactoryProvider">The <see cref="ICollectionItemFactoryProvider"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="collectionItemFactoryProvider"/>' cannot be null.</exception>
        [ImportingConstructor]
        public CollectionPropertyValueFactory([NotNull] ICollectionItemFactoryProvider collectionItemFactoryProvider)
        {
            if (collectionItemFactoryProvider == null)
            {
                throw new ArgumentNullException(nameof(collectionItemFactoryProvider));
            }

            this.collectionItemFactoryProvider = collectionItemFactoryProvider;
        }

        /// <summary>
        /// Checks if the supplied <see cref="Type"/> represents an <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="propertyType">The <see cref="Type"/> of the property.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyType"/>' cannot be null.</exception>
        /// <returns>A value indicating if the implementor can create the instance.</returns>
        public override Boolean CanCreatePropertyValue(Type propertyType)
        {
            if (propertyType == null)
            {
                throw new ArgumentNullException(nameof(propertyType));
            }

            return propertyType.Classify() == TypeClassifier.TypeClassification.Collection;
        }

        /// <summary>
        /// Additionally supplies the value of the <see cref="PropertyInfo"/> to the implementor.
        /// </summary>
        /// <param name="propertyValueCreationContext">The <see cref="PropertyValueCreationContext"/>.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyValueCreationContext"/>' cannot be null.</exception>
        /// <returns>A derivation of the <see cref="PropertyValue"/>.</returns>
        protected override PropertyValue CreatePropertyValueCore(PropertyValueCreationContext propertyValueCreationContext, Object propertyValue)
        {
            if (propertyValue == null)
            {
                return new IgnorablePropertyValue();
            }

            if (propertyValueCreationContext == null)
            {
                throw new ArgumentNullException(nameof(propertyValueCreationContext));
            }

            var itemType = this.GetICollectionType(propertyValueCreationContext.PropertyInfo.PropertyType);
            var result = new CollectionPropertyAssociation(itemType, propertyValueCreationContext.PropertyInfo);

            var collectionItemFactory = this.collectionItemFactoryProvider.GetCollectionItemFactory(itemType);
            foreach (var item in (IEnumerable)propertyValue)
            {
                this.CreateAndAddCollectionItem(result, item, propertyValueCreationContext, collectionItemFactory);       
            }

            return result;
        }

        /// <summary>
        /// Creates a new <see cref="CollectionItem"/> and outputs the used <see cref="ICollectionItemFactory"/> for caching purpose.
        /// If <paramref name="collectionItemFactory"/> is null, the instance will be determined and set via <c>ref</c>, so it is possible that the next call can supply this instance for faster processing.
        /// </summary>
        /// <param name="collectionPropertyAssociation">The <see cref="CollectionPropertyAssociation"/>.</param>
        /// <param name="item">The instance of the collection item.</param>
        /// <param name="propertyValueCreationContext">The <see cref="PropertyValueCreationContext"/>.</param>
        /// <param name="collectionItemFactory"></param>
        private void CreateAndAddCollectionItem([NotNull] CollectionPropertyAssociation collectionPropertyAssociation,
            [CanBeNull] Object item,
            [NotNull] PropertyValueCreationContext propertyValueCreationContext,
            [NotNull] ICollectionItemFactory collectionItemFactory)
        {
            if (collectionPropertyAssociation == null)
            {
                throw new ArgumentNullException(nameof(collectionPropertyAssociation));
            }
            
            if (propertyValueCreationContext == null)
            {
                throw new ArgumentNullException(nameof(propertyValueCreationContext));
            }

            if (collectionItemFactory == null)
            {
                throw new ArgumentNullException(nameof(collectionItemFactory));
            }

            var collectionItem = collectionItemFactory.CreateCollectionItem(propertyValueCreationContext.InstanceDescriptorCreationContext.CreateCollectionItemCreationContext(item));
            if (collectionItem is IgnorableCollectionItem)
            {
                return;
            }

            collectionPropertyAssociation.CollectionItemDescriptors.Add(collectionItem);
        }

        /// <summary>
        /// Gets the generic <see cref="Type"/> of the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/>.</param>
        /// <returns>The <see cref="Type"/> of the items inside the <see cref="ICollection{T}"/>.</returns>
        [NotNull]
        private Type GetICollectionType([NotNull] Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GenericTypeArguments[0];
        }
    }
}