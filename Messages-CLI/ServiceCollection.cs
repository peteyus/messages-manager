using Core.Interfaces;
using Messages.CLI.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.Indexing;
using Services.Parsers;
using System.CommandLine;
using System.IO.Abstractions;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Core.Models.Application;
using Messages.CLI.Interfaces;
using Messages.CLI.Commands;
using System.CommandLine.IO;
using System.Net;

namespace Messages.CLI
{
    public static class ServiceCollection
    {
        public static void RegisterConfiguration(this IServiceCollection serviceCollection)
        {
            ConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appsettings.json", false);
            configBuilder.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
            var config = configBuilder.Build();
            var appConfig = new ApplicationConfiguration();
            config.Bind(appConfig);

            serviceCollection.AddSingleton(config);
            serviceCollection.AddSingleton(appConfig);

            // disable validation of HTTPS certs. This is terrible and hacky but eh.
            // TODO PRJ: THIS SHOULD NOT BE RELEASED.
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, errors) => true;
        }

        public static void RegisterServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ICliExecutor, CliExecutor>();
            serviceCollection.AddSingleton<ICommandLineBuilderFactory, CommandLineBuilderFactory>();
            serviceCollection.AddSingleton<ICommandLineBuilderInvoker, CommandLineBuilderInvoker>();

            serviceCollection.AddSingleton<IFileSystem, FileSystem>();
            serviceCollection.AddSingleton<IConsole, SystemConsole>();
            serviceCollection.AddSingleton<IIndexer, ElsaticIndexer>();
            serviceCollection.AddTransient<IMessageParser, FacebookHtmlParser>();
            serviceCollection.AddTransient<IMessageParser, InstagramHtmlParser>();
            serviceCollection.AddTransient<IMessageParser, FacebookJsonParser>();
            serviceCollection.AddTransient<IMessageParser, InstagramJsonParser>();
            serviceCollection.AddTransient<IMessageParser, ZipFileParser>();
            serviceCollection.AddSingleton<IUnzipService, UnzipService>();
            serviceCollection.AddSingleton<IParserDetector, ParserDetector>();
        }

        public static void RegisterCommands(this IServiceCollection serviceCollection)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var fileSystem = serviceProvider.GetRequiredService<IFileSystem>();
            var console = serviceProvider.GetRequiredService<IConsole>();

            var assembly = Assembly.GetExecutingAssembly();
            var baseCommandType = typeof(Command);

            foreach (var pluginType in assembly.GetTypes()
                .Where(t => baseCommandType.IsAssignableFrom(t) && !t.IsAbstract))
            {
                // Associate Command objects to service collection
                serviceCollection.AddSingleton(baseCommandType, pluginType);

                // We also need to add a registration for concrete types
                serviceCollection.AddSingleton(pluginType);

                if (typeof(BaseCommand).IsAssignableFrom(pluginType))
                {
                    serviceCollection.AddSingleton(typeof(BaseCommand), pluginType);
                }
            }
        }

        public static void RegisterLogging(this IServiceCollection serviceCollection)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var logConfiguration = serviceProvider.GetRequiredService<IConfigurationRoot>();

            serviceCollection
                .AddLogging(builder => builder
                    .AddConsoleFormatter(options => options.ExcludeLogLevel = false)
                    .AddConfiguration(logConfiguration.GetSection("Logging")));
        }
    }
}
