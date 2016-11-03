namespace JanHafner.EAVDotNet.Query.Expression.Translation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    [Obsolete("Komplett refactoren!")]
    public abstract class ExpressionTranslator
    {
        private static readonly Lazy<MethodInfo> linqAnyMethodInfo;
        public static MethodInfo LinqAnyMethodInfo
        {
            get { return linqAnyMethodInfo.Value; }
        }

        private static readonly Lazy<MethodInfo> enumerableOfTypeMethodInfo;
        public static MethodInfo EnumerableOfTypeMethodInfo
        {
            get { return enumerableOfTypeMethodInfo.Value; }
        }

        static ExpressionTranslator ()
        {
            linqAnyMethodInfo = new Lazy<MethodInfo>(() =>
            {
                const String AnyMethodName = "Any";
                var enumerableClassType = typeof (Enumerable);

                var publicStaticMethodsNamedAny = enumerableClassType.GetMethods(BindingFlags.Static | BindingFlags.Public).Where(m => m.Name == AnyMethodName);

                var desiredAnyMethod = publicStaticMethodsNamedAny.First(m =>
                {
                    if (!m.IsGenericMethodDefinition)
                    {
                        return false;
                    }

                    var parameters = m.GetParameters();
                    return parameters.Length == 2 &&
                           (parameters[0].ParameterType.GetGenericTypeDefinition() == typeof (IEnumerable<>) &&
                            parameters[1].ParameterType.GetGenericTypeDefinition() == typeof (Func<,>));
                });

                return desiredAnyMethod;
            });

            enumerableOfTypeMethodInfo = new Lazy<MethodInfo>(() =>
            {
                const String OfTypeMethodName = "OfType";
                var enumerableClassType = typeof(Enumerable);

                var publicStaticMethodsNamedOfType = enumerableClassType.GetMethods(BindingFlags.Static | BindingFlags.Public).Where(m => m.Name == OfTypeMethodName);

                var desiredAnyMethod = publicStaticMethodsNamedOfType.First(m =>
                {
                    if (!m.IsGenericMethodDefinition)
                    {
                        return false;
                    }

                    var parameters = m.GetParameters();
                    if (parameters.Length != 1)
                    {
                        return false;
                    }

                    return parameters[0].ParameterType == typeof(IEnumerable);
                });

                return desiredAnyMethod;
            });
        }

        public abstract bool TryTranslate(ParameterExpression rootParameterExpression, Expression node, out Expression translatedExpression);
    }
}
