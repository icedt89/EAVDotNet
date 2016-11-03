namespace JanHafner.EAVDotNet.Model.Identity
{
    using System;
    using System.Linq;
    using System.Reflection;
    using JanHafner.EAVDotNet.Classification;
    using JetBrains.Annotations;

    /// <summary>
    /// Provides access to methods helping retrieving the identity of an object.
    /// </summary>
    internal static class ObjectIdentityExtensions
    {
        /// <summary>
        /// Returns the <see cref="PropertyInfo"/> which is specified as [Key].
        /// </summary>
        /// <param name="type">The <see cref="Type"/>.</param>
        /// <exception cref="MoreThanOneIdentifyingPropertySpecifiedException">There is more than one [Key] defined.</exception>
        /// <exception cref="IdentitfyingPropertyNotFoundException">There is no [Key] specified.</exception>
        /// <returns>The <see cref="PropertyInfo"/> specified as [Key].</returns>
        public static PropertyInfo GetIdentityProperty([NotNull] this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var identifyingProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(propertyInfo => propertyInfo.Classify().HasFlag(PropertyClassifier.PropertyClassification.PrimaryKey))
                .ToList();
            if (!identifyingProperties.Any())
            {
                throw new IdentitfyingPropertyNotFoundException(type);
            }

            var identifiedProperty = identifyingProperties.SingleOrDefault();
            if (identifiedProperty == null)
            {
                throw new MoreThanOneIdentifyingPropertySpecifiedException(type);
            }

            return identifiedProperty;
        }

        /// <summary>
        /// Checks if the identity of the supplied objects are equal.
        /// </summary>
        /// <param name="a">Object a.</param>
        /// <param name="b">Object b.</param>
        /// <returns>A value indicating, whether, the keys of the supplied objects are equal.</returns>
        public static Boolean IdentityEquals([NotNull] this Object a, [NotNull] Object b)
        {
            var objectAKey = a.GetIdentityValue();
            var objectBKey = b.GetIdentityValue();

            var identityEquality = Equals(objectAKey, objectBKey);

            return identityEquality;
        }

        /// <summary>
        /// Returns the value of the <see cref="PropertyInfo"/> specified as [Key].
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>The value.</returns>
        public static Object GetIdentityValue([NotNull] this Object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var identityProperty = instance.GetType().TryUnwrapProxiedType().GetIdentityProperty();
            return identityProperty.GetValue(instance);
        }
    }
}
