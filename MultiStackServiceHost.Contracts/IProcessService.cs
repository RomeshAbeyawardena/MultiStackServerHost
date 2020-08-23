using System.Diagnostics;

namespace MultiStackServiceHost.Contracts
{
    public interface IProcessService
    {
        Process StartProcess(string fileName, string arguments, string workingDirectory);
        void KillProcessAndChildrens(Process process);
    }
}
