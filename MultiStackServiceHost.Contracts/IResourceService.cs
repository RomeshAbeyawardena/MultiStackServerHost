using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiStackServiceHost.Contracts
{
    public interface IResourceService
    {
        string GetResourceByName(string resourceName);
    }
}
