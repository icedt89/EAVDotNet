namespace JanHafner.EAVDotNet.Classification
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using JetBrains.Annotations;

    /// <summary>
    /// Classifies methods.
    /// </summary>
    internal static class MethodClassifier
    {
        [NotNull]
        private static readonly ConcurrentDictionary<MethodInfo, MethodClassification> methodClassificationCache;

        [NotNull]
        private static readonly String propertyGetMethodPrefix = "get_";

        [NotNull]
        private static readonly String propertySetMethodPrefix = "set_";

        [NotNull]
        private static readonly String collectionAddMethodName = "Add";

        [NotNull]
        private static readonly String collectionRemoveMethodName = "Remove";

        [NotNull]
        private static readonly String collectionClearMethodName = "Clear";

        static MethodClassifier()
        {
            methodClassificationCache = new ConcurrentDictionary<MethodInfo, MethodClassification>();
        }

        /// <summary>
        /// Classifies the supplied <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="methodInfo">The <see cref="MethodInfo"/> to classify.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="methodInfo"/>' cannot be null.</exception>
        /// <returns>The classification of the <see cref="MethodInfo"/>.</returns>
        public static MethodClassification Classify([NotNull] this MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            return methodClassificationCache.GetOrAdd(methodInfo, _ => methodInfo.IsClassifiedAsPropertyGetMethod()
                ? MethodClassification.PropertyGetMethod
                : methodInfo.IsClassifiedAsPropertySetMethod()
                    ? MethodClassification.PropertySetMethod
                    : methodInfo.IsClassifiedAsCollectionAddMethod()
                        ? MethodClassification.CollectionAddMethod
                        : methodInfo.IsClassifiedAsCollectionRemoveMethod()
                            ? MethodClassification.CollectionRemoveMethod
                            : methodInfo.IsClassifiedAsCollectionClearMethod()
                                ? MethodClassification.CollectionClearMethod
                                : MethodClassification.Unclassified);
        }

        /// <summary>
        /// Classifies the supplied <see cref="MethodInfo"/> as a collection Add()-method.
        /// </summary>
        /// <param name="methodInfo">The <see cref="MethodInfo"/> to classify.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="methodInfo"/>' cannot be null.</exception>
        /// <returns>A value indicating, whether, the method is classified as collection Add()-method.</returns>
        private static Boolean IsClassifiedAsCollectionAddMethod([NotNull] this MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            if (methodInfo.DeclaringType.Classify() != TypeClassifier.TypeClassification.Collection)
            {
                return false;
            }

            var methodParameters = methodInfo.GetParameters();
            return methodInfo.ReturnType == typeof (void) && methodParameters.Length == 1 &&
                   methodInfo.Name == collectionAddMethodName;
        }

          /// <summary>
        /// Classifies the supplied <see cref="MethodInfo"/> as a collection Clear()-method.
        /// </summary>
        /// <param name="methodInfo">The <see cref="MethodInfo"/> to classify.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="methodInfo"/>' cannot be null.</exception>
        /// <returns>A value indicating, whether, the method is classified as collection Clear()-method.</returns>
        private static Boolean IsClassifiedAsCollectionClearMethod([NotNull] this MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            if (methodInfo.DeclaringType.Classify() != TypeClassifier.TypeClassification.Collection)
            {
                return false;
            }

            var methodParameters = methodInfo.GetParameters();
            return methodInfo.ReturnType == typeof(void) && methodParameters.Length == 0 &&
                   methodInfo.Name == collectionClearMethodName;
        }

        /// <summary>
        /// Classifies the supplied <see cref="MethodInfo"/> as a collection Remove()-method.
        /// </summary>
        /// <param name="methodInfo">The <see cref="MethodInfo"/> to classify.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="methodInfo"/>' cannot be null.</exception>
        /// <returns>A value indicating, whether, the method is classified as collection Remove()-method.</returns>
        private static Boolean IsClassifiedAsCollectionRemoveMethod([NotNull] this MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            if (methodInfo.DeclaringType.Classify() != TypeClassifier.TypeClassification.Collection)
            {
                return false;
            }

            var methodParameters = methodInfo.GetParameters();
            return methodInfo.ReturnType == typeof(Boolean) && methodParameters.Length == 1 &&
                   methodInfo.Name == collectionRemoveMethodName;
        }

        /// <summary>
        /// Indicates if the supplied <see cref="MethodInfo"/> is a Set-method.
        /// </summary>
        /// <param name="methodInfo">The <see cref="MethodInfo"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="methodInfo"/>' cannot be null.</exception>
        /// <returns>A value indicating, whether, the <see cref="MethodInfo"/> is a Set-method.</returns>
        private static Boolean IsClassifiedAsPropertySetMethod([NotNull] this MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            return methodInfo.Name.StartsWith(propertySetMethodPrefix, StringComparison.CurrentCultureIgnoreCase) && methodInfo.ReturnType == typeof(void);
        }

        /// <summary>
        /// Indicates if the supplied <see cref="MethodInfo"/> is a Get-method.
        /// </summary>
        /// <param name="methodInfo">The <see cref="MethodInfo"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="methodInfo"/>' cannot be null.</exception>
        /// <returns>A value indicating, whether, the <see cref="MethodInfo"/> is a Get-method.</returns>
        private static Boolean IsClassifiedAsPropertyGetMethod([NotNull] this MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            return methodInfo.Name.StartsWith(propertyGetMethodPrefix, StringComparison.CurrentCultureIgnoreCase) && methodInfo.ReturnType != typeof(void);
        }

        /// <summary>
        /// Removes the four first digits of the method name supplied by the <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="methodInfo">The <see cref="MethodInfo"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="methodInfo"/>' cannot be null.</exception>
        /// <returns>The name of the method with the first four digits stripped away.</returns>
        public static String StripMethodPrefix([NotNull] this MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            // Handled prefix length is always 4 (get_, set_)!
            return methodInfo.Name.Remove(0, 4);
        }

        /// <summary>
        /// Tries to get the corresponding <see cref="PropertyInfo"/> for the supplied <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="methodInfo">The <see cref="MethodInfo"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="methodInfo"/>' cannot be null.</exception>
        /// <param name="propertyInfo">The corresponding <see cref="PropertyInfo"/>.</param>
        /// <returns>A value indicating whether the <see cref="MethodInfo"/> relates to a <see cref="PropertyInfo"/>.</returns>
        public static Boolean TryGetPropertyInfoFromMethodInfo([NotNull] this MethodInfo methodInfo, out PropertyInfo propertyInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            propertyInfo = null;
            if (methodInfo.Classify() != MethodClassification.PropertySetMethod &&
                methodInfo.Classify() != MethodClassification.PropertyGetMethod)
            {
                return false;
            }

            var propertyName = methodInfo.StripMethodPrefix();
            propertyInfo = methodInfo.DeclaringType.GetProperty(propertyName);
            return true;
        }

        /// <summary>
        /// The possible classification values.
        /// </summary>
        public enum MethodClassification
        {
            /// <summary>
            /// The <see cref="MethodInfo"/> is unclassified.
            /// </summary>
            Unclassified = 0,

            /// <summary>
            /// The <see cref="MethodInfo"/> could be a Get-method of a property.
            /// </summary>
            PropertyGetMethod = 1,

            /// <summary>
            /// The <see cref="MethodInfo"/> could be a Set-method of a property.
            /// </summary>
            PropertySetMethod = 2,

            /// <summary>
            /// The <see cref="MethodInfo"/> could be a Add-method of a collection.
            /// </summary>
            CollectionAddMethod = 3,

            /// <summary>
            /// The <see cref="MethodInfo"/> could be a Remove-method of a collection.
            /// </summary>
            CollectionRemoveMethod = 4,

            /// <summary>
            /// The <see cref="MethodInfo"/> could be a Clear-method of a collection.
            /// </summary>
            CollectionClearMethod = 5
        }
    }
}
