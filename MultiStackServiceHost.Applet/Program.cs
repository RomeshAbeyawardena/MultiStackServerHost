using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MultiStackServiceHost.Shared.Extensions;
using MultiStackServiceHost.Broker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace MultiStackServiceHost.Applet
{
    static class Program
    {
        
        static async Task Main(string[] args)
        { 
            await Host.CreateDefaultBuilder(args)
                .UseDefaultServiceProvider(serviceProviderOptions => serviceProviderOptions.ValidateOnBuild = true)
                .UseConsoleLifetime()
                .ConfigureAppHost()
                .ConfigureAppConfiguration((host, configuration) => configuration
                    .AddJsonFile("appsettings.json")
                    .AddCommandLine(args))
                .ConfigureServices(RegisterServices)
                .ConfigureLogging(logging => logging
                    .AddConsole())
                .Build<Startup>()
                .UseStartup(startingAsyncMethod: (a, cancellationToken) => a.StartAsync(cancellationToken))
                .StartAsync();
        }

        private static void RegisterServices(HostBuilderContext hostBuilderContext, IServiceCollection services)
        {
            ServiceRegistration.Register(hostBuilderContext, services);
        }
    }
}
