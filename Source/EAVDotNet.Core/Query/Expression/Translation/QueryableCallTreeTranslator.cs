namespace JanHafner.EAVDotNet.Query.Expression.Translation
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Linq.Expressions;
    using JanHafner.EAVDotNet.Model;
    using JanHafner.EAVDotNet.Query.Expression.Inlining;
    using DynamicExpressionVisitor = JanHafner.EAVDotNet.Query.Expression.DynamicExpressionVisitor;

    [Export(typeof(IQueryableCallTreeTranslator))]
    internal sealed class QueryableCallTreeTranslator : DynamicExpressionVisitor, IQueryableCallTreeTranslator
    {
        private IQueryable<InstanceDescriptor> instances;

        private readonly InstanceDescriptorExpressionFactory instanceDescriptorExpressionFactory;

        private Type returnedExpressionInstanceType;

        [ImportingConstructor]
        public QueryableCallTreeTranslator(InstanceDescriptorExpressionFactory instanceDescriptorExpressionFactory)
        {
            this.instanceDescriptorExpressionFactory = instanceDescriptorExpressionFactory;
        }

        public Expression Translate(IQueryable<InstanceDescriptor> instances, Expression expression)
        {
            if (instances == null)
            {
                throw new ArgumentNullException(nameof(instances));
            }

            this.instances = instances;

            this.returnedExpressionInstanceType = ExtractExpressionInstanceType(expression);

            return this.Visit(expression);
        }

        private static Type ExtractExpressionInstanceType(Expression expression)
        {
            var callExpression = expression as MethodCallExpression;
            if (callExpression != null)
            {
                return ExtractExpressionInstanceType(callExpression.Arguments[0]);
            }

            return expression.Type.GetGenericArguments()[0];
        }

        protected override Expression VisitParameter(ParameterExpression parameterExpression)
        {
            return parameterExpression.Type != typeof(InstanceDescriptor) 
                ? Expression.Parameter(typeof (InstanceDescriptor)) 
                : base.VisitParameter(parameterExpression);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return this.instanceDescriptorExpressionFactory.CreateEntityFrameworkCompliantQuery(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Type.IsGenericType && node.Type.GetGenericTypeDefinition() == typeof(IQueryable<>))
            {
                return Expression.Constant(this.instances.OfType(this.returnedExpressionInstanceType));
            }

            return base.VisitConstant(node);
        }

        protected override Expression VisitQueryableExtensionMethodInvocation(MethodCallExpression methodCallExpression)
        {
            var genericMethodDefinition = methodCallExpression.Method.GetGenericMethodDefinition();

            var translatedArguments = methodCallExpression.Arguments.Select(this.Visit).ToList();

            var newGenericMethod = genericMethodDefinition.MakeGenericMethod(typeof(InstanceDescriptor));

            return Expression.Call(newGenericMethod, translatedArguments);
        }
    }
}
