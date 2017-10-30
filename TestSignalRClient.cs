using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;

namespace ClimateMeter.TestClient
{
    public class TestSignalRClient
    {
        private readonly ILogger<TestSignalRClient> _log;
        private readonly HubConnection _connection;
        private readonly AuthenticationSettings _authSettings;

        public TestSignalRClient(IOptions<AuthenticationSettings> authOptions, ILogger<TestSignalRClient> log)
        {
            _authSettings = authOptions.Value;
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
            
            var credential = new ClientCredential(_authSettings.ClientId, _authSettings.ClientSecret);
            
            var authenticationContext = new AuthenticationContext($"{_authSettings.Instance}/{_authSettings.TenantId}", false);

            var token = authenticationContext.AcquireTokenAsync(_authSettings.Resource, credential);
            _log.LogInformation("Acquired access token for the server");
            _log.LogInformation(JsonConvert.SerializeObject(token, Formatting.Indented));

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