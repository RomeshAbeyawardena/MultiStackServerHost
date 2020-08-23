using MultiStackServiceHost.Contracts;
using MultiStackServiceHost.Domains;
using MultiStackServiceHost.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiStackServiceHost.Services
{
    public class CommandParser : ICommandParser
    {
        
        public CommandParser(ApplicationSettings applicationSettings)
        {
            this.applicationSettings = applicationSettings;
        }

        public Command ParseCommand(string input)
        {
            var commandAndArgs = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var commandText = commandAndArgs[0];
            var arguments = commandAndArgs.Remove(0);

            var parameters = arguments.Where(o => !o.StartsWith(applicationSettings.SwitchSeparator));
            var switches = arguments.Where(o => o.StartsWith(applicationSettings.SwitchSeparator));

            return new Command
            {
                Text = commandText,
                Parameters = parameters,
                Switches = switches
            };
        }

        private readonly ApplicationSettings applicationSettings;
    }
}
