using System;
using System.Collections;
using System.Collections.Generic;

namespace BulletTime.Collections
{
    public class ConcurrentSet<T> : ISet<T>
    {
        private readonly IDictionary<T, byte> _set;

        public ConcurrentSet()
        {
            _set = new Dictionary<T, byte>();
        }

        public ConcurrentSet(IEqualityComparer<T> comparer)
        {
            _set = new Dictionary<T, byte>(comparer);
        }

        private ICollection<T> Values
        {
            get { return _set.Keys; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) Values).GetEnumerator();
        }

        void ICollection<T>.Add(T item)
        {
            _set.Add(item, 0);
        }

        public bool Add(T item)
        {
            var result = !_set.ContainsKey(item);

            if (result)
            {
                ((ICollection<T>) this).Add(item);
            }

            return result;
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            _set.Clear();
        }

        public bool Contains(T item)
        {
            return _set.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _set.Keys.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _set.Remove(item);
        }

        public int Count
        {
            get { return _set.Count; }
        }

        public bool IsReadOnly
        {
            get { return _set.IsReadOnly; }
        }
    }
}