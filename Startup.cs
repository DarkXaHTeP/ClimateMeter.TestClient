using System;
using System.Threading.Tasks;
using DarkXaHTeP.CommandLine;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ClimateMeter.TestClient
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public void Configure(IApplicationBuilder builder, TestSignalRClient client, ILogger<Startup> log)
        {
            builder.OnExecute(async () =>
            {
                var deviceId = await client.StartDevice();
                await Task.Delay(2000);
                await client.AddSensorReading(deviceId, 24.5m, 68.3m);
                await client.Disconnect();
                return 0;
            });

            builder.Command("interval", command =>
            {
                var everyOption = command.Option("--every", "Interval in seconds", CommandOptionType.SingleValue);
                var iterationsOption = command.Option(
                    "--iterations", "Amount of iterations to run",
                    CommandOptionType.SingleValue);

                command.OnExecute(async () =>
                {
                    int every = everyOption.HasValue() ? Convert.ToInt32(everyOption.Value()) : 10;
                    int iterations = iterationsOption.HasValue() ? Convert.ToInt32(iterationsOption.Value()) : Int32.MaxValue;

                    Guid deviceId = await client.StartDevice();

                    for (int i = 0; i < iterations; i++)
                    {
                        await client.AddSensorReading(deviceId, 23m, 45m);
                        await Task.Delay(every * 1000);
                    }

                    await client.Disconnect();
                    
                    return 0;
                });
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AuthenticationSettings>(_configuration.GetSection("Authentication"));
            services.AddSingleton<TestSignalRClient>();
        }
    }
}