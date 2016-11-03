namespace JanHafner.EAVDotNet.Instanciation.ChangeTracking
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Castle.DynamicProxy;
    using JanHafner.EAVDotNet.Classification;

    /// <summary>
    /// Classfies the input method and returns the specialized <see cref="IInterceptor"/> implementation.
    /// </summary>
    internal sealed class PropertyInterceptorSelector : IInterceptorSelector
    {
        /// <summary>
        /// Selects the interceptors that should intercept calls to the given <paramref name="method"/>.
        /// </summary>
        /// <param name="type">The type declaring the method to intercept.</param><param name="method">The method that will be intercepted.</param><param name="interceptors">All interceptors registered with the proxy.</param>
        /// <returns>
        /// An array of interceptors to invoke upon calling the <paramref name="method"/>.
        /// </returns>
        /// <remarks>
        /// This method is called only once per proxy instance, upon the first call to the
        ///               <paramref name="method"/>. Either an empty array or null are valid return values to indicate
        ///               that no interceptor should intercept calls to the method. Although it is not advised, it is
        ///               legal to return other <see cref="T:Castle.DynamicProxy.IInterceptor"/> implementations than these provided in
        ///               <paramref name="interceptors"/>.
        /// </remarks>
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            PropertyInfo propertyInfo;
            if (!method.TryGetPropertyInfoFromMethodInfo(out propertyInfo))
            {
                switch (method.Classify())
                {
                    case MethodClassifier.MethodClassification.CollectionAddMethod:
                        return interceptors.OfType<CollectionAddMethodInterceptor>().Cast<IInterceptor>().ToArray();
                    case MethodClassifier.MethodClassification.CollectionClearMethod:
                        return interceptors.OfType<CollectionClearMethodInterceptor>().Cast<IInterceptor>().ToArray();
                    case MethodClassifier.MethodClassification.CollectionRemoveMethod:
                        return interceptors.OfType<CollectionRemoveMethodInterceptor>().Cast<IInterceptor>().ToArray();
                    default:
                        return new IInterceptor[0];
                }
            }

            var propertyTypeClassification = propertyInfo.PropertyType.Classify();
            switch (method.Classify())
            {
                case MethodClassifier.MethodClassification.PropertyGetMethod:
                    return propertyTypeClassification != TypeClassifier.TypeClassification.Primitive
                        ? interceptors.OfType<LazyLoadingComplexPropertyInterceptor>().Cast<IInterceptor>().ToArray()
                        : new IInterceptor[0];
                case MethodClassifier.MethodClassification.PropertySetMethod:
                    switch (propertyTypeClassification)
                    {
                        case TypeClassifier.TypeClassification.Complex:
                        case TypeClassifier.TypeClassification.Collection:
                            return interceptors.OfType<ComplexPropertyChangeTrackingInterceptor>().Cast<IInterceptor>().ToArray();
                        case TypeClassifier.TypeClassification.Primitive:
                            return interceptors.OfType<PrimitivePropertyChangeTrackingInterceptor>().Cast<IInterceptor>().ToArray();
                        default:
                            return new IInterceptor[0];
                    }
                default:
                    return new IInterceptor[0];
            }
        }
    }
}