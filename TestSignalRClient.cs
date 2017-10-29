using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace ClimateMeter.TestClient
{
    public class TestSignalRClient
    {
        private readonly ILogger<TestSignalRClient> _log;
        private readonly HubConnection _connection;

        public TestSignalRClient(ILogger<TestSignalRClient> log)
        {
            _log = log;
            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/socket/device")
                .WithJsonProtocol()
                .WithConsoleLogger()
                .Build();
        }

        public async Task<Guid> StartDevice()
        {
            _log.LogInformation("Starting up test device...");
            
            var registeredTaskSource = new TaskCompletionSource<Guid>();
            
            _connection.On<Guid>("DeviceRegistered", id =>
            {
                registeredTaskSource.SetResult(id);
            });

            await _connection.StartAsync();
            _log.LogInformation("Connected to server");

            await _connection.InvokeAsync("RegisterDevice", "Test Device", "Test description");
            _log.LogInformation("Started registration process");

            Guid deviceId = await registeredTaskSource.Task;
            _log.LogInformation($"Registered with id: {deviceId}");
            
            return deviceId;
        }

        public async Task AddSensorReading(Guid id, decimal temperature, decimal humidity)
        {
            await _connection.InvokeAsync("AddSensorReading", id, 26.0, 43.0);
            _log.LogInformation($"Sent sensor readings: temp - {temperature}, humidity - {humidity}");
        }

        public async Task Disconnect()
        {
            await _connection.DisposeAsync();
            _log.LogInformation("Disconnected");
        }
    }
}