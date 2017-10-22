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

            connection.On<Guid>("OnDeviceRegistered", async id =>
            {
                Console.WriteLine($"Registered with Id: {id}");
                await connection.InvokeAsync("OnSensorReading", id, 26.0, 43.0);
            });

            await connection.StartAsync();

            await connection.InvokeAsync("OnRegisterDevice", "Test Device", "Test description");

            Console.ReadKey();

            await connection.DisposeAsync();
        }
    }
}