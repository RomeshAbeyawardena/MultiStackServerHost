using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MultiStackServiceHost.Contracts;
using MultiStackServiceHost.Domains;
using MultiStackServiceHost.Services;
using System.Reactive.Subjects;

namespace MultiStackServiceHost.Broker
{
    public static class ServiceRegistration
    {
        public static void Register(HostBuilderContext hostBuilderContext, IServiceCollection services)
        {
            services
                .AddSingleton<ApplicationSettings>()
                .AddSingleton(typeof(ISubject<>), typeof(Subject<>))
                .AddSingleton<IApplicationState, ApplicationState>()
                .AddSingleton<ICommandActions, CommandActions>()
                .AddSingleton<ICommandParser, CommandParser>()
                .AddSingleton<IProcessService, ProcessService>();
        }
    }
}
