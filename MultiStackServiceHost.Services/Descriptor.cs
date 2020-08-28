using MultiStackServiceHost.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiStackServiceHost.Services
{
    public class Descriptor<T> : IDescriptor<T>
    {
        public Descriptor(IEnumerable<T> items = null)
        {
            itemsBag = items == null 
                ? new ConcurrentBag<T>()
                : new ConcurrentBag<T>(items);
        }

        public IEnumerable<T> Described => itemsBag.ToArray();

        public IDescriptor<T> Describe(T value)
        {
            if(!itemsBag.Contains(value))
            {
                itemsBag.Add(value);
            }

            return this;
        }

        private ConcurrentBag<T> itemsBag;
    }
}
