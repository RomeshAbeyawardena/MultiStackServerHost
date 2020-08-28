using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace MultiStackServiceHost.Domains
{
    public class Parameter
    {
        public Parameter()
        {
            LogBuilder = new StringBuilder();
        }

        public string CommandText { get; set; }
        public string WorkingDirectory { get; set; }

        [JsonIgnore]
        public bool Activated { get; set; }

        [JsonIgnore]
        public Task Instance { get; set; }

        [JsonIgnore]
        public Process ProcessInstance { get; set; }

        [JsonIgnore]
        public StringBuilder LogBuilder { get; }
    }
}
