﻿using MultiStackServiceHost.Domains;
using MultiStackServiceHost.Contracts;
using MultiStackServiceHost.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MultiStackServiceHost.Services
{
    public class CommandActions : Switch<string, Action<Command>>, ICommandActions
    {
        public CommandActions(
            ILogger<CommandActions> logger,
            IProcessService processService,
            IApplicationState applicationState,
            IResourceService resourceService,
            IApplicationStateValueSetter applicationStateValueSetter,
            IFileSystem fileSystem,
            ApplicationSettings applicationSettings)
        {
            parameters = new List<Parameter>();

            CaseWhen("add", AddParameter)
                .CaseWhen("run", RunParameters)
                .CaseWhen("list", List)
                .CaseWhen("load", Load)
                .CaseWhen("read", ReadLogs)
                .CaseWhen("abort", Abort)
                .CaseWhen("help", Help)
                .CaseWhen("global", Global)
                .CaseWhen("quit", Quit);

            this.logger = logger;
            this.processService = processService;
            this.applicationState = applicationState;
            this.resourceService = resourceService;
            this.applicationStateValueSetter = applicationStateValueSetter;
            this.fileSystem = fileSystem;
            this.applicationSettings = applicationSettings;
        }

        private void Load(Command command)
        {
            var fileName = command.Parameters.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(fileName))
            {
                logger.LogError("File {0} not specified", fileName);
                return;
            }

            var attempt = fileSystem.TryReadTextFile(fileName);

            if(attempt.Successful){
                Abort(command);
                var newParameters = JsonSerializer
                    .Deserialize<IEnumerable<Parameter>>(attempt.Result);

                parameters.Clear();
                parameters.AddRange(newParameters);
            }
            else
            {
                logger.LogError(attempt.Exception, "Unable to save tasks to file.");
            }
        }

        private void Global(Command command)
        {
            var firstParameter = command.Parameters.FirstOrDefault();

            if (firstParameter == null)
            {
                return;
            }

            if (firstParameter.StartsWith("set:"))
            {
                if (command.Parameters.Count() < 2)
                {
                    return;
                }

                var setting = firstParameter.Replace("set:", string.Empty);
                var value = command.Parameters.GetByIndex(1);

                if (!applicationStateValueSetter.TrySetValue(setting, value))
                {
                    logger.LogError("Unable to set property {0}", setting);
                }
            }
        }

        private void Help(Command command)
        {
            var helpFile = resourceService.GetResourceByName(applicationSettings.HelpFile);
            logger.LogInformation(helpFile);
        }

        private void ReadLogs(Command command)
        {
            var processIndexString = command.Parameters.FirstOrDefault();

            if (processIndexString != null 
                && int.TryParse(processIndexString, out var processIndex))
            {

                if (processIndex < 0 
                    || parameters.Count == 0 
                    || processIndex > parameters.Count)
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

                        var process = processService.CreateProcess(
                            applicationSettings.FileName,
                            parameter.CommandText,
                            string.IsNullOrWhiteSpace(parameter.WorkingDirectory)
                                ? applicationState.State.DefaultWorkDirectory ?? string.Empty
                                : parameter.WorkingDirectory);

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

            if (command.SwitchDictionary
                .TryGetValue(applicationSettings.WorkingDirectoryParameter, out var workingDirectory))
            {
                logger.LogDebug("Working directory switch has been specified: {0}", workingDirectory);
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
                bool hasParameters = !parameters.IsEmpty()
                    && parameters.Any(parameter => parameter.Activated);

                if (hasParameters)
                {
                    if (applicationState.State.WarnOnMultipleAbort)
                    {
                        logger
                            .LogWarning("This will cause all tasks to terminate, " +
                            "are you sure you want to proceed? Y/N");
                    }

                    if (!applicationState.State.WarnOnMultipleAbort || Console.ReadKey().Key == ConsoleKey.Y)
                    {
                        foreach (var parameter in parameters)
                        {
                            KillProcess(parameter);
                        }
                    }
                    else
                    {
                        logger.LogInformation("Abort process cancelled");
                    }

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

            if (command.SwitchDictionary.TryGetValue("save", out var saveFilePath))
            {
                var attempt = fileSystem.TryWriteTextFile(saveFilePath, JsonSerializer.Serialize(parameters.ToArray()));
                if(attempt.Successful)
                {
                    logger.LogInformation("Tasks have been saved to {0}", saveFilePath);
                }
                else
                {
                    logger.LogError(attempt.Exception, "Unable to save tasks to file.");
                }
                return;
            }

            var listBuilder = new StringBuilder(
                "\r\nIndex:\t[CommandText]\t\t\t[Activated]\tInstance Id\tProcessId\t[Working Directory]\r\n");

            var index = 0;
            foreach (var parameter in parameters)
            {
                listBuilder.AppendFormat("{0}:\t{1}\t\t\t{2}\t{3}\t{4}\t{5}\r\n",
                    index++, parameter.CommandText, parameter.Activated,
                    parameter.Instance?.Id, parameter.ProcessInstance?.Id, parameter.WorkingDirectory);
            }

            logger.LogInformation(listBuilder.ToString());
        }

        private void Quit(Command command)
        {
            applicationState.SetState(state => state.IsRunning = false);
            Abort(command);
        }

        private readonly List<Parameter> parameters;
        private readonly ILogger<CommandActions> logger;
        private readonly IProcessService processService;
        private readonly IApplicationState applicationState;
        private readonly IResourceService resourceService;
        private readonly IApplicationStateValueSetter applicationStateValueSetter;
        private readonly IFileSystem fileSystem;
        private readonly ApplicationSettings applicationSettings;
    }
}
