namespace JanHafner.EAVDotNet.Instanciation.ChangeTracking
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Castle.DynamicProxy;
    using JanHafner.EAVDotNet.Classification;
    using JanHafner.EAVDotNet.Instrumentation.InstanceSystem.PropertyValueFactories;
    using JanHafner.EAVDotNet.Model;
    using JetBrains.Annotations;

    /// <summary>
    /// Base class for property interceptors which intercept the set_-method.
    /// </summary>
    internal abstract class PropertyChangeTrackingInterceptor : PropertyInterceptor
    {
        [NotNull]
        protected readonly InstanceDescriptor InstanceDescriptor;

        [NotNull]
        private readonly IPropertyValueFactoryProvider propertyValueFactoryProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangeTrackingInterceptor"/> class.
        /// </summary>
        /// <param name="instanceDescriptor">The associated <see cref="Model.InstanceDescriptor"/>.</param>
        /// <param name="propertyValueFactoryProvider">The <see cref="IPropertyValueFactoryProvider"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyValueFactoryProvider"/>' and '<paramref name="instanceDescriptor"/>' cannot be null.</exception>
        protected PropertyChangeTrackingInterceptor([NotNull] InstanceDescriptor instanceDescriptor,
            [NotNull] IPropertyValueFactoryProvider propertyValueFactoryProvider)
        {
            if (instanceDescriptor == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptor));
            }

            if (propertyValueFactoryProvider == null)
            {
                throw new ArgumentNullException(nameof(propertyValueFactoryProvider));
            }

            this.InstanceDescriptor = instanceDescriptor;
            this.propertyValueFactoryProvider = propertyValueFactoryProvider;
        }

        /// <summary>
        /// Overriden in a derived class, handles the interception and provides the <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="invocation">The <see cref="IInvocation"/>.</param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="invocation"/>' and '<paramref name="propertyInfo"/>' cannot be null.</exception>
        protected override void InterceptCore([NotNull] IInvocation invocation, [NotNull] PropertyInfo propertyInfo)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            var interceptedPropertyValue = invocation.Arguments[0];
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            var propertyValue = this.GetPropertyValue(propertyInfo);
            var propertyValueFactory = this.propertyValueFactoryProvider.GetPropertyValueFactory(propertyInfo.PropertyType);
            if (propertyValue == null && interceptedPropertyValue != null)
            {
                propertyValue = this.CreateNewPropertyValue(propertyValueFactory, invocation.InvocationTarget, propertyInfo, interceptedPropertyValue);

                this.InstanceDescriptor.PropertyValues.Add(propertyValue);
            }
            else
            {
                if (interceptedPropertyValue != null)
                {
                    if (interceptedPropertyValue != ((IValueAssociation)propertyValue).Value && !interceptedPropertyValue.GetType().IsEnumerable() && !interceptedPropertyValue.GetType().Classify().HasFlag(TypeClassifier.TypeClassification.CastleDynamicProxy))
                    {
                        ((IValueAssociation)propertyValue).Value = this.CreateValueForExistingPropertyValue(propertyValueFactory, invocation.InvocationTarget, propertyInfo, interceptedPropertyValue);
                    }
                }
                else if (propertyValue != null)
                {
                    this.RemovePropertyValue(propertyValue);
                }
            }
        }

        /// <summary>
        /// Tries to get the <see cref="PropertyValue"/> of the <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyInfo" />' cannot be null.</exception>
        /// <returns>A value indicating, whether, the <see cref="PropertyValue"/> was found.</returns>
        [CanBeNull]
        protected PropertyValue GetPropertyValue([NotNull] PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            return this.InstanceDescriptor.PropertyValues.SingleOrDefault(pv => pv.Name == propertyInfo.Name && propertyInfo.PropertyType.AssemblyQualifiedName == pv.AssemblyQualifiedPropertyTypeName);
        }

        /// <summary>
        /// Creates a new <see cref="PropertyValue"/>.
        /// </summary>
        /// <param name="propertyValueFactory">The <see cref="IPropertyValueFactory"/>.</param>
        /// <param name="instance">The instance of the complex object.</param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> which is intercepted.</param>
        /// <param name="value">The intercepted property value.</param>
        /// <returns>The created <see cref="PropertyValue"/>.</returns>
        protected abstract PropertyValue CreateNewPropertyValue(IPropertyValueFactory propertyValueFactory, Object instance, PropertyInfo propertyInfo, Object value);

        /// <summary>
        /// Returns the value of an existing <see cref="PropertyValue"/>.
        /// </summary>
        /// <param name="propertyValueFactory">The <see cref="IPropertyValueFactory"/>.</param>
        /// <param name="instance">The instance of the complex object.</param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> which is intercepted.</param>
        /// <param name="value">The intercepted property value.</param>
        /// <returns>The value of the existing <see cref="PropertyValue"/>.</returns>
        protected abstract Object CreateValueForExistingPropertyValue(IPropertyValueFactory propertyValueFactory, Object instance, PropertyInfo propertyInfo, Object value);

        /// <summary>
        /// Removes the supplied <see cref="PropertyValue"/> from the <see cref="Model.InstanceDescriptor"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyValue" />' cannot be null.</exception>
        /// <param name="propertyValue">The <see cref="PropertyValue"/> to remove.</param>
        protected virtual void RemovePropertyValue([NotNull] PropertyValue propertyValue)
        {
            if (propertyValue == null)
            {
                throw new ArgumentNullException(nameof(propertyValue));
            }

            this.InstanceDescriptor.PropertyValues.Remove(propertyValue);
        }
    }
}
