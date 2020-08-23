using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace MultiStackServerHost.Applet
{
    public class Parameter
    {
        public Parameter()
        {
            LogBuilder = new StringBuilder();
        }
        public string CommandText { get; set; }
        public string WorkingDirectory { get; set; }
        public bool Activated { get; set; }
        public Task Instance { get; set; }
        public Process ProcessInstance { get; set; }
        public StringBuilder LogBuilder { get; }
    }
}
