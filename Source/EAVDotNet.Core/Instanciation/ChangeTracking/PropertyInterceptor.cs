namespace JanHafner.EAVDotNet.Instanciation.ChangeTracking
{
    using System;
    using System.Reflection;
    using Castle.DynamicProxy;
    using JanHafner.EAVDotNet.Classification;
    using JetBrains.Annotations;

    /// <summary>
    /// base class for all interceptors wich intercept a property-method (get_ and set_).
    /// </summary>
    internal abstract class PropertyInterceptor : IInterceptor
    {
        /// <summary>
        /// Intercepts the method.
        /// </summary>
        /// <param name="invocation">Information about the invocation.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="invocation"/>' cannot be null.</exception>
        public void Intercept([NotNull] IInvocation invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            PropertyInfo propertyInfo;
            if (!invocation.Method.TryGetPropertyInfoFromMethodInfo(out propertyInfo))
            {
                throw new InvalidOperationException();
            }

            this.InterceptCore(invocation, propertyInfo);

            invocation.Proceed();
        }

        /// <summary>
        /// Overriden in a derived class, handles the interception and provides the <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="invocation">The <see cref="IInvocation"/>.</param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/>.</param>
        protected abstract void InterceptCore(IInvocation invocation, PropertyInfo propertyInfo);
    }
}
