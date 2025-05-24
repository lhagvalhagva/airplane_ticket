using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace FlightDashboard.Services
{
    public interface ISignalRService
    {
        Task StartConnectionAsync();
        Task StopConnectionAsync();
        bool IsConnected { get; }
        event Action<int, int> FlightStatusChanged;
        string ConnectionStatus { get; }
    }

    public class SignalRService : ISignalRService
    {
        private HubConnection? _hubConnection;
        private readonly string _hubUrl;

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
        public string ConnectionStatus => _hubConnection?.State.ToString() ?? "Disconnected";

        public event Action<int, int> FlightStatusChanged;

        public SignalRService(IConfiguration configuration)
        {
            _hubUrl = configuration["ApiSettings:SignalRUrl"] ?? "http://10.3.132.225:5027/flightHub";
        }

        public async Task StartConnectionAsync()
        {
            try
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(_hubUrl)
                    .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) })
                    .Build();

                // Нислэгийн төлөв өөрчлөгдсөн үед дуудагдах
                _hubConnection.On<int, int>("NotifyFlightStatusChanged", (flightId, newStatus) =>
                {
                    Console.WriteLine($"SignalR: Flight {flightId} status changed to {newStatus}");
                    FlightStatusChanged?.Invoke(flightId, newStatus);
                });

                await _hubConnection.StartAsync();
                Console.WriteLine($"SignalR холболт амжилттай: {_hubUrl}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR холболтод алдаа: {ex.Message}");
                throw;
            }
        }

        public async Task StopConnectionAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
            }
        }
    }
} 