using Microsoft.Extensions.Hosting;
using MultiStackServiceHost.Shared.Contracts;
using MultiStackServiceHost.Shared.Hosts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiStackServiceHost.Shared.Extensions
{
    public static class HostExtensions
    {
        public static IAppHostBuilder ConfigureAppHost(this IHostBuilder hostBuilder, Action<IAppHostBuilder> configureAppHost = null)
        {
            var appHostBuilder = new AppHostBuilder(hostBuilder);
            configureAppHost?.Invoke(appHostBuilder);
            return appHostBuilder;
        }

        public static IAppHost<TStartup> UseStartup<TStartup>(
            this IAppHost<TStartup> host, 
            Action<TStartup> startingMethod = null, 
            Func<TStartup, CancellationToken, Task> startingAsyncMethod = null)
        {
            host.StartAction = startingMethod;
            host.StartActionAsync = startingAsyncMethod;
            return host;
        }
    }
}
