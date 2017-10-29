using DarkXaHTeP.CommandLine;
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
                .UseStartup<Startup>()
                .Build();

            return host.Run(args);
        }
    }
}