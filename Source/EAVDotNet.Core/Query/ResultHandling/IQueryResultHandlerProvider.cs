namespace JanHafner.EAVDotNet.Query.ResultHandling
{
    using System;
    using JetBrains.Annotations;

    /// <summary>
    /// The implementation finds the <see cref="IQueryResultHandler"/> suitable for the processing the result, and returns the result of the operation.
    /// </summary>
    public interface IQueryResultHandlerProvider
    {
        /// <summary>
        /// Processes the supplied <paramref name="queryResult"/> from the data source.
        /// </summary>
        /// <param name="resultHandlerContext"></param>
        /// <returns>The real result of the query.</returns>
        [CanBeNull]
        Object CreateResult([NotNull] ResultHandlerContext resultHandlerContext);
    }
}