namespace JanHafner.EAVDotNet.Query.Expression.Translation
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using JetBrains.Annotations;

    internal abstract class PropertyValueEqualityExpressionTranslator<TConcretePropertyValueType, TPropertyType> : ExpressionTranslator
    {
        [NotNull]
        private readonly Type concretePropertyValueType;

        [NotNull]
        private readonly Type propertyType;

        protected PropertyValueEqualityExpressionTranslator()
        {
            this.concretePropertyValueType = typeof(TConcretePropertyValueType);
            this.propertyType = typeof(TPropertyType);
        }

        public override bool TryTranslate(ParameterExpression rootParameterExpression, Expression node, out Expression translatedExpression)
        {
            translatedExpression = null;
            var binaryExpression = node as BinaryExpression;

            var leftMemberExpression = binaryExpression?.Left as MemberExpression;

            var propertyInfo = leftMemberExpression?.Member as PropertyInfo;
            if (propertyInfo == null || propertyInfo.PropertyType != this.propertyType)
            {
                return false;
            }

            translatedExpression = this.CreatePropertyValuesAnyExpression(rootParameterExpression, binaryExpression, propertyInfo);
            return true;
        }

        private Expression CreatePropertyValuesAnyExpression(ParameterExpression parameterExpression, BinaryExpression binaryExpression, PropertyInfo propertyInfo)
        {
            var constructedLinqOfTypeMethodInfo = EnumerableOfTypeMethodInfo.MakeGenericMethod(this.concretePropertyValueType);
            var ofTypeCall = Expression.Call(constructedLinqOfTypeMethodInfo, Expression.Property(parameterExpression, "PropertyValues"));

            var constructedLinqAnyMethodInfo = LinqAnyMethodInfo.MakeGenericMethod(this.concretePropertyValueType);

            var anyCallParameterExpression = Expression.Parameter(this.concretePropertyValueType);

            var checkExpression = this.CreatePropertyValueCheckExpression(anyCallParameterExpression, binaryExpression, propertyInfo);

            var lambda = Expression.Lambda(checkExpression, anyCallParameterExpression);

            return Expression.Call(constructedLinqAnyMethodInfo, ofTypeCall, lambda);
        }

        private Expression CreatePropertyValueCheckExpression(ParameterExpression parameterExpression, BinaryExpression binaryExpression, PropertyInfo propertyInfo)
        {
            var propertyNameCheckExpression = Expression.Equal(Expression.Property(parameterExpression, "Name"), Expression.Constant(propertyInfo.Name));

            var propertyTypeCheckExpression = Expression.Equal(Expression.Property(parameterExpression, "AssemblyQualifiedPropertyTypeName"), Expression.Constant(propertyInfo.PropertyType.AssemblyQualifiedName));

            var propertyValueCheckExpression = binaryExpression.Update(Expression.Property(parameterExpression, "Value"), binaryExpression.Conversion, binaryExpression.Right);

            return Expression.AndAlso(
                Expression.AndAlso(propertyNameCheckExpression,
                    propertyTypeCheckExpression), propertyValueCheckExpression);
        }
    }
}
