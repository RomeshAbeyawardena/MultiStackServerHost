using MultiStackServiceHost.Domains;
using MultiStackServiceHost.Contracts;
using MultiStackServiceHost.Shared.Extensions;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text;

namespace MultiStackServiceHost.Services
{
    public class CommandActions : Switch<string, Action<Command>>, ICommandActions
    {
        public CommandActions(
            ILogger<CommandActions> logger,
            IProcessService processService,
            IApplicationState applicationState,
            ApplicationSettings applicationSettings)
        {
            parameters = new List<Parameter>();

            CaseWhen("add", AddParameter)
                .CaseWhen("run", RunParameters)
                .CaseWhen("list", List)
                .CaseWhen("read", ReadLogs)
                .CaseWhen("abort", Abort)
                .CaseWhen("quit", Quit);
            this.logger = logger;
            this.processService = processService;
            this.applicationState = applicationState;
            this.applicationSettings = applicationSettings;
        }


        private void ReadLogs(Command command)
        {
            var processIndexString = command.Parameters.GetByIndex(0);

            if (int.TryParse(processIndexString, out var processIndex))
            {
                var cmd = parameters[processIndex];

                if (command != null)
                {
                    logger.LogInformation(cmd.LogBuilder.ToString());
                }
            }
        }

        private void RunParameters(Command command)
        {
            foreach (var parameter in parameters)
            {
                if (!parameter.Activated)
                {
                    parameter.Instance = Task.Run(() =>
                    {
                        logger.LogInformation($"Task { parameter.CommandText } running");
                        var process = processService.StartProcess(applicationSettings.FileName, parameter.CommandText, parameter.WorkingDirectory);
                        parameter.ProcessInstance = process;
                        parameter.Activated = true;
                        process.Start();

                        Task.Run(() =>
                        {
                            while (!process.StandardOutput.EndOfStream)
                            {
                                parameter.LogBuilder.AppendLine(process.StandardOutput.ReadLine());
                            }
                        });
                        
                        process.WaitForExit();
                        parameter.Activated = false;
                        logger.LogInformation($"Task { parameter.CommandText } completed");
                    });
                }
            }
        }

        private void AddParameter(Command command)
        {
            var workingDirectory = command.Switches.FirstOrDefault(@switch => @switch.StartsWith(applicationSettings.WorkingDirectoryParameter));

            if (!string.IsNullOrEmpty(workingDirectory))
            {
                workingDirectory = workingDirectory.Replace(applicationSettings.WorkingDirectoryParameter, string.Empty);
            }

            parameters.Add(new Parameter
            {
                CommandText = string.Join(' ', command.Parameters),
                WorkingDirectory = workingDirectory
            });
        }


        private void Abort(Command command)
        {
            foreach (var parameter in parameters)
            {
                if (parameter.Activated)
                {
                    processService.KillProcessAndChildrens(parameter.ProcessInstance);
                }
            }
        }

        private void List(Command command)
        {
            if (parameters.IsEmpty())
            {
                logger.LogInformation("Nothing to list, add commands using the add command");
                return;
            }

            var listBuilder = new StringBuilder();
            listBuilder.AppendFormat("\r\nIndex:\t[CommandText]\t\t\t[Activated]\t[Working Directory]\r\n");
            
            var index = 0;
            foreach (var parameter in parameters)
            {
                listBuilder.AppendFormat("{0}:\t{1}\t\t\t{2}\t{3}\t{4}\r\n", 
                    index++, parameter.CommandText, parameter.Activated, 
                    parameter.Instance?.Id, parameter.WorkingDirectory);
            }

            logger.LogInformation(listBuilder.ToString());
        }

        private void Quit(Command command)
        {
            applicationState.SetRunningState(false);
            Abort(command);
        }

        private readonly List<Parameter> parameters;
        private readonly ILogger<CommandActions> logger;
        private readonly IProcessService processService;
        private readonly IApplicationState applicationState;
        private readonly ApplicationSettings applicationSettings;
    }
}
