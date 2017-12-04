namespace CXLight.DataStructures.MultiMap
{
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using CXLight.Exts;

    public class MultiMap<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, List<TValue>> _dictionary = new Dictionary<TKey, List<TValue>>();

        #region IDictionary Compliance

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var keyValuePair in _dictionary)
            {
                foreach (var value in keyValuePair.Value)
                {
                    yield return new KeyValuePair<TKey, TValue>(keyValuePair.Key, value);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (_dictionary.ContainsKey(item.Key))
            {
                _dictionary[item.Key].Add(item.Value);
            }
            else
            {
                _dictionary[item.Key] = new List<TValue> { item.Value };
            }

            Count++;
        }

        public void Clear()
        {
            _dictionary.Clear();
            Count = 0;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this.Some(
                pair =>
                {
                    if (!pair.Key.Equals(item.Key)) return false;

                    return pair.Value != null && pair.Value.Equals(item.Value);
                }
            );
        }

        /// <summary>
        /// Return true if key was found inside
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(TKey key)
        {
            return this.Some(pair => pair.Key.Equals(key));
        }

        /// <summary>
        /// Returns true if value was found inside
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(TValue value)
        {
            return this.Some(pair => pair.Value != null && pair.Value.Equals(value));
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException();
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
            if ((array.Length - arrayIndex) < Count) throw new ArgumentException();

            var i = 0 + arrayIndex;
            foreach (var pair in this) array[i++] = pair;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!_dictionary.ContainsKey(item.Key)) return false;
            if (!_dictionary[item.Key].Contains(item.Value)) return false;

            _dictionary[item.Key].Remove(item.Value);

            if (_dictionary[item.Key].Count == 0) _dictionary.Remove(item.Key);

            Count--;

            return true;
        }

        public int Count { get; private set; } = 0;

        public bool IsReadOnly { get; }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            if (_dictionary.ContainsKey(key))
            {
                _dictionary[key].Add(value);
            }
            else
            {
                _dictionary[key] = new List<TValue> { value };
            }

            Count++;
        }

        public bool Remove(TKey key)
        {
            if (!_dictionary.ContainsKey(key)) return false;

            var keySize = _dictionary[key].Count;

            _dictionary[key].Clear();
            _dictionary.Remove(key);

            Count -= keySize;
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (ContainsKey(key))
            {
                if (_dictionary[key].Count >= 1)
                {
                    value = _dictionary[key][0];
                    return true;
                }
            }

            value = default(TValue);
            return false;
        }

        // TODO Perhaps making another MultiMap where the values are objects and then defining some casting operations would work better
        // TODO Making this rotate what it provides while keeping a counter for each key rotated would work wonders a map of {key, index}
        public TValue this[TKey key]
        {
            get => _dictionary[key][0];
            set => Add(key, value);
        }

        public ICollection<TKey> Keys
        {
            get
            {
                var result = new List<TKey>();

                foreach (var pair in _dictionary) result.Add(pair.Key);

                return result;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                var result = new List<TValue>();

                foreach (var pair in this) result.Add(pair.Value);

                return result;
            }
        }

        #endregion

        public void Remove(TKey key, TValue value)
        {
            Remove(new KeyValuePair<TKey, TValue>(key, value));
        }

        public List<TValue> GetKeyValues(TKey key)
        {
            var result = new List<TValue>();

            if (!_dictionary.ContainsKey(key)) return result;

            result.AddRange(_dictionary[key]);

            return result;
        }

        /// <summary>
        /// Retrieves the key's values directly
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<TValue> GetValues(TKey key)
        {
            return _dictionary.ContainsKey(key) ? _dictionary[key] : null;
        }
    }
}