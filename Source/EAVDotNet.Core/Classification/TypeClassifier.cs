namespace JanHafner.EAVDotNet.Classification
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Castle.DynamicProxy;
    using JanHafner.EAVDotNet.Instanciation.ChangeTracking;
    using JanHafner.Toolkit.Common.ExtensionMethods;
    using JetBrains.Annotations;

    /// <summary>
    /// Classifies a type.
    /// </summary>
    internal static class TypeClassifier
    {
        /// <summary>
        /// Specifies types that are classified as primitive.
        /// </summary>
        [NotNull]
        private static readonly IEnumerable<Type> typesClassifiedAsPrimitive;

        /// <summary>
        /// Specifies types that are classified as collection.
        /// </summary>
        [NotNull]
        private static readonly IEnumerable<Type> typesClassifiedAsGenericCollection;

        [NotNull]
        private static readonly ConcurrentDictionary<Type, TypeClassification> typeClassificationCache;

        /// <summary>
        /// Initializes the <see cref="TypeClassifier"/> type.
        /// </summary>
        static TypeClassifier()
        {
            typesClassifiedAsPrimitive = new[]
            {
                typeof(Boolean), typeof(Boolean?),
                typeof(Guid), typeof(Guid?),
                typeof(Int16), typeof(Int16?),
                typeof(Int32), typeof(Int32?),
                typeof(Int64), typeof(Int64?),
                typeof(String),
                typeof(DateTime), typeof(DateTime?),
                typeof(Char), typeof(Char?),
                typeof(Byte), typeof(Byte?),
                typeof(SByte), typeof(SByte?),
                typeof(UInt16), typeof(UInt16?),
                typeof(UInt32), typeof(UInt32?),
                typeof(UInt64), typeof(UInt64?),
                typeof(Byte[])
            };

            typesClassifiedAsGenericCollection = new[]
            {
                typeof(ICollection<>), typeof(Collection<>),
                typeof(IList<>), typeof(List<>),
                typeof(IEnumerable<>),
                typeof(ProxyableList<>)
            };

            typeClassificationCache = new ConcurrentDictionary<Type, TypeClassification>();
        }

        /// <summary>
        /// Classifies the supplied <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to classify.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="type"/>' cannot be null.</exception>
        /// <returns>The classification of the <see cref="Type"/>.</returns>
        public static TypeClassification Classify([NotNull] this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return typeClassificationCache.GetOrAdd(type, _ =>
            {
                var result = type.IsClassifiedAsPrimitiveType()
                    ? TypeClassification.Primitive
                    : type.IsClassifiedAsCollectionType()
                        ? TypeClassification.Collection
                        : type.IsClassifiedAsComplexType()
                            ? TypeClassification.Complex
                            : TypeClassification.Unclassified;

                if (type.IsClassifiedAsCastleDynamicProxy())
                {
                    result |= TypeClassification.CastleDynamicProxy;
                }

                return result;
            });
        }

        /// <summary>
        /// Checks if the supplied <see cref="Type"/> is classified as primitive.
        /// </summary>
        /// <param name="type">The <see cref="Type"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="type"/>' cannot be null.</exception>
        /// <returns>A value indicating if the <see cref="Type"/> is classified as primitive.</returns>
        private static Boolean IsClassifiedAsPrimitiveType([NotNull] this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return typesClassifiedAsPrimitive.Contains(type) || typesClassifiedAsPrimitive.Contains(type.TryUnwrapIfNullableType()) || type.IsEnum;
        }

        /// <summary>
        /// Checks if the supplied <see cref="Type"/> is classified as collection.
        /// </summary>
        /// <param name="type">The <see cref="Type"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="type"/>' cannot be null.</exception>
        /// <returns>A value indicating if the <see cref="Type"/> is classified as collection.</returns>
        private static Boolean IsClassifiedAsCollectionType([NotNull] this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsClassifiedAsPrimitiveType())
            {
                return false;
            }

            var genericTypeDefinition = type;
            if (genericTypeDefinition.IsConstructedGenericType)
            {
                genericTypeDefinition = type.GetGenericTypeDefinition();
            }

            return typesClassifiedAsGenericCollection.Contains(genericTypeDefinition);
        }

        /// <summary>
        /// Checks if the supplied <see cref="Type"/> is classified as complex.
        /// </summary>
        /// <param name="type">The <see cref="Type"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="type"/>' cannot be null.</exception>
        /// <returns>A value indicating if the <see cref="Type"/> is classified as complex.</returns>
        private static Boolean IsClassifiedAsComplexType([NotNull] this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return (type.IsClass || type.IsValueType) && !type.IsClassifiedAsPrimitiveType() && !type.IsClassifiedAsCollectionType();
        }

        /// <summary>
        /// Checks if the supplied <see cref="Type"/> is classified as complex.
        /// </summary>
        /// <param name="type">The <see cref="Type"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="type"/>' cannot be null.</exception>
        /// <returns>A value indicating if the <see cref="Type"/> is classified as a <see cref="Type"/> dynamically proxied by DynamicProxy of Castle Windsor.</returns>
        private static Boolean IsClassifiedAsCastleDynamicProxy([NotNull] this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return typeof (IProxyTargetAccessor).IsAssignableFrom(type);
        }

        /// <summary>
        /// Checks if the supplied <see cref="Type"/> is classified as a proxy and returns the underlying target <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to unwrap if it is a proxy.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="type"/>' cannot be null.</exception>
        /// <returns>The unwrapped <see cref="Type"/>.</returns>
        [NotNull]
        public static Type TryUnwrapProxiedType([NotNull] this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.Classify().HasFlag(TypeClassification.CastleDynamicProxy) 
                ? type.BaseType 
                : type;
        }

        /// <summary>
        /// The possible classification values.
        /// </summary>
        [Flags]
        public enum TypeClassification
        {
            /// <summary>
            /// The <see cref="Type"/> is unclassified.
            /// </summary>
            Unclassified = 0,

            /// <summary>
            /// The <see cref="Type"/> is classified as primitive. 
            /// </summary>
            Primitive = 1,

            /// <summary>
            /// The <see cref="Type"/> as a collection.
            /// </summary>
            Collection = 2,

            /// <summary>
            /// The <see cref="Type"/> as complex.
            /// </summary>
            Complex = 4,

            /// <summary>
            /// The <see cref="Type"/> is additionally classified as a DynamicProxy created by Casle Windsor.
            /// </summary>
            CastleDynamicProxy = 8
        }
    }
}
