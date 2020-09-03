using Microsoft.Extensions.Logging;
using MultiStackServiceHost.Contracts;
using MultiStackServiceHost.Domains;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileSystemGlobbing;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace MultiStackServiceHost.Applet
{
    public class Startup
    {   
        public Startup(
            ILogger<Startup> logger,
            IApplicationState applicationState,
            ICommandActions commandActions,
            ICommandParser commandParser,
            ApplicationSettings applicationSettings)
        {
            this.logger = logger;
            this.applicationState = applicationState;
            this.commandActions = commandActions;
            this.commandParser = commandParser;
            this.applicationSettings = applicationSettings;
            this.applicationState.Subscribe(OnNextState);
        }

        
        public static string ReadLine()
        {
            var lineStringBuilder = new StringBuilder();

            var dictionary = new Dictionary<ConsoleKey, Func<ConsoleKey, string, bool>>();

            dictionary.Add(ConsoleKey.Enter, (key, currentVal) => true);
            dictionary.Add(ConsoleKey.Tab, (key, currentVal) =>
            {
                var matcher = new Matcher();
                var parameterList = new List<string>();
                foreach(var parameter in currentVal.Split(' '))
                { 
                    if(Regex.IsMatch(parameter, @"(.)+([:]{0,1})([\\])(.)+"))
                    {
                        var result = matcher.AddInclude(parameter).Execute(new DirectoryInfoWrapper(new DirectoryInfo("C:\\")));
                        if (result.HasMatches)
                        {
                            parameterList.Add(result.Files.FirstOrDefault().Path);
                        }
                        parameterList.Add(parameter);
                        continue;
                    }
                    parameterList.Add(parameter);
                }
                lineStringBuilder.Append(string.Join(' ', parameterList));
                return false;
            });

            var currentKey = Console.ReadKey(true);

            while(true)
            {
                if (dictionary.TryGetValue(currentKey.Key, out var action) 
                    && action.Invoke(currentKey.Key, lineStringBuilder.ToString()))
                {
                    break;
                }
                if(action == null)
                { 
                    lineStringBuilder.Append(currentKey.KeyChar);
                    Console.Write(currentKey.KeyChar);
                }

                currentKey = Console.ReadKey(true);
            }

            return lineStringBuilder.ToString();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.Title = applicationSettings.ApplicationTitle;
            applicationState.SetState(state => { 
                state.IsRunning = true;
                state.WarnOnMultipleAbort = true;
            });

            logger.LogInformation("Starting app...");
            while (applicationState.State.IsRunning)
            {
                var input = Console.ReadLine();
                
                var command = commandParser.ParseCommand(input);

                if(command == null)
                {
                    logger.LogError("Unable to parse command '{0}'", input);
                    continue;
                }

                var parameter = commandActions.Case(command.Text);

                if(parameter == null)
                {
                    logger.LogError("Unable to process command '{0}'", input);
                    continue;
                }

                parameter.Invoke(command);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private void OnNextState(AppState newAppState)
        {
            logger.LogDebug("Application state updated");
        }

        private readonly ILogger<Startup> logger;
        private readonly IApplicationState applicationState;
        private readonly ICommandActions commandActions;
        private readonly ICommandParser commandParser;
        private readonly ApplicationSettings applicationSettings;
    }
}
