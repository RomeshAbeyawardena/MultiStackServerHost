﻿using Microsoft.Extensions.Logging;
using MultiStackServiceHost.Contracts;
using MultiStackServiceHost.Domains;
using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace MultiStackServiceHost.Applet
{
    public class Startup
    {   
        public Startup(
            ILogger<Startup> logger,
            ISubject<AppState> appStateSubject, 
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
            appStateSubject.Subscribe(OnNextState);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.Title = applicationSettings.ApplicationTitle;
            applicationState.SetRunningState(true);
            logger.LogInformation("Starting app...");
            while (appState.IsRunning)
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
            appState = newAppState;
        }

        private AppState appState;
        private readonly ILogger<Startup> logger;
        private readonly IApplicationState applicationState;
        private readonly ICommandActions commandActions;
        private readonly ICommandParser commandParser;
        private readonly ApplicationSettings applicationSettings;
    }
}
