using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiStackServiceHost.Contracts
{
    public interface IApplicationStateValueSetter
    {
        bool TrySetValue(string setting, string value);
    }
}
