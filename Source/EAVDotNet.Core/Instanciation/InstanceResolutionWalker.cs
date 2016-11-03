namespace JanHafner.EAVDotNet.Instanciation
{
    using TypeResolution;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;
    using JetBrains.Annotations;
    using JanHafner.EAVDotNet.Model;
    using JanHafner.Toolkit.Common.Reflection;

    /// <summary>
    /// Walks over a <see cref="Type"/> and creates <see cref="Object"/> instances.
    /// Instanciation via MEF is transient.
    /// </summary>
    [Export(typeof(IInstanceResolutionWalker))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal sealed class InstanceResolutionWalker : TypeWalker, IInstanceResolutionWalker
    {
        [NotNull]
        private readonly IEnumerable<IPropertyValueResolver> propertyValueResolvers;

        [NotNull]
        private readonly ITypeInstanciator typeInstanciator;

        [CanBeNull]
        private Object instanciatedType;

        [CanBeNull]
        private InstanceResolutionContext instanceResolutionContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceResolutionWalker"/> class.
        /// </summary>
        /// <param name="propertyReflector">The <see cref="IPropertyReflector"/>.</param>
        /// <param name="propertyValueResolvers">The <see cref="IPropertyValueResolver"/>.</param>
        /// <param name="typeInstanciator">The <see cref="ITypeInstanciator"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyValueResolvers"/>' and '<paramref name="typeInstanciator"/>' cannot be null.</exception>
        [ImportingConstructor]
        public InstanceResolutionWalker([NotNull] IPropertyReflector propertyReflector,
            [NotNull, ImportMany(typeof (IPropertyValueResolver))] IEnumerable<IPropertyValueResolver> propertyValueResolvers,
            [NotNull] ITypeInstanciator typeInstanciator)
            : base(propertyReflector)
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
        /// Creates an object from the supplied <see cref="InstanceResolutionContext"/>.
        /// </summary>
        /// <param name="instanceResolutionContext">The <see cref="InstanceResolutionContext"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instanceResolutionContext"/>' cannot be null.</exception>
        /// <returns>The created object.</returns>
        [NotNull]
        public Object ResolveInstance(InstanceResolutionContext instanceResolutionContext)
        {
            if (instanceResolutionContext == null)
            {
                throw new ArgumentNullException(nameof(instanceResolutionContext));
            }

            this.InitializedWithType = null;
            this.instanciatedType = null;
            this.instanceResolutionContext = null;

            this.instanceResolutionContext = instanceResolutionContext;

            Type type;
            try
            {
                type = Type.GetType(instanceResolutionContext.InstanceDescriptor.AssemblyQualifiedTypeName, true, true);
            }
            catch (TypeLoadException typeLoadException)
            {
                throw;
            }

            this.InitializedWithType = type;

            Object result;
            if (!instanceResolutionContext.InstanceRelationStore.TryGetRelated(instanceResolutionContext.InstanceDescriptor, out result))
            {
                var typeInstanciationContext = instanceResolutionContext.NoChangeTracking &&
                                               instanceResolutionContext.NoLazyLoading
                    ? new DefaultTypeInstanciationContext(type,
                        instanceResolutionContext.InstanceRelationStore)
                    : (ITypeInstanciationContext)
                        new InstanceProxyingInstanciationContext(instanceResolutionContext.InstanceDescriptor, type,
                            instanceResolutionContext.InstanceRelationStore)
                        {
                            NoChangeTracking = instanceResolutionContext.NoChangeTracking,
                            NoLazyLoading = instanceResolutionContext.NoLazyLoading
                        };

                this.instanciatedType = this.typeInstanciator.CreateInstance(typeInstanciationContext);

                result = this.instanciatedType;

                this.Walk();

                instanceResolutionContext.InstanceRelationStore.CreateRelation(instanceResolutionContext.InstanceDescriptor, this.instanciatedType);
            }

            return result;
        }

        /// <summary>
        /// The <see cref="TypeWalker"/> is stepped on a property.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> of the property.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyInfo"/>' cannot be null.</exception>
        protected override void StepOnPropertyInfo(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            var propertyValueAssociation = this.instanceResolutionContext.InstanceDescriptor.PropertyValues.FirstOrDefault(pv => pv.Name == propertyInfo.Name && pv.AssemblyQualifiedPropertyTypeName == propertyInfo.PropertyType.AssemblyQualifiedName);
            var propertyValue = propertyValueAssociation as IValueAssociation;

            if (propertyValue == null)
            {
                return;
            }

            var propertyValueResolver = this.propertyValueResolvers.SingleOrDefault(pvr => pvr.CanResolveValue(propertyValue));
            if (propertyValueResolver == null)
            {
                return;
            }

            if((propertyValueResolver is CollectionPropertyValueResolver || propertyValueResolver is InstancePropertyValueResolver) && !this.instanceResolutionContext.NoLazyLoading)
            {
                return;
            }

            var value = propertyValueResolver.ResolveValue(this.instanceResolutionContext.CreatePropertyValueResolutionContext(propertyValue));

            propertyInfo.SetValue(this.instanciatedType, value);
        }
    }
}
