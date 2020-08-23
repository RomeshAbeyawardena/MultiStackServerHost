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
            ICommandParser commandParser)
        {
            this.logger = logger;
            this.applicationState = applicationState;
            this.commandActions = commandActions;
            this.commandParser = commandParser;
            appStateSubject.Subscribe(OnNextState);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            applicationState.SetRunningState(true);
            logger.LogInformation("Starting app...");
            while (appState.IsRunning)
            {
                Console.Write("\r\n> ");
                var command = commandParser.ParseCommand(Console.ReadLine());

                var parameter = commandActions.Case(command.Text);

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
    }
}
