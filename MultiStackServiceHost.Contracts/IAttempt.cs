using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiStackServiceHost.Contracts
{
    public interface IAttempt
    {
        Exception Exception { get; }
        bool Successful { get; }
        object Result { get; }
    }

    public interface IAttempt<T> : IAttempt
        where T: class
    {
        new T Result { get; }
    }
}
