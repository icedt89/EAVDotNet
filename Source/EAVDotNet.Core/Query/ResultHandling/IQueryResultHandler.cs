namespace JanHafner.EAVDotNet.Query.ResultHandling
{
    using System;
    using JetBrains.Annotations;

    /// <summary>
    /// the implementation handles the result of the execution of the <see cref="DynamicQueryProvider"/>.
    /// </summary>
    internal interface IQueryResultHandler
    {
        /// <summary>
        /// Processes the supplied <paramref name="queryResult"/> from the data source.
        /// </summary>
        /// <param name="resultHandlerContext"></param>
        /// <param name="processResult">The real result of the execution operation.</param>
        /// <returns>A value indicating, whether, this handler handled the <paramref name="queryResult"/>.</returns>
        Boolean TryProcessQueryResult([NotNull] ResultHandlerContext resultHandlerContext, [CanBeNull] out Object processResult);
    }
}