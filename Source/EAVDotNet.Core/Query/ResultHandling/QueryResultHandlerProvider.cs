namespace JanHafner.EAVDotNet.Query.ResultHandling
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using JetBrains.Annotations;

    /// <summary>
    /// Finds the <see cref="IQueryResultHandler"/> suitable for the processing the result, and returns the result of the operation.
    /// </summary>
    [Export(typeof(IQueryResultHandlerProvider))]
    internal sealed class QueryResultHandlerProvider : IQueryResultHandlerProvider
    {
        [NotNull]
        private readonly IEnumerable<IQueryResultHandler> queryResultHandlers;

        /// <summary>
        /// Initializes a new istance of the <see cref="QueryResultHandlerProvider"/> class.
        /// </summary>
        /// <param name="queryResultHandlers">The list of <see cref="IQueryResultHandler"/> instances.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="queryResultHandlers"/>' cannot be null.</exception>
        [ImportingConstructor]
        public QueryResultHandlerProvider([NotNull] [ImportMany(typeof(IQueryResultHandler))] IEnumerable<IQueryResultHandler> queryResultHandlers)
        {
            if (queryResultHandlers == null)
            {
                throw new ArgumentNullException(nameof(queryResultHandlers));
            }

            this.queryResultHandlers = queryResultHandlers;
        }

        /// <summary>
        /// Processes the supplied <paramref name="queryResult"/> from the data source.
        /// </summary>
        /// <param name="resultHandlerContext"></param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="resultHandlerContext"/>' cannot be null.</exception>
        /// <returns>The real result of the query.</returns>
        public Object CreateResult(ResultHandlerContext resultHandlerContext)
        {
            if (resultHandlerContext == null)
            {
                throw new ArgumentNullException(nameof(resultHandlerContext));
            }

            Object result = null;
            foreach (var queryResultHandler in this.queryResultHandlers)
            {
                if (queryResultHandler.TryProcessQueryResult(resultHandlerContext, out result))
                {
                    break;
                }
            }

            return result;
        }
    }
}
