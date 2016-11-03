namespace JanHafner.EAVDotNet.Instanciation.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Castle.DynamicProxy;
    using JanHafner.EAVDotNet.Context.InstanceRelation;
    using JanHafner.EAVDotNet.Model;
    using JetBrains.Annotations;

    /// <summary>
    /// Lazy loads a property classified as Complex.
    /// </summary>
    internal sealed class LazyLoadingComplexPropertyInterceptor : PropertyInterceptor
    {
        private Boolean isPropertyGetterInitialized;

        [NotNull]
        private readonly InstanceDescriptor instanceDescriptor;

        [NotNull]
        private readonly IInstanceRelationStore instancRelationStore;

        [NotNull]
        private readonly IEnumerable<IPropertyValueResolver> propertyValueResolvers;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyLoadingComplexPropertyInterceptor"/> class.
        /// </summary>
        /// <param name="instanceDescriptor">The associated <see cref="InstanceDescriptor"/>.</param>
        /// <param name="instancRelationStore">The <see cref="IInstanceRelationStore"/>.</param>
        /// <param name="propertyValueResolvers">The <see cref="IInstanceRelationStore"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instanceDescriptor"/>', '<paramref name="instancRelationStore"/>' and '<paramref name="propertyValueResolvers"/>' cannot be null.</exception>
        public LazyLoadingComplexPropertyInterceptor([NotNull] InstanceDescriptor instanceDescriptor,
            [NotNull] IInstanceRelationStore instancRelationStore,
            [NotNull] IEnumerable<IPropertyValueResolver> propertyValueResolvers) 
        {
            if (instanceDescriptor == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptor));
            }

            if (instancRelationStore == null)
            {
                throw new ArgumentNullException(nameof(instancRelationStore));
            }

            if (propertyValueResolvers == null)
            {
                throw new ArgumentNullException(nameof(propertyValueResolvers));
            }

            this.instanceDescriptor = instanceDescriptor;
            this.instancRelationStore = instancRelationStore;
            this.propertyValueResolvers = propertyValueResolvers;
        }

        /// <summary>
        /// Overriden in a derived class, handles the interception and provides the <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="invocation">The <see cref="IInvocation"/>.</param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/>.</param>
        protected override void InterceptCore(IInvocation invocation, PropertyInfo propertyInfo)
        {
            if (this.isPropertyGetterInitialized)
            {
                return;
            }

            var propertyValueAssociation = this.instanceDescriptor.PropertyValues.FirstOrDefault(pv => pv.Name == propertyInfo.Name && pv.AssemblyQualifiedPropertyTypeName == propertyInfo.PropertyType.AssemblyQualifiedName);
            var propertyValue = propertyValueAssociation as IValueAssociation;

            if (propertyValue == null)
            {
                return;
            }

            var propertyValueResolver = this.propertyValueResolvers.SingleOrDefault(pvr => pvr.CanResolveValue(propertyValue));
            if (propertyValueResolver is PrimitivePropertyValueResolver)
            {
                return;
            }

            var value = propertyValueResolver.ResolveValue(new PropertyValueResolutionContext(propertyValue, new InstanceResolutionContext(this.instanceDescriptor, this.instancRelationStore)));

            this.isPropertyGetterInitialized = true;

            propertyInfo.SetValue(invocation.InvocationTarget, value);
        }
    }
}
