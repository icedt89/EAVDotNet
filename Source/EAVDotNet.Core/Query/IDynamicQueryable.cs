namespace JanHafner.EAVDotNet.Query
{
    using System;
    using System.Linq;

    /// <summary>
    /// Defines a specialized <see cref="IQueryable"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the entity.</typeparam>
    internal interface IDynamicQueryable<out T> : IOrderedQueryable<T>
    {
        /// <summary>
        /// Disables change tracking.
        /// </summary>
        /// <returns>A new query.</returns>
        IDynamicQueryable<T> AsNoTracking();

        /// <summary>
        /// Disables lazy loading.
        /// </summary>
        /// <returns>A new query.</returns>
        IDynamicQueryable<T> WithoutLazyLoading();
    }
}
