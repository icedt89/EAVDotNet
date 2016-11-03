namespace JanHafner.EAVDotNet.Query.Expression.Translation
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq.Expressions;
    using JanHafner.EAVDotNet.Model;
    using JetBrains.Annotations;

    /// <summary>
    /// Provides 
    /// </summary>
    [Export(typeof(InstanceDescriptorExpressionFactory))]
    internal sealed class InstanceDescriptorExpressionFactory
    {
        [NotNull]
        private readonly LambdaExpressionTranslator expressionTranslatorVisitor;

        [ImportingConstructor]
        public InstanceDescriptorExpressionFactory([NotNull] LambdaExpressionTranslator expressionTranslatorVisitor)
        {
            if (expressionTranslatorVisitor == null)
            {
                throw new ArgumentNullException(nameof(expressionTranslatorVisitor));
            }

            this.expressionTranslatorVisitor = expressionTranslatorVisitor;
        }

        [NotNull]
        public Expression<Func<InstanceDescriptor, Boolean>> CreateEntityFrameworkCompliantQuery<T>(
            [NotNull] Expression<Func<T, Boolean>> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return this.CreateEntityFrameworkCompliantQuery((LambdaExpression)query);
        }

        [NotNull]
        public Expression<Func<InstanceDescriptor, Boolean>> CreateEntityFrameworkCompliantQuery(
            [NotNull] LambdaExpression lambdaExpression)
        {
            if (lambdaExpression == null)
            {
                throw new ArgumentNullException(nameof(lambdaExpression));
            }

            var instanceDescriptorExpressionParameter = Expression.Parameter(typeof (InstanceDescriptor));

            var newBody = this.expressionTranslatorVisitor.TranslateExpression(instanceDescriptorExpressionParameter, lambdaExpression.Body);

            return Expression.Lambda<Func<InstanceDescriptor, Boolean>>(newBody, instanceDescriptorExpressionParameter);
        }
    }
}
