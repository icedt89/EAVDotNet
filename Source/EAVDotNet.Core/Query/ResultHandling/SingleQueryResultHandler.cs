﻿namespace JanHafner.EAVDotNet.Query.ResultHandling
{
    using System;
    using System.ComponentModel.Composition;
    using JanHafner.EAVDotNet.Instanciation;
    using JanHafner.EAVDotNet.Model;
    using JetBrains.Annotations;

    /// <summary>
    /// Handles an <see cref="InstanceDescriptor"/> instances returned from the data source.
    /// </summary>
    [Export(typeof(IQueryResultHandler))]
    internal sealed class SingleQueryResultHandler : IQueryResultHandler
    {
        [NotNull]
        private readonly ExportFactory<IInstanceResolutionWalker> instanceResolutionWalkerFactory;

        /// <summary>
        /// Initializes a new istance of the <see cref="EnumerableQueryResultHandler"/> class.
        /// </summary>
        /// <param name="instanceResolutionWalkerFactory">The <see cref="IInstanceResolutionWalker"/> factory.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instanceResolutionWalkerFactory"/>' cannot be null.</exception>
        [ImportingConstructor]
        public SingleQueryResultHandler(
            [NotNull] ExportFactory<IInstanceResolutionWalker> instanceResolutionWalkerFactory)
        {
            if (instanceResolutionWalkerFactory == null)
            {
                throw new ArgumentNullException(nameof(instanceResolutionWalkerFactory));
            }

            this.instanceResolutionWalkerFactory = instanceResolutionWalkerFactory;
        }

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

            var instanceDescriptor = resultHandlerContext.Result as InstanceDescriptor;
            if (instanceDescriptor != null)
            {
                using (var instanceResolutionWalker = this.instanceResolutionWalkerFactory.CreateExport())
                {
                    processResult = instanceResolutionWalker.Value.ResolveInstance(new InstanceResolutionContext(instanceDescriptor, resultHandlerContext.InstanceRelationStore)
                    {
                        NoChangeTracking = resultHandlerContext.NoChangeTracking,
                        NoLazyLoading = resultHandlerContext.NoLazyLoading
                    });
                }
            }

            return instanceDescriptor != null;
        }
    }
}
