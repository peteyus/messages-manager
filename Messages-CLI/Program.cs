using Core.Models.Application;
using Messages.CLI.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.Reflection;
namespace Messages.CLI
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var ServiceProvider = BuildServiceProvider();
            var executor = ServiceProvider.GetRequiredService<ICliExecutor>();

            return await executor.ExecuteAsync(args).ConfigureAwait(false);
        }

        private static IServiceProvider BuildServiceProvider()
        {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

            services.RegisterConfiguration();
            services.RegisterServices();
            services.RegisterCommands();

            return services.BuildServiceProvider();
        }
    }
}