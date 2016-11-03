namespace JanHafner.EAVDotNet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using JanHafner.EAVDotNet.Classification;
    using JanHafner.EAVDotNet.Context.Adapter;
    using JanHafner.EAVDotNet.Model;
    using JanHafner.EAVDotNet.Model.Collections;
    using JanHafner.EAVDotNet.Model.Identity;
    using JanHafner.EAVDotNet.Properties;
    using JanHafner.EAVDotNet.Query;
    using JetBrains.Annotations;

    /// <summary>
    /// Provides extension methods for Types inside this assembly.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Compares the identity of the supplied <see cref="Object"/> to all identities in the supplied collection.
        /// If the identity is present the Remove()-operation will be executed.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="instanciatedCollectionItem">The <see cref="Object"/> to remove.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="collection"/>' and '<paramref name="instanciatedCollectionItem"/>' cannot be null.</exception>
        internal static void RemoveCollectionItem([NotNull] this ICollection<CollectionItem> collection,
            [NotNull] Object instanciatedCollectionItem)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (instanciatedCollectionItem == null)
            {
                throw new ArgumentNullException(nameof(instanciatedCollectionItem));
            }

            var existingCollectionItem = instanciatedCollectionItem.GetType().Classify() == TypeClassifier.TypeClassification.Primitive
                ? collection.SingleOrDefault(m => ((IValueAssociation) m).Value.Equals(instanciatedCollectionItem))
                : collection.SingleOrDefault(m => ((IValueAssociation) m).Value.IdentityEquals(instanciatedCollectionItem));

            if (existingCollectionItem != null)
            {
                collection.Remove(existingCollectionItem);
            }
        }

        /// <summary>
        /// Determines whether the supplied generic type definition of <see cref="Type"/> is assignable from <see cref="IDynamicDbSetAdapter{T}"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to check.</param>
        /// <exception cref="ArgumentNullException">The value of <paramref name="type"/> cannot be null.</exception>
        /// <returns>A value indicating, whether, the supplied <see cref="Type"/> is assignable to <see cref="IDynamicDbSetAdapter{T}"/>.</returns>
        internal static Boolean IsDynamicDbSet([NotNull] this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return typeof (IDynamicDbSetAdapter<>).IsAssignableFrom(type.GetGenericTypeDefinition());
        }

        /// <summary>
        /// Determines whether the supplied <see cref="Type"/> is assignable to <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to check.</param>
        /// <exception cref="ArgumentNullException">The value of <paramref name="type"/> cannot be null.</exception>
        /// <returns>A value indicating, whether, the supplied <see cref="Type"/> is assignable to <see cref="IEnumerable"/>.</returns>
        internal static Boolean IsEnumerable([NotNull] this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type != typeof(String) && typeof (IEnumerable).IsAssignableFrom(type);
        }

        /// <summary>
        /// Dematerializes the <see cref="InstanceDescriptor"/> instances of the supplied <see cref="IQueryable{InstanceDescriptor}"/> the <see cref="Type"/> defined by <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to which the <see cref="InstanceDescriptor"/> are converted to.</typeparam>
        /// <param name="source">The <see cref="IQueryable{InstanceDescriptor}"/>.</param>
        /// <returns>A list of dematerialized <see cref="InstanceDescriptor"/> instances.</returns>
        [LinqTunnel]
        [NotNull]
        public static IEnumerable<T> Dematerialize<T>([NotNull] this IQueryable<InstanceDescriptor> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var dynamicQueryProvider = source.Provider as IDynamicQueryProvider;
            if (dynamicQueryProvider == null)
            {
                throw new InvalidOperationException(ExceptionMessages.IDynamicQueryProviderNotProvidedExceptionMessage);
            }

            var result = dynamicQueryProvider.Dematerialize<IEnumerator>(source);
            if (result == null)
            {
                yield return default(T);
            }
            else
            {
                while (result.MoveNext())
                {
                    yield return (T)result.Current;
                }
            }
        }

        /// <summary>
        /// Tries to disable lazy loading on the supplied <see cref="IQueryable{T}"/>. Works only if the Provider property returns an <see cref="IDynamicQueryProvider{T}"/>.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryable">The <see cref="IQueryable{T}"/>.</param>
        /// <returns>The same <see cref="IQueryable{T}"/> as supplied.</returns>
        public static IQueryable<T> WithoutLazyLoading<T>(this IQueryable<T> queryable) 
            where T : class
        {
            if (queryable == null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }

            var dynamicQueryProvider = queryable.Provider as IDynamicQueryProvider<T>;
            dynamicQueryProvider?.NoLazyLoading();

            return queryable;
        }
    }
}
