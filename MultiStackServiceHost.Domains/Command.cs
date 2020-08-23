using System.Collections.Generic;

namespace MultiStackServiceHost.Domains
{
    public class Command
    {
        public string Text { get; set; }
        public IEnumerable<string> Parameters { get; set; }
        public IEnumerable<string> Switches { get; set; }
    }
}
