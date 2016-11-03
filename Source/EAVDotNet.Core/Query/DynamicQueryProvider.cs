namespace JanHafner.EAVDotNet.Query
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using JanHafner.EAVDotNet.Context;
    using JanHafner.EAVDotNet.Context.InstanceRelation;
    using JanHafner.EAVDotNet.Model;
    using JanHafner.EAVDotNet.Query.Expression.Inlining;
    using JanHafner.EAVDotNet.Query.Expression.Translation;
    using JanHafner.EAVDotNet.Query.ResultHandling;
    using JetBrains.Annotations;

    /// <summary>
    /// Provides an <see cref="IQueryProvider"/> implementation which translates queries to entity framework queries.
    /// </summary>
    internal class DynamicQueryProvider<TCreatedFromEntity> : IDynamicQueryProvider<TCreatedFromEntity>
    {
        [NotNull]
        private readonly IQueryable<InstanceDescriptor> instances;

        [NotNull]
        private readonly IInstanceRelationStore instanceRelationStore;

        [NotNull]
        private readonly IQueryResultHandlerProvider queryResultHandlerProvider;

        [NotNull]
        private readonly IQueryableCallTreeTranslator queryableCallTreeTranslator;

        private Boolean noChangeTracking;

        private Boolean noLazyLoading;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicQueryProvider"/> class.
        /// </summary>
        /// <param name="instances">The associated <see cref="IDynamicDbContext"/>.</param>
        /// <param name="instanceRelationStore">The <see cref="IInstanceRelationStore"/>.</param>
        /// <param name="queryResultHandlerProvider">The <see cref="IQueryResultHandlerProvider"/>.</param>
        /// <param name="queryableCallTreeTranslator">The <see cref="QueryableCallTreeTranslator"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="instanceRelationStore"/>', '<paramref name="queryResultHandlerProvider"/>', '<paramref name="instances"/>', '<paramref name="queryableCallTreeTranslator"/>' cannot be null.</exception>
        public DynamicQueryProvider([NotNull] IQueryable<InstanceDescriptor> instances, [NotNull] IInstanceRelationStore instanceRelationStore, [NotNull] IQueryResultHandlerProvider queryResultHandlerProvider, [NotNull] IQueryableCallTreeTranslator queryableCallTreeTranslator)
        {
            if (instances == null)
            {
                throw new ArgumentNullException(nameof(instances));
            }
            
            if (instanceRelationStore == null)
            {
                throw new ArgumentNullException(nameof(instanceRelationStore));
            }

            if (queryResultHandlerProvider == null)
            {
                throw new ArgumentNullException(nameof(queryResultHandlerProvider));
            }

            if (queryableCallTreeTranslator == null)
            {
                throw new ArgumentNullException(nameof(queryableCallTreeTranslator));
            }

            this.instances = instances;
            this.instanceRelationStore = instanceRelationStore;
            this.queryResultHandlerProvider = queryResultHandlerProvider;
            this.queryableCallTreeTranslator = queryableCallTreeTranslator;
        }

        /// <summary>
        /// Konstruiert ein <see cref="T:System.Linq.IQueryable`1"/>-Objekt, das die Abfrage auswerten kann, die von einer angegebenen Ausdrucksbaumstruktur dargestellt wird.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="expression"/>' cannot be null.</exception>
        /// <returns>
        /// Ein <see cref="T:System.Linq.IQueryable`1"/>-Objekt, das die Abfrage auswerten kann, die von der angegebenen Ausdrucksbaumstruktur dargestellt wird.
        /// </returns>
        /// <param name="expression">Eine Ausdrucksbaumstruktur, die eine LINQ-Abfrage darstellt.</param><typeparam name="T">Der Typ der Elemente des <see cref="T:System.Linq.IQueryable`1"/>-Objekts, das zurückgegeben wird.</typeparam>
        public IQueryable<T> CreateQuery<T>(System.Linq.Expressions.Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return new DynamicQueryable<T>(this, expression);
        }

        /// <summary>
        /// Konstruiert ein <see cref="T:System.Linq.IQueryable"/>-Objekt, das die Abfrage auswerten kann, die von einer angegebenen Ausdrucksbaumstruktur dargestellt wird.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="expression"/>' cannot be null.</exception>
        /// <returns>
        /// Ein <see cref="T:System.Linq.IQueryable"/>-Objekt, das die Abfrage auswerten kann, die von der angegebenen Ausdrucksbaumstruktur dargestellt wird.
        /// </returns>
        /// <param name="expression">Eine Ausdrucksbaumstruktur, die eine LINQ-Abfrage darstellt.</param>
        public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var elementType = expression.Type.GetElementType();
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(DynamicQueryable<>).MakeGenericType(elementType), this, expression);
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        /// <summary>
        /// Führt die stark typisierte Abfrage aus, die von einer angegebenen Ausdrucksbaumstruktur dargestellt wird.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="expression"/>' cannot be null.</exception>
        /// <returns>
        /// Der Wert, der aus der Ausführung der angegebenen Abfrage resultiert.
        /// </returns>
        /// <param name="expression">Eine Ausdrucksbaumstruktur, die eine LINQ-Abfrage darstellt.</param><typeparam name="T">Der Typ des Werts, der aus der Ausführung der Abfrage resultiert.</typeparam>
        public T Execute<T>(System.Linq.Expressions.Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return (T) this.Execute(expression);
        }

        /// <summary>
        /// Dematerializes the supplied object to the <see cref="Type"/> defined by <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to return.</typeparam>
        /// <param name="source">The object to dematerialize.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="source"/>' cannot be null.</exception>
        /// <returns>The dematerialized result.</returns>
        public T Dematerialize<T>(Object source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return (T)this.queryResultHandlerProvider.CreateResult(new ResultHandlerContext(source, this.instanceRelationStore, this.noChangeTracking, this.noLazyLoading));
        }

        /// <summary>
        /// The entity type the <see cref="IDynamicQueryProvider"/> was initially created for.
        /// </summary>
        public Type BasedOnEntityType
        {
            get { return typeof (TCreatedFromEntity); }
        }

        /// <summary>
        /// Compiles the supplied <see cref="Expression"/> to a <see cref="LambdaExpression"/> and executes it.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="expression"/>' cannot be null.</exception>
        /// <param name="expression">The <see cref="Expression"/>.</param>
        /// <returns>The result of the executed <see cref="LambdaExpression"/>.</returns>
        Object IQueryProvider.Execute(System.Linq.Expressions.Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return this.Execute(expression);
        }

        /// <summary>
        /// Dissallows change tracking proxy creation for all instances created by this <see cref="DynamicQueryProvider{TCreatedFromEntity}"/> after a call to this method.
        /// </summary>
        /// <returns>The same instance.</returns>
        public IDynamicQueryProvider NoChangeTracking()
        {
            this.noChangeTracking = true;
            return this;
        }

        /// <summary>
        /// Dissallows lazy loading proxy creation for all instances created by this <see cref="DynamicQueryProvider{TCreatedFromEntity}"/> after a call to this method.
        /// </summary>
        /// <returns>The same instance.</returns>
        public IDynamicQueryProvider NoLazyLoading()
        {
            this.noLazyLoading = true;
            return this;
        } 

        /// <summary>
        /// Compiles the supplied <see cref="Expression"/> to a <see cref="LambdaExpression"/> and executes it.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="expression"/>' cannot be null.</exception>
        /// <returns>The result of the executed <see cref="LambdaExpression"/>.</returns>
        public Object Execute([NotNull] System.Linq.Expressions.Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var queryableExpression = expression;

            if (this.BasedOnEntityType == typeof (InstanceDescriptor))
            {
                queryableExpression = new InlinableMethodRewriter().Visit(queryableExpression);

                var compiledLambdaExpression = System.Linq.Expressions.Expression.Lambda(queryableExpression).Compile();

                return compiledLambdaExpression.DynamicInvoke();
            }

            Object source;
            try
            {
                queryableExpression = this.queryableCallTreeTranslator.Translate(this.instances, expression);

                var compiledLambdaExpression = System.Linq.Expressions.Expression.Lambda(queryableExpression).Compile();
                source = compiledLambdaExpression.DynamicInvoke();
            }
            catch (Exception exception)
            {
                throw new INeedYourHelpToImproveException(exception);
            }

            return this.queryResultHandlerProvider.CreateResult(new ResultHandlerContext(source, this.instanceRelationStore, this.noChangeTracking, this.noLazyLoading));
        }
    }
}