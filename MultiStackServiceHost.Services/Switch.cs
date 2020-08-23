using MultiStackServiceHost.Contracts;
using System.Collections;
using System.Collections.Generic;

namespace MultiStackServiceHost.Services
{
    public class Switch<TKey, TValue> : ISwitch<TKey, TValue>
    {
        public Switch()
        {
            dictionary = new Dictionary<TKey, TValue>();
        }

        public ISwitch<TKey, TValue> CaseWhen(TKey key, TValue value)
        {
            dictionary.Add(key, value);
            return this;
        }

        public TValue Case(TKey key)
        {
            if(dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return default;
        }

        TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key] => dictionary[key];

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => dictionary.Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => dictionary.Values;

        int IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count => dictionary.Count;

        bool IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        private readonly IDictionary<TKey, TValue> dictionary;
    }
}
