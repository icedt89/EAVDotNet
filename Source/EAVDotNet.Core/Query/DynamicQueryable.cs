namespace JanHafner.EAVDotNet.Query
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using JetBrains.Annotations;

    /// <summary>
    /// Provides an <see cref="IQueryable{T}"/> implementation which operates with the <see cref="DynamicQueryProvider"/> implementation.
    /// </summary>
    /// <typeparam name="T">The type of objects to dematerialize.</typeparam>
    internal sealed class DynamicQueryable<T> : IDynamicQueryable<T>
    {
        [NotNull]
        private readonly IQueryProvider queryProvider;

        [NotNull]
        private readonly System.Linq.Expressions.Expression expression;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicQueryable{T}"/> class.
        /// </summary>
        /// <param name="queryProvider">The <see cref="IQueryProvider"/>.</param>
        /// <param name="expression">The <see cref="Expression"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="expression"/>' and '<paramref name="queryProvider"/>' cannot be null. The <paramref name="expression"/> is not assignable to an <see cref="IQueryable{T}"/>.</exception>
        public DynamicQueryable([NotNull] IQueryProvider queryProvider, [NotNull] System.Linq.Expressions.Expression expression)
        {
            if (queryProvider == null)
            {
                throw new ArgumentNullException(nameof(queryProvider));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentException("expression");
            }

            this.queryProvider = queryProvider;
            this.expression = expression;
        }

        /// <summary>
        /// Disables change tracking.
        /// </summary>
        /// <returns>A new query.</returns>
        public IDynamicQueryable<T> AsNoTracking()
        {
            var dynamicQueryProvider = this.queryProvider as DynamicQueryProvider<T>;
            dynamicQueryProvider?.NoChangeTracking();

            return this;
        }

        /// <summary>
        /// Disables lazy loading.
        /// </summary>
        /// <returns>A new query.</returns>
        public IDynamicQueryable<T> WithoutLazyLoading()
        {
            var dynamicQueryProvider = this.queryProvider as DynamicQueryProvider<T>;
            dynamicQueryProvider?.NoLazyLoading();

            return this;
        }

        /// <summary>
        /// Ruft die Ausdrucksbaumstruktur ab, die mit der Instanz von <see cref="T:System.Linq.IQueryable"/> verknüpft ist.
        /// </summary>
        /// <returns>
        /// Der <see cref="T:System.Linq.Expressions.Expression"/>, der mit dieser Instanz von <see cref="T:System.Linq.IQueryable"/> verknüpft ist.
        /// </returns>
        System.Linq.Expressions.Expression IQueryable.Expression
        {
            get
            {
                return this.expression;
            }
        }

        /// <summary>
        /// Ruft den Typ der Elemente ab, die zurückgegeben werden, wenn die Ausdrucksbaumstruktur ausgeführt wird, die mit dieser Instanz von <see cref="T:System.Linq.IQueryable"/> verknüpft ist.
        /// </summary>
        /// <returns>
        /// Ein <see cref="T:System.Type"/>, der den Typ der Elemente darstellt, die zurückgegeben werden, wenn die Ausdrucksbaumstruktur ausgeführt wird, die mit diesem Objekt verknüpft ist.
        /// </returns>
        Type IQueryable.ElementType
        {
            get
            {
                return typeof (T);
            }
        }

        /// <summary>
        /// Ruft den Abfrageanbieter ab, der dieser Datenquelle zugeordnet ist.
        /// </summary>
        /// <returns>
        /// Der <see cref="T:System.Linq.IQueryProvider"/>, der dieser Datenquelle zugeordnet ist.
        /// </returns>
        IQueryProvider IQueryable.Provider
        {
            get
            {
                return this.queryProvider;
            }
        }
        

        /// <summary>
        /// Gibt einen Enumerator zurück, der die Auflistung durchläuft.
        /// </summary>
        /// <returns>
        /// Ein <see cref="T:System.Collections.Generic.IEnumerator`1"/>, der zum Durchlaufen der Auflistung verwendet werden kann.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            var result = ((IEnumerable)this.queryProvider.Execute(this.expression)).GetEnumerator();

            while (result.MoveNext())
            {
                yield return (T)result.Current;
            }
        }

        /// <summary>
        /// Gibt einen Enumerator zurück, der eine Auflistung durchläuft.
        /// </summary>
        /// <returns>
        /// Ein <see cref="T:System.Collections.IEnumerator"/>-Objekt, das zum Durchlaufen der Auflistung verwendet werden kann.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.queryProvider.Execute(this.expression)).GetEnumerator();
        }
    }
}