using Microsoft.Extensions.Logging;
using MultiStackServiceHost.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MultiStackServiceHost.Services
{
    public class ProcessService : IProcessService
    {
        public ProcessService(ILogger<ProcessService> logger)
        {
            this.logger = logger;
        }

        public void KillProcessAndChildren(Process process)
        {
            KillProcessAndChildren(process.Id);
        }

        public Process StartProcess(string fileName, string arguments, string workingDirectory)
        {
            var process = new Process();
            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;

            if (!string.IsNullOrEmpty(workingDirectory))
            {
                process.StartInfo.WorkingDirectory = workingDirectory;
            }

            return process;
        }

        private void KillProcessAndChildren(int pid)
        {
            var processSearcher = new ManagementObjectSearcher
              ("Select * From Win32_Process Where ParentProcessID=" + pid);
            var processCollection = processSearcher.Get();

            logger.LogDebug("{0} child proccesses found for PID: {1}", processCollection.Count, pid);

            try
            {
                Process proc = Process.GetProcessById(pid);
                if (!proc.HasExited) proc.Kill();
            }
            catch (ArgumentException exception)
            {
                logger.LogError(exception, "Process already exited");
            }

            if (processCollection != null)
            {
                
                foreach (var mo in processCollection)
                {
                    KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"])); //kill child processes(also kills childrens of childrens etc.)
                }
            }
        }

        private readonly ILogger<ProcessService> logger;
    }
}
