using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiStackServiceHost.Contracts
{
    public interface IDescriptor<T>
    {
        IDescriptor<T> Describe(T value);
        IEnumerable<T> Described { get; }
    }
}
