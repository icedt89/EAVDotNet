namespace JanHafner.EAVDotNet.Query
{
    using System;
    using System.Linq;
    using JetBrains.Annotations;

    /// <summary>
    /// Defines a specialized <see cref="IQueryProvider"/>.
    /// </summary>
    internal interface IDynamicQueryProvider<TBasedOnEntityType> : IDynamicQueryProvider
    {
    }

    /// <summary>
    /// Defines a specialized <see cref="IQueryProvider"/>.
    /// </summary>
    internal interface IDynamicQueryProvider : IQueryProvider
    {
        /// <summary>
        /// Dematerializes the supplied object to the <see cref="Type"/> defined by <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to return.</typeparam>
        /// <param name="source">The object to dematerialize.</param>
        /// <returns>The dematerialized result.</returns>
        [CanBeNull]
        T Dematerialize<T>([NotNull] Object source);

        /// <summary>
        /// The entity type the <see cref="IDynamicQueryProvider"/> was initially created for.
        /// </summary>
        Type BasedOnEntityType { get; }

        /// <summary>
        /// Dissallows change tracking proxy creation for all instances created by this <see cref="IDynamicQueryProvider"/> after a call to this method.
        /// </summary>
        /// <returns>The same instance.</returns>
        IDynamicQueryProvider NoChangeTracking();

        /// <summary>
        /// Dissallows lazy loading proxy creation for all instances created by this <see cref="IDynamicQueryProvider"/> after a call to this method.
        /// </summary>
        /// <returns>The same instance.</returns>
        IDynamicQueryProvider NoLazyLoading();
    }
}
