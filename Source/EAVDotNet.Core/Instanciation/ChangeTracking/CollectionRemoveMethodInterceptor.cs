namespace JanHafner.EAVDotNet.Instanciation.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using Castle.DynamicProxy;
    using JanHafner.EAVDotNet.Model.Values;
    using JetBrains.Annotations;

    /// <summary>
    /// Intercepts the Remove-method of an <see cref="ICollection{T}"/>.
    /// </summary>
    internal sealed class CollectionRemoveMethodInterceptor : IInterceptor
    {
        [NotNull]
        private readonly CollectionPropertyAssociation collectionPropertyAssociation;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionRemoveMethodInterceptor"/> class.
        /// </summary>
        /// <param name="collectionPropertyAssociation">The <see cref="Model.Values.CollectionPropertyAssociation"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="collectionPropertyAssociation"/>' cannot be null.</exception>
        public CollectionRemoveMethodInterceptor([NotNull] CollectionPropertyAssociation collectionPropertyAssociation)
        {
            if (collectionPropertyAssociation == null)
            {
                throw new ArgumentNullException(nameof(collectionPropertyAssociation));
            }

            this.collectionPropertyAssociation = collectionPropertyAssociation;
        }

        /// <summary>
        /// Intercepts the method.
        /// </summary>
        /// <param name="invocation">Information about the invocation.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="invocation"/>' cannot be null.</exception>
        public void Intercept([NotNull] IInvocation invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            this.collectionPropertyAssociation.CollectionItemDescriptors.RemoveCollectionItem(invocation.Arguments[0]);

            invocation.Proceed();
        }
    }
}
