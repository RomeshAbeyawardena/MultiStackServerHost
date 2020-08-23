using MultiStackServiceHost.Contracts;
using MultiStackServiceHost.Shared.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiStackServiceHost.Services
{
    public class ResourceService : IResourceService
    {
        public string GetResourceByName(string resourceName)
        {
            return Resources.ResourceManager.GetString(resourceName);
        }
    }
}
