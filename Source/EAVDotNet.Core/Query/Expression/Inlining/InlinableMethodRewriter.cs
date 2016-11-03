namespace JanHafner.EAVDotNet.Query.Expression.Inlining
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using JetBrains.Annotations;
    using DynamicExpressionVisitor = JanHafner.EAVDotNet.Query.Expression.DynamicExpressionVisitor;

    /// <summary>
    /// When detecting an inlinable method, the method call will be inlined by the corresponding inlining method.
    /// </summary>
    internal sealed class InlinableMethodRewriter : DynamicExpressionVisitor
    {
        [CanBeNull]
        private readonly ParameterExpression originalParameterExpression;

        [CanBeNull]
        private readonly Expression replacementExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="InlinableMethodRewriter"/> class.
        /// </summary>
        public InlinableMethodRewriter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InlinableMethodRewriter"/> class.
        /// </summary>
        /// <param name="originalParameterExpression">The original <see cref="ParameterExpression"/>.</param>
        /// <param name="replacementExpression">The substituting <see cref="Expression"/>.</param>
        private InlinableMethodRewriter([NotNull] ParameterExpression originalParameterExpression, [NotNull] Expression replacementExpression)
        {
            if (originalParameterExpression == null)
            {
                throw new ArgumentNullException(nameof(originalParameterExpression));
            }

            if (replacementExpression == null)
            {
                throw new ArgumentNullException(nameof(replacementExpression));
            }

            this.originalParameterExpression = originalParameterExpression;
            this.replacementExpression = replacementExpression;
        }

        /// <summary>
        /// Visits a <see cref="MethodCallExpression"/> which is classified as an inlineable extension method.
        /// </summary>
        /// <param name="methodCallExpression">The <see cref="MemberExpression"/></param>
        /// <param name="inlinableMethod">The method for inlining.</param>
        protected override Expression VisitInlineableExtensionMethodInvocation(MethodCallExpression methodCallExpression, MethodInfo inlinableMethod)
        {
            var inlineableLambdaExpression = Expression.Lambda<Func<LambdaExpression>>(Expression.Call(inlinableMethod)).Compile()();

            var parameterBinders = inlineableLambdaExpression.Parameters.Zip(methodCallExpression.Arguments, (parameterExpression, expression) => new InlinableMethodRewriter(parameterExpression, expression));

            var inlinedMethodCallExpression = this.Visit(parameterBinders.Aggregate(inlineableLambdaExpression.Body, (expression, inlinableMethodRewriter) => inlinableMethodRewriter.Visit(expression)));

            return inlinedMethodCallExpression;
        }

        /// <inheritdoc />
        protected override Expression VisitParameter(ParameterExpression parameterExpression)
        {
            if (parameterExpression == this.originalParameterExpression && this.replacementExpression != null)
            {
                return this.replacementExpression;
            }

            return base.VisitParameter(parameterExpression);
        }

        /// <inheritdoc />
        protected override Expression VisitInvocation(InvocationExpression invocationExpression)
        {
            var lambdaExpression = this.replacementExpression as LambdaExpression;
            if (invocationExpression == null || invocationExpression.Expression != this.originalParameterExpression || lambdaExpression == null)
            {
                return base.VisitInvocation(invocationExpression);
            }

            return lambdaExpression.Parameters.Zip(invocationExpression.Arguments, (p, a) => new InlinableMethodRewriter(p, a)).Aggregate(lambdaExpression.Body, (e, b) => b.Visit(e));
        }
    }
}
