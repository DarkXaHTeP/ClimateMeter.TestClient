using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace ClimateMeter.TestClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/socket/device")
                .WithJsonProtocol()
                .WithConsoleLogger()
                .Build();

            connection.On<Guid>("DeviceRegistered", async id =>
            {
                Console.WriteLine($"Registered with Id: {id}");
                await connection.InvokeAsync("AddSensorReading", id, 26.0, 43.0);
            });

            await connection.StartAsync();

            await connection.InvokeAsync("RegisterDevice", "Test Device", "Test description");

            Console.ReadKey();

            await connection.DisposeAsync();
        }
    }
}