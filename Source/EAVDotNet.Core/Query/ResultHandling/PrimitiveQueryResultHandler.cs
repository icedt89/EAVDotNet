namespace JanHafner.EAVDotNet.Query.ResultHandling
{
    using System;
    using System.ComponentModel.Composition;
    using JanHafner.EAVDotNet.Classification;

    /// <summary>
    /// Handles a primitive value returned from the datasource. Probably the result of a Max() or something.
    /// </summary>
    [Export(typeof(IQueryResultHandler))]
    internal sealed class PrimitiveQueryResultHandler : IQueryResultHandler
    {
        /// <summary>
        /// Processes the supplied <paramref name="queryResult"/> from the data source.
        /// </summary>
        /// <param name="resultHandlerContext"></param>
        /// <param name="processResult">The real result of the execution operation.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="resultHandlerContext"/>' cannot be null.</exception>
        /// <returns>A value indicating, whether, this handler handled the <paramref name="queryResult"/>.</returns>
        public Boolean TryProcessQueryResult(ResultHandlerContext resultHandlerContext, out Object processResult)
        {
            if (resultHandlerContext == null)
            {
                throw new ArgumentNullException(nameof(resultHandlerContext));
            }

            processResult = null;
            if (resultHandlerContext.Result == null)
            {
                return false;
            }

            var queryResultClassification = resultHandlerContext.Result.GetType().Classify();
            if (queryResultClassification == TypeClassifier.TypeClassification.Primitive)
            {
                processResult = resultHandlerContext.Result;
            }

            return queryResultClassification == TypeClassifier.TypeClassification.Primitive;
        }
    }
}