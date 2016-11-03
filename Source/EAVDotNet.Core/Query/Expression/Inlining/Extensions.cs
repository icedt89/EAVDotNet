namespace JanHafner.EAVDotNet.Query.Expression.Inlining
{
    using System;
    using System.Linq;
    using JanHafner.EAVDotNet.Model;
    using JetBrains.Annotations;

    /// <summary>
    /// Provides extension methods for the expression translation engine.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns all instances of <see cref="InstanceDescriptor"/> of the <see cref="Type"/> supplied by <typeparamref name="T"/>.
        /// Checks if the <see cref="Type"/> provided by <typeparamref name="T"/> is either available as associated type or alias.
        /// </summary>
        /// <typeparam name="T">The concrete <see cref="Type"/> to query for.</typeparam>
        /// <param name="instanceDescriptors">A list of <see cref="InstanceDescriptor"/> instances.</param>
        /// <exception cref="ArgumentNullException">The value of <paramref name="instanceDescriptors"/> cannot be null.</exception>
        /// <returns>The filtered list of <see cref="InstanceDescriptor"/> instances.</returns>
        [LinqTunnel]
        [NotNull]
        public static IQueryable<InstanceDescriptor> OfType<T>(
            [NotNull] this IQueryable<InstanceDescriptor> instanceDescriptors)
        {
            if (instanceDescriptors == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptors));
            }

            return instanceDescriptors.OfType(typeof (T));
        }

        /// <summary>
        /// Returns all instances of <see cref="InstanceDescriptor"/> of the <see cref="Type"/> supplied by <typeparamref name="T"/>.
        /// Checks if the <see cref="Type"/> provided by <paramref name="type"/> name="T"/> is either available as associated type or alias.
        /// </summary>
        /// <param name="instanceDescriptors">A list of <see cref="InstanceDescriptor"/> instances.</param>
        /// <param name="type">The concrete <see cref="Type"/> to query for.</param>
        /// <exception cref="ArgumentNullException">The value of <paramref name="instanceDescriptors"/> cannot be null.</exception>
        /// <returns>The filtered list of <see cref="InstanceDescriptor"/> instances.</returns>
        [LinqTunnel]
        [NotNull]
        internal static IQueryable<InstanceDescriptor> OfType([NotNull] this IQueryable<InstanceDescriptor> instanceDescriptors, Type type)
        {
            if (instanceDescriptors == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptors));
            }

            return instanceDescriptors.Where(id => id.AssemblyQualifiedTypeName == type.AssemblyQualifiedName || id.Aliases.Any(rt => rt.AssemblyQualifiedTypeName == type.AssemblyQualifiedName));
        }

        /// <summary>
        /// Returns all instances of <see cref="InstanceDescriptor"/> of the <see cref="Type"/> supplied by <typeparamref name="T"/>.
        /// Checks if the <see cref="Type"/> provided by <typeparamref name="T"/> is available as interface alias.
        /// </summary>
        /// <typeparam name="T">The concrete <see cref="Type"/> to query for.</typeparam>
        /// <param name="instanceDescriptors">A list of <see cref="InstanceDescriptor"/> instances.</param>
        /// <exception cref="ArgumentNullException">The value of <paramref name="instanceDescriptors"/> cannot be null.</exception>
        /// <returns>The filtered list of <see cref="InstanceDescriptor"/> instances.</returns>
        [LinqTunnel]
        [NotNull]
        public static IQueryable<InstanceDescriptor> Implements<T>(
            [NotNull] this IQueryable<InstanceDescriptor> instanceDescriptors)
        {
            if (instanceDescriptors == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptors));
            }

            return instanceDescriptors.Implements(typeof (T));
        }

        /// <summary>
        /// Returns all instances of <see cref="InstanceDescriptor"/> of the <see cref="Type"/> supplied by <typeparamref name="T"/>.
        /// Checks if the <see cref="Type"/> provided by <paramref name="type"/> is available as interface alias.
        /// </summary>
        /// <param name="type">The concrete <see cref="Type"/> to query for.</param>
        /// <param name="instanceDescriptors">A list of <see cref="InstanceDescriptor"/> instances.</param>
        /// <exception cref="ArgumentNullException">The value of <paramref name="instanceDescriptors"/> cannot be null.</exception>
        /// <returns>The filtered list of <see cref="InstanceDescriptor"/> instances.</returns>
        [LinqTunnel]
        [NotNull]
        internal static IQueryable<InstanceDescriptor> Implements(
            [NotNull] this IQueryable<InstanceDescriptor> instanceDescriptors, Type type)
        {
            if (instanceDescriptors == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptors));
            }

            return instanceDescriptors.Where(id => id.Aliases.Any(a => a.AssemblyQualifiedTypeName == type.AssemblyQualifiedName && a.IsInterface));
        }

        /// <summary>
        /// Returns all instances of <see cref="InstanceDescriptor"/> of the <see cref="Type"/> supplied by <typeparamref name="T"/>.
        /// Checks if the <see cref="Type"/> provided by <typeparamref name="T"/> is available as base type alias.
        /// </summary>
        /// <typeparam name="T">The concrete <see cref="Type"/> to query for.</typeparam>
        /// <param name="instanceDescriptors">A list of <see cref="InstanceDescriptor"/> instances.</param>
        /// <exception cref="ArgumentNullException">The value of <paramref name="instanceDescriptors"/> cannot be null.</exception>
        /// <returns>The filtered list of <see cref="InstanceDescriptor"/> instances.</returns>
        [LinqTunnel]
        [NotNull]
        public static IQueryable<InstanceDescriptor> BasedOn<T>(
            [NotNull] this IQueryable<InstanceDescriptor> instanceDescriptors)
        {
            if (instanceDescriptors == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptors));
            }

            return instanceDescriptors.BasedOn(typeof (T));
        }

        /// <summary>
        /// Returns all instances of <see cref="InstanceDescriptor"/> of the <see cref="Type"/> supplied by <typeparamref name="T"/>.
        /// Checks if the <see cref="Type"/> provided by <paramref name="type"/> is available as base type alias.
        /// </summary>
        /// <param name="type">The concrete <see cref="Type"/> to query for.</param>
        /// <param name="instanceDescriptors">A list of <see cref="InstanceDescriptor"/> instances.</param>
        /// <exception cref="ArgumentNullException">The value of <paramref name="instanceDescriptors"/> cannot be null.</exception>
        /// <returns>The filtered list of <see cref="InstanceDescriptor"/> instances.</returns>
        [LinqTunnel]
        [NotNull]
        internal static IQueryable<InstanceDescriptor> BasedOn([NotNull] this IQueryable<InstanceDescriptor> instanceDescriptors, Type type)
        {
            if (instanceDescriptors == null)
            {
                throw new ArgumentNullException(nameof(instanceDescriptors));
            }

            return instanceDescriptors.Where(id => id.Aliases.Any(a => a.AssemblyQualifiedTypeName == type.AssemblyQualifiedName && !a.IsInterface));
        }
    }
}
