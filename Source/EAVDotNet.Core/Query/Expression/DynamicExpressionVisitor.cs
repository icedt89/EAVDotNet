namespace JanHafner.EAVDotNet.Query.Expression
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using JanHafner.EAVDotNet.Classification;
    using JanHafner.EAVDotNet.Query.Expression.Inlining;
    using JetBrains.Annotations;

    /// <summary>
    /// Extends the <see cref="ExpressionVisitor"/> by providing more specialized Visit-methods for this project.
    /// </summary>
    internal abstract class DynamicExpressionVisitor : ExpressionVisitor
    {
        [NotNull]
        private static readonly ConcurrentDictionary<MethodInfo, MethodInfo> CorrespondingMethodInfoCache;

        /// <summary>
        /// Initializes the static instance of the <see cref="DynamicExpressionVisitor"/> class.
        /// </summary>
        static DynamicExpressionVisitor()
        {
            CorrespondingMethodInfoCache = new ConcurrentDictionary<MethodInfo, MethodInfo>();
        }

        /// <inheritdoc />
        protected override Expression VisitMember(MemberExpression node)
        {
            var info = node.Member as PropertyInfo;
            if (info == null)
            {
                return base.VisitMember(node);
            }

            var classification = info.PropertyType.Classify();
            switch (classification)
            {
                case TypeClassifier.TypeClassification.Collection:
                    return this.VisitCollectionAccess(node);
                case TypeClassifier.TypeClassification.Complex:
                    return this.VisitComplexPropertyAccess(node);
                case TypeClassifier.TypeClassification.Primitive:
                    return this.VisitPrimitivePropertyAccess(node);
                default:
                    return base.VisitMember(node);
            }
        }

        /// <summary>
        /// Returns a value indicating, whether, the supplied <see cref="MethodInfo"/> <paramref name="methodOne"/> does look a like the supplied <see cref="MethodInfo"/> <paramref name="methodTwo"/>.
        /// </summary>
        /// <param name="methodOne">The first <see cref="MethodInfo"/>.</param>
        /// <param name="methodTwo">The second <see cref="MethodInfo"/>.</param>
        /// <returns>A value indicating, whether, the method infos look the same.</returns>
        private static Boolean LookALike(MethodInfo methodOne, MethodInfo methodTwo)
        {
            return !methodOne.IsGenericMethod || !methodTwo.IsGenericMethod
                ? methodOne == methodTwo
                : methodTwo.GetGenericMethodDefinition() == methodOne.GetGenericMethodDefinition();
        }

        /// <inheritdoc />
        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            if (typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static).Any(mi => LookALike(mi, methodCallExpression.Method)))
            {
                return this.VisitQueryableExtensionMethodInvocation(methodCallExpression);
            }

            if (typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static).Any(mi => LookALike(mi, methodCallExpression.Method)))
            {
                return this.VisitEnumerableExtensionMethodInvocation(methodCallExpression);
            }

            MethodInfo correspondingLambdaExpression = null;
            return typeof (InlinableExpression).GetMethods(BindingFlags.Public |
                                                           BindingFlags.Static)
                .Any(
                    mi =>
                        LookALike(mi, methodCallExpression.Method) && this.TryGetCorrespondingLambdaExpression(methodCallExpression.Method, out correspondingLambdaExpression)) 
                        ? this.VisitInlineableExtensionMethodInvocation(methodCallExpression, correspondingLambdaExpression) 
                        : base.VisitMethodCall(methodCallExpression);
        }

        /// <summary>
        /// Tries to find the method which should be used for inlining for the supplied <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="methodInfo">The source <see cref="MethodInfo"/>.</param>
        /// <param name="correspondingLambdaExpression">The <see cref="MethodInfo"/> for inlining.</param>
        /// <returns></returns>
        private Boolean TryGetCorrespondingLambdaExpression(MethodInfo methodInfo, out MethodInfo correspondingLambdaExpression)
        {
            var result = CorrespondingMethodInfoCache.GetOrAdd(methodInfo, _ =>
            {
                var correspondingLookALikes = _.DeclaringType.GetMethods(BindingFlags.NonPublic |
                                                                         BindingFlags.Static)
                    .Where(
                        mi =>
                            mi.Name == _.Name &&
                            mi.ReturnType.IsGenericType &&
                            mi.ReturnType.GetGenericTypeDefinition() == typeof (Expression<>));

                var methodInfoComparableParameters = _.GetParameters().Select(m => m.ParameterType).ToList();
                methodInfoComparableParameters.Add(_.ReturnType);
                foreach (var correspondingLookALike in correspondingLookALikes)
                {
                    var correspondingMethod = correspondingLookALike;
                    if (correspondingLookALike.IsGenericMethod)
                    {
                        correspondingMethod = correspondingLookALike.MakeGenericMethod(_.GetGenericArguments());
                    }

                    var correspondingLookALikeComparableParameters =
                        correspondingMethod.ReturnType.GetGenericArguments()[0].GetGenericArguments();

                    if (methodInfoComparableParameters.SequenceEqual(correspondingLookALikeComparableParameters))
                    {
                        return correspondingMethod;
                    }
                }

                return null;
            });

            correspondingLambdaExpression = result;
            return result != null;
        }

        /// <summary>
        /// Visits a <see cref="MemberExpression"/> which is classified as primitive.
        /// </summary>
        /// <param name="memberExpression">The <see cref="MemberExpression"/></param>
        protected virtual Expression VisitPrimitivePropertyAccess(
            MemberExpression memberExpression)
        {
            return base.VisitMember(memberExpression);
        }

        /// <summary>
        /// Visits a <see cref="MemberExpression"/> which is classified as complex.
        /// </summary>
        /// <param name="memberExpression">The <see cref="MemberExpression"/></param>
        protected virtual Expression VisitComplexPropertyAccess(
            MemberExpression memberExpression)
        {
            return base.VisitMember(memberExpression);
        }

        /// <summary>
        /// Visits a <see cref="MemberExpression"/> which is classified as collection.
        /// </summary>
        /// <param name="memberExpression">The <see cref="MemberExpression"/></param>
        protected virtual Expression VisitCollectionAccess(
            MemberExpression memberExpression)
        {
            return base.VisitMember(memberExpression);
        }

        /// <summary>
        /// Visits a <see cref="MethodCallExpression"/> which is classified as an extension method of an <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <param name="methodCallExpression">The <see cref="MemberExpression"/></param>
        protected virtual Expression VisitQueryableExtensionMethodInvocation(MethodCallExpression methodCallExpression)
        {
            return base.VisitMethodCall(methodCallExpression);
        }

        /// <summary>
        /// Visits a <see cref="MethodCallExpression"/> which is classified as an inlineable extension method.
        /// </summary>
        /// <param name="methodCallExpression">The <see cref="MemberExpression"/></param>
        /// <param name="inlinableMethod">The method for inlining.</param>
        protected virtual Expression VisitInlineableExtensionMethodInvocation(MethodCallExpression methodCallExpression, MethodInfo inlinableMethod)
        {
            return base.VisitMethodCall(methodCallExpression);
        }

        /// <summary>
        /// Visits a <see cref="MethodCallExpression"/> which is classified as an extension method of an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="methodCallExpression">The <see cref="MemberExpression"/></param>
        protected virtual Expression VisitEnumerableExtensionMethodInvocation(MethodCallExpression methodCallExpression)
        {
            return base.VisitMethodCall(methodCallExpression);
        }
    }
}
