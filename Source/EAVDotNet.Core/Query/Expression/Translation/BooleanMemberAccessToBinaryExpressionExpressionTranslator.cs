namespace JanHafner.EAVDotNet.Query.Expression.Translation
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq.Expressions;
    using System.Reflection;

    [Export(typeof(ExpressionTranslator))]
    internal sealed class BooleanMemberAccessToBinaryExpressionExpressionTranslator : ExpressionTranslator
    {
        public override bool TryTranslate(ParameterExpression rootParameterExpression, Expression node, out Expression translatedExpression)
        {
            translatedExpression = null;
            var memberExpression = node as MemberExpression;

            var propertyInfo = memberExpression?.Member as PropertyInfo;
            if (propertyInfo == null || propertyInfo.PropertyType != typeof(Boolean))
            {
                return false;
            }

            var booleanBinaryExpression = this.CreateBooleanBinaryExpressionFromBooleanPropertyAccess(memberExpression);
            return new BooleanPropertyValueEqualityExpressionTranslator().TryTranslate(rootParameterExpression, booleanBinaryExpression, out translatedExpression);
        }

        private BinaryExpression CreateBooleanBinaryExpressionFromBooleanPropertyAccess(MemberExpression booleanMemberExpression)
        {
            var rightSidedExpression = Expression.Constant(true);

            return Expression.Equal(booleanMemberExpression, rightSidedExpression);
        }
    }
}