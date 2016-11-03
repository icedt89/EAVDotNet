namespace JanHafner.EAVDotNet.Instrumentation.InstanceSystem.PropertyValueFactories
{
    using JanHafner.EAVDotNet.Classification;
    using System;
    using System.ComponentModel.Composition;
    using System.Reflection;
    using JanHafner.EAVDotNet.Model;
    using JanHafner.EAVDotNet.Model.Values;
    using JetBrains.Annotations;

    /// <summary>
    /// Creates instances of <see cref="InstancePropertyAssociation"/> instances.
    /// </summary>
    [Export(typeof(IPropertyValueFactory))]
    internal sealed class InstancePropertyValueFactory : PropertyValueFactory
    {
        [NotNull]
        private readonly Lazy<IInstanceDescriptorFactory> instanceDescriptorFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstancePropertyValueFactory"/> class.
        /// </summary>
        /// <param name="instanceDescriptorFactory">The <see cref="IInstanceDescriptorFactory"/> which creates an instance of the related <see cref="InstanceDescriptor"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instanceDescriptorFactory"/>' cannot be null.</exception>
        [ImportingConstructor]
        public InstancePropertyValueFactory([NotNull] Lazy<IInstanceDescriptorFactory> instanceDescriptorFactory)
        {
            if (instanceDescriptorFactory == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptorFactory));
            }

            this.instanceDescriptorFactory = instanceDescriptorFactory;
        }

        /// <summary>
        /// Checks if the supplied <see cref="Type"/> is classified as complex.
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

            return propertyType.Classify() == TypeClassifier.TypeClassification.Complex;
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

            InstanceDescriptor instanceDescriptor;
            if (propertyValueCreationContext.InstanceDescriptorCreationContext.InstanceRelationStore.TryGetRelated(propertyValue, out instanceDescriptor))
            {
                if (instanceDescriptor == null)
                {
                    throw new InvalidOperationException();
                }
            }

            if (instanceDescriptor == null)
            {
                instanceDescriptor = this.instanceDescriptorFactory.Value.CreateInstanceDescriptor(propertyValueCreationContext.InstanceDescriptorCreationContext.CreateChildContext(propertyValue));
            }
            
            return new InstancePropertyAssociation(instanceDescriptor, propertyValueCreationContext.PropertyInfo);
        }
    }
}