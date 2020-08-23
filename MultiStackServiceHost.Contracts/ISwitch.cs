using System.Collections.Generic;

namespace MultiStackServiceHost.Contracts
{
    public interface ISwitch<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        ISwitch<TKey, TValue> CaseWhen(TKey key, TValue value);
        TValue Case(TKey key);
    }
}
