using DataAccess.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using SignalRHubLibrary;
using SocketServerLibrary;
using System;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class NotificationService : INotificationService
    {
        private IHubContext<FlightHub> _flightHubContext;
        private HubConnection _hubConnection;
        private readonly string _hubUrl = "http://localhost:5000/flightHub";
        
        public NotificationService()
        {
            InitializeSignalRConnection();
            InitializeWebSocketServer();
        }
        
        private void InitializeWebSocketServer()
        {
            try
            {
                // Синглтон WebSocketServer объект авах
                WebSocketServer instance = WebSocketServer.Instance;
                
                if (instance.HasStarted)
                {
                    Console.WriteLine("WebSocket server is already running");
                    return;
                }
                
                instance.Start();
                Console.WriteLine("WebSocket server initialized successfully at port 9009");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing WebSocket server: {ex.Message}");
            }
        }
        
        private async void InitializeSignalRConnection()
        {
            try
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(_hubUrl)
                    .WithAutomaticReconnect()
                    .Build();
                
                await _hubConnection.StartAsync();
                Console.WriteLine("SignalR connection successful: " + _hubUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating SignalR connection: {ex.Message}");
            }
        }

        /// <summary>
        /// Нислэгийн төлөв өөрчлөгдөх үед дуудагдах арга. SignalR ашиглан бүх хэрэглэгчдэд мэдэгдэнэ.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="newStatus">Шинэ төлөв</param>
        public async Task NotifyFlightStatusChangedAsync(int flightId, FlightStatus newStatus)
        {
            Console.WriteLine($"========== SENDING FLIGHT STATUS CHANGE NOTIFICATION ===========");
            Console.WriteLine($"NotificationService: Flight ID: {flightId}, status changed to: {newStatus} (value: {(int)newStatus})");
            
            try
            {
                    Console.WriteLine($"Sending notification using HubConnection... (State: {_hubConnection.State})");
                    await _hubConnection.SendAsync("NotifyFlightStatusChanged", flightId, (int)newStatus);
                    Console.WriteLine("SignalR notification sent successfully (using HubConnection)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CRITICAL ERROR: Failed to send SignalR notification: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Суудал хуваарилах үед дуудагдах арга. WebSocket ашиглан мэдэгдэл илгээнэ.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="seatNumber">Суудлын дугаар</param>
        /// <param name="passengerId">Зорчигчийн ID</param>
        public async Task NotifySeatAssignedAsync(int flightId, string seatNumber, int passengerId)
        {
            Console.WriteLine($"========== SENDING SEAT ASSIGNMENT NOTIFICATION ===========");
            Console.WriteLine($"NotificationService: Flight ID: {flightId}, Seat: {seatNumber}, Passenger ID: {passengerId}");
            try
            {
                WebSocketServer instance = WebSocketServer.Instance;
                
                    Console.WriteLine("Sending notification using WebSocket...");
                    instance.NotifySeatAssigned(flightId, seatNumber, passengerId);
                    Console.WriteLine("WebSocket notification sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Failed to send seat assignment notification: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
            
            await Task.CompletedTask;
        }
    }
}