namespace JanHafner.EAVDotNet.Query.Expression.Inlining
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using JanHafner.EAVDotNet.Model;
    using JetBrains.Annotations;

    /// <summary>
    /// Provides extension methods for the expression translation engine.
    /// Expressions in this class need to be inlinable.
    /// 
    /// The designt time method is public and the method inlined at runtime is private and must return a LambdaExpressionwith the same signature as the design time method.
    /// </summary>
    public static class InlinableExpression
    {
        /// <summary>
        /// Returns all <see cref="PropertyValue"/> instances of the <see cref="Type"/> defined by <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of <see cref="PropertyValue"/> instances to return.</typeparam>
        /// <param name="instanceDescriptor">The <see cref="InstanceDescriptor"/> to query.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instanceDescriptor"/>' cannot be null.</exception>
        /// <returns>A list of <see cref="PropertyValue"/> instances of <see cref="Type"/> <typeparamref name="T"/>.</returns>
        public static IEnumerable<T> Properties<T>([NotNull] this InstanceDescriptor instanceDescriptor)
            where T : PropertyValue
        {
            if (instanceDescriptor == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptor));
            }

            return Properties<T>().Compile()(instanceDescriptor);
        }

        private static Expression<Func<InstanceDescriptor, IEnumerable<T>>> Properties<T>()
            where T : PropertyValue
        {
            return id => id.PropertyValues.OfType<T>();
        }

        /// <summary>
        /// Returns all <see cref="PropertyValue"/> instances named by <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of <see cref="PropertyValue"/> instances to return.</typeparam>
        /// <param name="properties">The <see cref="PropertyValue"/> instances to query.</param>
        /// <param name="name">The name of the property.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="properties"/>' and '<paramref name="name"/>' cannot be null.</exception>
        /// <returns>A the <see cref="PropertyValue"/> named by <paramref name="name"/>.</returns>
        public static T Named<T>(this IEnumerable<T> properties, String name)
            where T : PropertyValue
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return Named<T>().Compile()(properties, name);
        }

        private static Expression<Func<IEnumerable<T>, String, T>> Named<T>()
           where T : PropertyValue
        {
            return (properties, name) => properties.FirstOrDefault(pv => pv.Name == name);
        }
    }
}
