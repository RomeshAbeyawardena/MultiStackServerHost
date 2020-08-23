using System.Diagnostics;

namespace MultiStackServiceHost.Contracts
{
    public interface IProcessService
    {
        Process CreateProcess(string fileName, string arguments, string workingDirectory);
        void KillProcessAndChildren(Process process);
    }
}
