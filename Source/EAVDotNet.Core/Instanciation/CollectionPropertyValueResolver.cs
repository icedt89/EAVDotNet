namespace JanHafner.EAVDotNet.Instanciation
{
    using TypeResolution;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;
    using JanHafner.EAVDotNet.Instanciation.ChangeTracking;
    using JanHafner.EAVDotNet.Model;
    using JanHafner.EAVDotNet.Model.Values;
    using JetBrains.Annotations;

    /// <summary>
    /// Is responsible for resolving collection property values.
    /// </summary>
    [Export(typeof(IPropertyValueResolver))]
    internal sealed class CollectionPropertyValueResolver : IPropertyValueResolver
    {
        [NotNull]
        private readonly IEnumerable<Lazy<IPropertyValueResolver>> propertyValueResolvers;

        [NotNull]
        private readonly ITypeInstanciator typeInstanciator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionPropertyValueResolver"/> class.
        /// </summary>
        /// <param name="propertyValueResolvers">A list of <see cref="IPropertyValueResolver"/> instances.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyValueResolvers"/>' and '<paramref name="typeInstanciator"/>' cannot be null.</exception>
        /// <param name="typeInstanciator">The <see cref="ITypeInstanciator"/>.</param>
        [ImportingConstructor]
        public CollectionPropertyValueResolver(
            [NotNull, ImportMany(typeof (IPropertyValueResolver))] IEnumerable<Lazy<IPropertyValueResolver>> propertyValueResolvers,
            [NotNull] ITypeInstanciator typeInstanciator)
        {
            if (propertyValueResolvers == null)
            {
                throw new ArgumentNullException(nameof(propertyValueResolvers));
            }

            if (typeInstanciator == null)
            {
                throw new ArgumentNullException(nameof(typeInstanciator));
            }

            this.propertyValueResolvers = propertyValueResolvers;
            this.typeInstanciator = typeInstanciator;
        }

        /// <summary>
        /// Checks if the supplied <see cref="IValueAssociation"/> is an <see cref="CollectionPropertyAssociation"/>.
        /// </summary>
        /// <param name="propertyValue">The <see cref="IValueAssociation"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyValue"/>' cannot be null.</exception>
        /// <returns>A value indicating, whether, this instance can handle the <see cref="IValueAssociation"/>.</returns>
        public Boolean CanResolveValue(IValueAssociation propertyValue)
        {
            if (propertyValue == null)
            {
                throw new ArgumentNullException(nameof(propertyValue));
            }

            return propertyValue is CollectionPropertyAssociation;
        }

        /// <summary>
        /// Resolves the collection from the <see cref="CollectionPropertyAssociation"/>.
        /// </summary>
        /// <param name="propertyValueResolutionContext">The <see cref="PropertyValueResolutionContext"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyValueResolutionContext"/>' cannot be null.</exception>
        /// <returns>The resolved value.</returns>
        public Object ResolveValue(PropertyValueResolutionContext propertyValueResolutionContext)
        {
            if (propertyValueResolutionContext == null)
            {
                throw new ArgumentNullException(nameof(propertyValueResolutionContext));
            }

            var collectionPropertyAssociation = (CollectionPropertyAssociation)propertyValueResolutionContext.PropertyValue;

            MethodBase addMethod;
            var newCollectionInstance = this.CreateDynamicListInstance(collectionPropertyAssociation, propertyValueResolutionContext.InstanceResolutionContext, out addMethod);

            IPropertyValueResolver propertyValueResolver = null;
            foreach (var item in collectionPropertyAssociation.CollectionItemDescriptors.Cast<IValueAssociation>().ToList())
            {
                var itemPropertyResolutionContext = propertyValueResolutionContext.InstanceResolutionContext.CreatePropertyValueResolutionContext(item);
             
                this.ResolveCollectionItemAndAdd(newCollectionInstance, addMethod, itemPropertyResolutionContext, ref propertyValueResolver);
            }

            return newCollectionInstance;
        }

        /// <summary>
        /// Resolves the collection item and adds it to the collection.
        /// If <paramref name="itemPropertyValueResolver"/> is null, it will be resolved for the <see cref="IValueAssociation"/>, otherweise the supplied instance will be used.
        /// </summary>
        /// <param name="collection">An instance of the created <see cref="List{T}"/>.</param>
        /// <param name="addMethod">The reference to the Add-method.</param>
        /// <param name="propertyValueResolutionContext">The <see cref="PropertyValueResolutionContext"/>.</param>
        /// <param name="itemPropertyValueResolver">The used or created resolved <see cref="IPropertyValueResolver"/> instance.</param>
        private void ResolveCollectionItemAndAdd([NotNull] Object collection, [NotNull] MethodBase addMethod,
            [NotNull] PropertyValueResolutionContext propertyValueResolutionContext, [CanBeNull] ref IPropertyValueResolver itemPropertyValueResolver)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (addMethod == null)
            {
                throw new ArgumentNullException(nameof(addMethod));
            }

            if (propertyValueResolutionContext == null)
            {
                throw new ArgumentNullException(nameof(propertyValueResolutionContext));
            }

            if (itemPropertyValueResolver == null)
            {
                var propertyValueResolver = this.propertyValueResolvers.First(pvr => pvr.Value.CanResolveValue(propertyValueResolutionContext.PropertyValue));

                itemPropertyValueResolver = propertyValueResolver.Value;
            }

            var resolvedValue = itemPropertyValueResolver.ResolveValue(propertyValueResolutionContext);

            addMethod.Invoke(collection, new[] { resolvedValue });
        }

        /// <summary>
        /// Creates a new <see cref="List{T}"/> based on the supplied <see cref="CollectionPropertyAssociation"/>.
        /// Outputs the <see cref="MethodBase"/> pointing to the Add-method for further usage.
        /// </summary>
        /// <param name="collectionPropertyAssociation">The <see cref="CollectionPropertyAssociation"/>.</param>
        /// <param name="InstanceResolutionContext"></param>
        /// <param name="methodBase">The <see cref="MethodBase"/> of the Add-method.</param>
        /// <returns>An instance of the <see cref="List{T}"/>.</returns>
        [NotNull]
        private Object CreateDynamicListInstance([NotNull] CollectionPropertyAssociation collectionPropertyAssociation,
            [NotNull] InstanceResolutionContext InstanceResolutionContext, [NotNull] out MethodBase methodBase)
        {
            if (collectionPropertyAssociation == null)
            {
                throw new ArgumentNullException(nameof(collectionPropertyAssociation));
            }

            if (InstanceResolutionContext == null)
            {
                throw new ArgumentNullException(nameof(InstanceResolutionContext));
            }

            var newCollectionDefinition = typeof(ProxyableList<>);

            var collectionItemType = Type.GetType(collectionPropertyAssociation.AssemblyQualifiedItemTypeName, true, true);
            var realCollectionType = newCollectionDefinition.MakeGenericType(collectionItemType);

            methodBase = realCollectionType.GetMethod("AddNonProxied", BindingFlags.Instance | BindingFlags.NonPublic, null,
                new[] { collectionItemType }, new ParameterModifier[0]);

            var typeInstanciationContext = InstanceResolutionContext.NoChangeTracking && InstanceResolutionContext.NoLazyLoading
                ? new DefaultTypeInstanciationContext(realCollectionType,
                    InstanceResolutionContext.InstanceRelationStore)
                : (ITypeInstanciationContext) new CollectionProxyingInstanciationContext(realCollectionType,
                    InstanceResolutionContext.InstanceRelationStore, InstanceResolutionContext.InstanceDescriptor,
                    collectionPropertyAssociation)
                {
                    NoChangeTracking = InstanceResolutionContext.NoChangeTracking
                };

            return this.typeInstanciator.CreateInstance(typeInstanciationContext);
        }
    }
}