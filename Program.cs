using DarkXaHTeP.CommandLine;
using DarkXaHTeP.Extensions.Configuration.Consul;
using Microsoft.Extensions.Logging;

namespace ClimateMeter.TestClient
{
    static class Program
    {
        static int Main(string[] args)
        {
            ICommandLineHost host = new CommandLineHostBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                })
                .ConfigureAppConfiguration(config =>
                {
                    config.AddConsul("ClimateMeter.TestClient");
                })
                .UseStartup<Startup>()
                .Build();

            return host.Run(args);
        }
    }
}