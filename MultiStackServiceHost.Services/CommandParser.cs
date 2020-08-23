using Microsoft.Extensions.Logging;
using MultiStackServiceHost.Contracts;
using MultiStackServiceHost.Domains;
using MultiStackServiceHost.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MultiStackServiceHost.Services
{
    public class CommandParser : ICommandParser
    {
        
        public CommandParser(
            ILogger<CommandParser> logger,
            ApplicationSettings applicationSettings)
        {
            this.logger = logger;
            this.applicationSettings = applicationSettings;
        }

        public Command ParseCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            var commandAndArgs = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var commandText = commandAndArgs[0];
            var arguments = commandAndArgs.Remove(0);

            var parameters = arguments.Where(o => !o.StartsWith(applicationSettings.SwitchSeparator));
            var switches = arguments.Where(o => o.StartsWith(applicationSettings.SwitchSeparator));
            var switchKeyValuePairs = GetSwitchKeyValuePairs(switches);

            return new Command
            {
                Text = commandText,
                Parameters = parameters,
                Switches = switches,
                SwitchDictionary = new Dictionary<string, string>(switchKeyValuePairs)
            };
        }

        private IEnumerable<KeyValuePair<string, string>> GetSwitchKeyValuePairs(IEnumerable<string> switches)
        {
            var list = new List<KeyValuePair<string, string>>();

            foreach(var @switch in switches)
            {
                var firstIndex = @switch.IndexOf(':');

                if(firstIndex == -1)
                {
                    logger.LogError("Invalid switch {0} detected, this will not be available", @switch);
                    continue;
                }

                var key = @switch[1..firstIndex];

                var value = @switch.Substring(firstIndex + 1, @switch.Length - firstIndex - 1);

                list.Add(KeyValuePair.Create(key, value));
            }

            return list.ToArray();
        }

        private readonly ILogger<CommandParser> logger;
        private readonly ApplicationSettings applicationSettings;
    }
}
