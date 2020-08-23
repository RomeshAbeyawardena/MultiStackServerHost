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

                if (processIndex > parameters.Count)
                {
                    logger.LogError("Unable to find logs for process {0}", processIndex);
                    return;
                }

                var cmd = parameters[processIndex];

                if (command != null)
                {
                    logger.LogInformation(cmd.LogBuilder.ToString());
                }

                return;
            }

            logger.LogError("Unable to parse {0} as a number", processIndexString);
        }

        private void RunParameters(Command command)
        {
            foreach (var parameter in parameters)
            {
                if (!parameter.Activated)
                {
                    parameter.Instance = Task.Run(() =>
                    {
                        logger.LogDebug("Creating process in worker {0} with the following parameters:" +
                            "\r\n\tFile Name: {1}" +
                            "\r\n\tArguments: {2}" +
                            "\r\n\tWorking Directory: {3}",
                            Task.CurrentId,
                            applicationSettings.FileName,
                            parameter.CommandText,
                            parameter.WorkingDirectory
                        );

                        var process = processService.StartProcess(
                            applicationSettings.FileName,
                            parameter.CommandText,
                            parameter.WorkingDirectory);

                        parameter.ProcessInstance = process;
                        parameter.Activated = true;
                        process.Start();
                        logger.LogInformation($"Task { parameter.CommandText } running");

                        Task.Run(() =>
                        {
                            while (!process.HasExited && !process.StandardOutput.EndOfStream)
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
            if (command.Parameters.IsEmpty())
            {
                logger.LogError("Unable to add parameter with an empty command");
                return;
            }

            var workingDirectory = command.Switches.FirstOrDefault(@switch => @switch.StartsWith(applicationSettings.WorkingDirectoryParameter));

            if (!string.IsNullOrEmpty(workingDirectory))
            {
                workingDirectory = workingDirectory
                    .Replace(applicationSettings.WorkingDirectoryParameter, string.Empty);
            }

            parameters.Add(new Parameter
            {
                CommandText = string.Join(' ', command.Parameters),
                WorkingDirectory = workingDirectory
            });
        }


        private void Abort(Command command)
        {
            void KillProcess(Parameter param)
            {
                if (param.Activated)
                    {
                        processService.KillProcessAndChildren(param.ProcessInstance);
                    }
            }

            var processIdParameter = command.Parameters.FirstOrDefault();

            if (string.IsNullOrEmpty(processIdParameter)
                || !int.TryParse(processIdParameter, out var processId)
                || processId > parameters.Count)
            {
                foreach (var parameter in parameters)
                {
                    KillProcess(parameter);
                }
                return;
            }

            KillProcess(parameters[processId]);
        }

        private void List(Command command)
        {
            if (parameters.IsEmpty())
            {
                logger.LogInformation("Nothing to list, add commands using the add command");
                return;
            }

            var listBuilder = new StringBuilder(
                "\r\nIndex:\t[CommandText]\t\t\t[Activated]\t[Working Directory]\r\n");

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
