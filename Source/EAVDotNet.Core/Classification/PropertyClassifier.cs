namespace JanHafner.EAVDotNet.Classification
{
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using JanHafner.EAVDotNet.Model;
    using Toolkit.Common.ExtensionMethods;
    using JetBrains.Annotations;

    /// <summary>
    /// Classifies properties.
    /// </summary>
    internal static class PropertyClassifier
    {
        private static readonly ConcurrentDictionary<PropertyInfo, PropertyClassification> propertyClassificationCache;

        static PropertyClassifier()
        {
            propertyClassificationCache = new ConcurrentDictionary<PropertyInfo, PropertyClassification>();
        }

        /// <summary>
        /// Classifies the supplied <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> to classify.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyInfo"/>' cannot be null.</exception>
        /// <returns>The classification of the property.</returns>
        public static PropertyClassification Classify([NotNull] this PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            return propertyClassificationCache.GetOrAdd(propertyInfo, _ => propertyInfo.IsClassifiedAsPrimaryKey()
                ? PropertyClassification.PrimaryKey
                : PropertyClassification.Unclassified);
        }

        /// <summary>
        /// Classifies the supplied <see cref="PropertyInfo"/> as the primary key of the complex object.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> to classify.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyInfo"/>' cannot be null.</exception>
        /// <returns>A value indicating, whether, the property is the primary key of the complex object.</returns>
        private static Boolean IsClassifiedAsPrimaryKey([NotNull] this PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            return propertyInfo.HasAttributeExactly<KeyAttribute>() 
                && propertyInfo.CanRead 
                && propertyInfo.CanWrite 
                && propertyInfo.GetGetMethod().IsPublic;
        }

        /// <summary>
        /// The possible classification values.
        /// </summary>
        public enum PropertyClassification
        {
            /// <summary>
            /// The <see cref="PropertyInfo"/> is unclassified.
            /// </summary>
            Unclassified = 0,

            /// <summary>
            /// The <see cref="PropertyInfo"/> is definitely the primary key of the complex instance, BUT that does not mean it does look a like the primary key from the <see cref="InstanceDescriptor"/>.
            /// </summary>
            PrimaryKey = 1
        }
    }
}
