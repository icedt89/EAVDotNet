namespace JanHafner.EAVDotNet.Instanciation
{
    using System;
    using System.ComponentModel.Composition;
    using JanHafner.EAVDotNet.Model;
    using JanHafner.EAVDotNet.Model.Values;
    using JetBrains.Annotations;

    /// <summary>
    /// Is responsible for resolving complex instances of property values.
    /// </summary>
    [Export(typeof(IPropertyValueResolver))]
    internal sealed class InstancePropertyValueResolver : IPropertyValueResolver
    {
        [NotNull]
        private readonly ExportFactory<IInstanceResolutionWalker> instanceResolutionWalkerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstancePropertyValueResolver"/> class.
        /// </summary>
        /// <param name="instanceResolutionWalkerFactory">The <see cref="IInstanceResolutionWalker"/> factory.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instanceResolutionWalkerFactory"/>' cannot be null.</exception>
        [ImportingConstructor]
        public InstancePropertyValueResolver([NotNull] [Import(typeof(IInstanceResolutionWalker))] ExportFactory<IInstanceResolutionWalker> instanceResolutionWalkerFactory)
        {
            if (instanceResolutionWalkerFactory == null)
            {
                throw new ArgumentNullException(nameof(instanceResolutionWalkerFactory));
            }

            this.instanceResolutionWalkerFactory = instanceResolutionWalkerFactory;
        }

        /// <summary>
        /// Checks if the supplied <see cref="IValueAssociation"/> is an <see cref="InstancePropertyAssociation"/>.
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

            return propertyValue is IInstanceValueAssociation;
        }

        /// <summary>
        /// Resolves the complex value from the <see cref="InstancePropertyAssociation"/>.
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

            var instanceAssociation= (IInstanceValueAssociation)propertyValueResolutionContext.PropertyValue;
            var instanceDescriptor = instanceAssociation.Instance;

            Object resolvedInstance;
            if (propertyValueResolutionContext.InstanceResolutionContext.InstanceRelationStore.TryGetRelated(instanceDescriptor, out resolvedInstance))
            {
                if (instanceDescriptor == null)
                {
                    throw new InvalidOperationException();
                }
            }

            if (resolvedInstance == null)
            {
                using (var instanceResolutionWalker = this.instanceResolutionWalkerFactory.CreateExport())
                {
                    resolvedInstance = instanceResolutionWalker.Value.ResolveInstance(propertyValueResolutionContext.InstanceResolutionContext.CreateChildContext(instanceDescriptor));
                }    
            }

            return resolvedInstance;
        }
    }
}