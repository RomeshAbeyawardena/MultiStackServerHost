using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace MultiStackServiceHost.Shared.Contracts
{
    public interface IAppHostBuilder : IHostBuilder
    {
        IAppHost<TStartup> Build<TStartup>()
            where TStartup: class;
        new IAppHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configurationDelegate);
        new IAppHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configurationDelegate);
        new IAppHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configurationDelegate);
    }
}
