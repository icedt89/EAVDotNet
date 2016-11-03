namespace JanHafner.EAVDotNet.Query.Expression.Translation
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq.Expressions;
    using JanHafner.EAVDotNet.Model;

    /// <summary>
    /// Translates: Expression<Func<Application, bool>> expression = e => e is ApplicationOne;
    /// to:         Expression<Func<InstanceDescriptor, bool>> expression = e => e.AssemblyQualifiedTypeName == typeof(ApplicationOne).AssemblyQualifiedName || e.Aliases.Any(a => a.AliasFullTypeName == typeof(ApplicationOne).AssemblyQualifiedName);
    /// </summary>
    [Export(typeof(ExpressionTranslator))]
    internal sealed class IsTypeExpressionTranslator : ExpressionTranslator
    {
        public override bool TryTranslate(ParameterExpression rootParameterExpression, Expression node, out Expression translatedExpression)
        {
            var typeBinaryExpression = node as TypeBinaryExpression;
            if (typeBinaryExpression == null || typeBinaryExpression.NodeType != ExpressionType.TypeIs)
            {
                translatedExpression = null;
                return false;
            }

            var typeToCheck = typeBinaryExpression.TypeOperand;

            var translatedIsTypeCheck = Expression.Property(Expression.Constant(typeToCheck), "AssemblyQualifiedName");

            var leftAssociatedTypeExpression = this.CreateExpressionForTypeDescriptor(rootParameterExpression, translatedIsTypeCheck);
            var rightAliasTypeExpression = this.CreateExpressionForTypeDescriptorAliases(rootParameterExpression, translatedIsTypeCheck);

            var fullTranslatedExpression = Expression.OrElse(leftAssociatedTypeExpression, rightAliasTypeExpression);

            translatedExpression = fullTranslatedExpression;
            return true;
        }

        private Expression CreateExpressionForTypeDescriptor(ParameterExpression rootParameterExpression, Expression translatedIsTypeCheck)
        {
            var translatedLeftOperand = Expression.Property(rootParameterExpression, "AssemblyQualifiedTypeName");

            return Expression.Equal(translatedLeftOperand, translatedIsTypeCheck);
        }

        private Expression CreateExpressionForTypeDescriptorAliases(ParameterExpression rootParameterExpression, Expression translatedIsTypeCheck)
        {
            var aliasParameterExpression = Expression.Parameter(typeof(TypeAlias));

            var translatedAliasLeftOperand = Expression.Property(aliasParameterExpression, "AssemblyQualifiedTypeName");

            var aliasTypeCheckEqualExpression = Expression.Equal(translatedAliasLeftOperand, translatedIsTypeCheck);
            var aliasTypeCheckLambdaExpression = Expression.Lambda<Func<TypeAlias, bool>>(aliasTypeCheckEqualExpression, aliasParameterExpression);
            
            var collectionPropertyExpression = Expression.Property(rootParameterExpression, "Aliases");

            var constructedLinqAnyMethodInfo = LinqAnyMethodInfo.MakeGenericMethod(typeof (TypeAlias));

            return Expression.Call( constructedLinqAnyMethodInfo, collectionPropertyExpression,
                aliasTypeCheckLambdaExpression);
        }
    }
}