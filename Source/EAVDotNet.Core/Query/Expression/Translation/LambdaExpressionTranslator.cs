namespace JanHafner.EAVDotNet.Query.Expression.Translation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Linq.Expressions;
    using JetBrains.Annotations;
    using DynamicExpressionVisitor = JanHafner.EAVDotNet.Query.Expression.DynamicExpressionVisitor;

    [Export(typeof(LambdaExpressionTranslator))]
    internal sealed class LambdaExpressionTranslator : DynamicExpressionVisitor
    {
        [NotNull]
        private ParameterExpression rootParameterExpression;

        [NotNull]
        private readonly IEnumerable<ExpressionTranslator> expressionTranslaters;

        [ImportingConstructor]
        public LambdaExpressionTranslator([NotNull] [ImportMany(typeof(ExpressionTranslator))] IEnumerable<ExpressionTranslator> expressionTranslaters)
        {
            if (expressionTranslaters == null)
            {
                throw new ArgumentNullException(nameof(expressionTranslaters));
            }

            this.expressionTranslaters = expressionTranslaters;
        }

        public Expression TranslateExpression([NotNull] ParameterExpression rootParameterExpression, Expression expressionBody)
        {
            if (rootParameterExpression == null)
            {
                throw new ArgumentNullException(nameof(rootParameterExpression));
            }

            this.rootParameterExpression = rootParameterExpression;

            return this.Visit(expressionBody);
        }

        public override Expression Visit([NotNull] Expression node)
        {
            var result = base.Visit(node);

            Expression translatedResult = null;
            if(this.expressionTranslaters.Any(et => et.TryTranslate(this.rootParameterExpression, node, out translatedResult)))
            {
                result = translatedResult;
            }

            return result;
        }
    }
}