using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MultiStackServiceHost.Contracts;
using MultiStackServiceHost.Domains;
using MultiStackServiceHost.Services;
using System.Reactive.Subjects;
using System.Text.Json.Serialization;

namespace MultiStackServiceHost.Broker
{
    public static class ServiceRegistration
    {
        public static void Register(HostBuilderContext hostBuilderContext, IServiceCollection services)
        {
            services
                .AddSingleton<ApplicationSettings>()
                .AddSingleton<IResourceService, ResourceService>()
                .AddSingleton(typeof(ISubject<>), typeof(Subject<>))
                .AddSingleton<IApplicationState, ApplicationState>()
                .AddSingleton<ICommandActions, CommandActions>()
                .AddSingleton<ICommandParser, CommandParser>()
                .AddSingleton<IApplicationStateValueSetter, ApplicationStateValueSetter>()
                .AddSingleton<IProcessService, ProcessService>();
        }
    }
}
