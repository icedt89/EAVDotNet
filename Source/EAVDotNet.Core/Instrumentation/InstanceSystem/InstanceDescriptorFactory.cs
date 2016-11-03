namespace JanHafner.EAVDotNet.Instrumentation.InstanceSystem
{
    using JanHafner.EAVDotNet.Classification;
    using System;
    using System.ComponentModel.Composition;
    using JanHafner.Toolkit.Common.Reflection;
    using JetBrains.Annotations;
    using PropertyValueFactories;
    using TypeSystem;
    using Model;
    using Model.ValueHandling;

    /// <summary>
    /// Default implementation of the <see cref="IInstanceDescriptorFactory"/>.
    /// </summary>
    [Export(typeof(IInstanceDescriptorFactory))]
    internal class InstanceDescriptorFactory : IInstanceDescriptorFactory
    {
        [NotNull]
        private readonly IPropertyReflector propertyReflector;

        [NotNull]
        private readonly IPropertyValueFactoryProvider propertyValueFactoryProvider;

        [NotNull]
        private readonly IPropertyValueHandlerFactory propertyValueHandlerFactory;

        [NotNull]
        private readonly ITypeAliasWalker typeAliasWalker;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceDescriptorFactory"/> class.
        /// </summary>
        /// <param name="propertyReflector">The <see cref="IPropertyReflector"/>.</param>
        /// <param name="propertyValueFactoryProvider">The <see cref="IPropertyValueFactoryProvider"/>.</param>
        /// <param name="propertyValueHandlerFactory">The <see cref="IPropertyValueHandlerFactory"/>.</param>
        /// <param name="typeAliasWalker">The <see cref="ITypeAliasWalker"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyReflector"/>', '<paramref name="typeAliasWalker"/>','<paramref name="propertyValueHandlerFactory"/>' and '<paramref name="propertyValueFactoryProvider"/>' cannot be null.</exception>
        [ImportingConstructor]
        public InstanceDescriptorFactory([NotNull] IPropertyReflector propertyReflector,
            [NotNull] IPropertyValueFactoryProvider propertyValueFactoryProvider,
            [NotNull] IPropertyValueHandlerFactory propertyValueHandlerFactory,
            [NotNull] ITypeAliasWalker typeAliasWalker)
        {
            if (propertyReflector == null)
            {
                throw new ArgumentNullException(nameof(propertyReflector));
            }

            if (propertyValueFactoryProvider == null)
            {
                throw new ArgumentNullException(nameof(propertyValueFactoryProvider));
            }

            if (propertyValueHandlerFactory == null)
            {
                throw new ArgumentNullException(nameof(propertyValueHandlerFactory));
            }

            if (typeAliasWalker == null)
            {
                throw new ArgumentNullException(nameof(typeAliasWalker));
            }

            this.propertyReflector = propertyReflector;
            this.propertyValueFactoryProvider = propertyValueFactoryProvider;
            this.propertyValueHandlerFactory = propertyValueHandlerFactory;
            this.typeAliasWalker = typeAliasWalker;
        }

        /// <summary>
        /// Creates new instances of <see cref="InstanceDescriptor"/> instances based on the information contained by the supplied <see cref="InstanceDescriptorCreationContext"/>.
        /// </summary>
        /// <param name="instanceDescriptorCreationContext">The <see cref="InstanceDescriptorCreationContext"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instanceDescriptorCreationContext"/>' cannot be null.</exception>
        /// <returns>The newly created <see cref="InstanceDescriptor"/>.</returns>
        public InstanceDescriptor CreateInstanceDescriptor(InstanceDescriptorCreationContext instanceDescriptorCreationContext)
        {
            if (instanceDescriptorCreationContext == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptorCreationContext));
            }

            InstanceDescriptor resultInstanceDescriptor;
            if (!instanceDescriptorCreationContext.InstanceRelationStore.TryGetRelated(instanceDescriptorCreationContext.CurrentInstance, out resultInstanceDescriptor))
            {
                var instanceType = instanceDescriptorCreationContext.CurrentInstance.GetType().TryUnwrapProxiedType();

                resultInstanceDescriptor = new InstanceDescriptor(instanceType);

                foreach (var propertyInfo in this.propertyReflector.ReflectProperties(instanceDescriptorCreationContext.CurrentInstance))
                {
                    var propertyValueFactory = this.propertyValueFactoryProvider.GetPropertyValueFactory(propertyInfo.PropertyType);
                    var propertyValue = propertyValueFactory.CreatePropertyValue(instanceDescriptorCreationContext.CreatePropertyValueCreationContext(propertyInfo));

                    var propertyValueHandler = this.propertyValueHandlerFactory.GetPropertyValueHandler(propertyValue, propertyInfo);
                    propertyValueHandler.HandlePropertyValue(resultInstanceDescriptor, propertyValue, propertyInfo);
                }

                this.typeAliasWalker.CreateAliases(resultInstanceDescriptor, instanceDescriptorCreationContext.CurrentInstance.GetType());

                instanceDescriptorCreationContext.InstanceRelationStore.CreateRelation(resultInstanceDescriptor, instanceDescriptorCreationContext.CurrentInstance);
            }

            return resultInstanceDescriptor;
        }
    }
}