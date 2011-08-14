using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace FireEngine.Library
{
    /// <summary>
    /// TODO: 正常实现！
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class TreeMap<TKey, TValue> : IDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>, ICollection<KeyValuePair<TKey, TValue>>
    {
        Dictionary<int, TValue> idToValueMap = new Dictionary<int, TValue>();
        Dictionary<TKey, int> keyToIdMap = new Dictionary<TKey, int>();
        Dictionary<int, TKey> idToKeyMap = new Dictionary<int, TKey>();
        int counter = 0;

        #region IDictionary<TKey,TValue> Members

        public void Add(TKey key, TValue value)
        {
            idToValueMap.Add(counter, value);
            keyToIdMap.Add(key, counter);
            idToKeyMap.Add(counter, key);
            counter++;
        }

        public bool ContainsKey(TKey key)
        {
            return keyToIdMap.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return keyToIdMap.Keys; }
        }

        public bool Remove(TKey key)
        {
            if (keyToIdMap.ContainsKey(key))
            {
                int id = keyToIdMap[key];
                keyToIdMap.Remove(key);
                idToValueMap.Remove(id);
                idToKeyMap.Remove(id);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (keyToIdMap.ContainsKey(key))
            {
                value = idToValueMap[keyToIdMap[key]];
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return idToValueMap.Values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (keyToIdMap.ContainsKey(key))
                {
                    return idToValueMap[keyToIdMap[key]];
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }

            set
            {
                if (keyToIdMap.ContainsKey(key))
                {
                    idToValueMap[keyToIdMap[key]] = value;
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            keyToIdMap.Clear();
            idToValueMap.Clear();
            counter = 0;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public int Count
        {
            get { return idToValueMap.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, System.Collections.IEnumerator
        {
            private TreeMap<TKey, TValue> treeMap;

            int pos;

            public Enumerator(TreeMap<TKey, TValue> treeMap)
            {
                this.treeMap = treeMap;
                pos = -1;
            }

            #region IEnumerator<KeyValuePair<TKey,TValue>> Members

            public KeyValuePair<TKey, TValue> Current
            {
                get 
                {
                    return new KeyValuePair<TKey, TValue>(treeMap.idToKeyMap[pos], treeMap.idToValueMap[pos]);
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get 
                {
                    return Current;
                }
            }

            public bool MoveNext()
            {
                pos++;
                return gotoExist();
            }

            public void Reset()
            {
                pos = 0;
            }

            #endregion

            private bool gotoExist()
            {
                for (; !treeMap.idToKeyMap.ContainsKey(pos) && pos < treeMap.counter; pos++);
                return pos < treeMap.counter;

            }
        }

    }
}
