namespace JanHafner.EAVDotNet.Instanciation.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using JetBrains.Annotations;

    /// <summary>
    /// Provides an implementation of <see cref="List{T}"/> whose Add-/Remove-Methods can be proxied. 
    /// </summary>
    /// <typeparam name="T">The element <see cref="Type"/>.</typeparam>
    public class ProxyableList<T> : IList<T>
    {
        [NotNull]
        private readonly IList<T> internalList; 

        public ProxyableList()
        {
            this.internalList = new List<T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public virtual void Add(T item)
        {
            this.internalList.Add(item);
        }

        /// <summary>
        /// Is used implicitly during instanciation of a collction to avoid proxy behavior. 
        /// Otherwise the proxy would try to add the object twice.
        /// </summary>
        /// <param name="item">The item.</param>
        [UsedImplicitly]
        private void AddNonProxied(T item)
        {
            this.internalList.Add(item);
        }

        public Int32 Add(Object value)
        {
            return ((IList)this.internalList).Add(value);
        }

        public Boolean Contains(Object value)
        {
            return ((IList)this.internalList).Contains(value);
        }

        public Int32 IndexOf(Object value)
        {
            return ((IList)this.internalList).IndexOf(value);
        }

        public void Insert(Int32 index, Object value)
        {
            ((IList)this.internalList).Insert(index, value);
        }

        public void Remove(Object value)
        {
            ((IList)this.internalList).Remove(value);
        }

        public Boolean IsFixedSize
        {
            get { return ((IList) this.internalList).IsFixedSize; }
        }

        public virtual void Clear()
        {
            this.internalList.Clear();
        }

        public Boolean Contains(T item)
        {
            return this.internalList.Contains(item);
        }

        public void CopyTo(T[] array, Int32 arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        public virtual Boolean Remove(T item)
        {
            return this.internalList.Remove(item);
        }

        public void CopyTo(Array array, Int32 index)
        {
            ((IList)this.internalList).CopyTo(array, index);
        }

        public Object SyncRoot
        {
            get { return ((IList) this.internalList).SyncRoot; }
        }

        public Boolean IsSynchronized
        {
            get { return ((IList) this.internalList).IsSynchronized; }
        }

        Int32 ICollection<T>.Count
        {
            get { return this.internalList.Count; }
        }

        Boolean ICollection<T>.IsReadOnly
        {
            get { return this.internalList.IsReadOnly; }
        }
        public Int32 IndexOf(T item)
        {
            return this.internalList.IndexOf(item);
        }

        public void Insert(Int32 index, T item)
        {
            this.internalList.Insert(index, item);
        }

        void IList<T>.RemoveAt(Int32 index)
        {
            this.internalList.RemoveAt(index);
        }

        T IList<T>.this[Int32 index]
        {
            get { return this.internalList[index]; }
            set { this.internalList[index] = value; }
        }
    }
}
