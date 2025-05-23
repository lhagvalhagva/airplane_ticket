using DataAccess.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using SignalRHubLibrary;
using SocketServerLibrary;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationTarget _defaultTarget;
        private IHubContext<FlightHub> _flightHubContext;
        private HubConnection _hubConnection;
        private readonly string _hubUrl = "http://localhost:5000/flightHub";
        // WebSocketServer синглтон байх шаардлагагүй
        
        public NotificationService()
        {
            _defaultTarget = NotificationTarget.Both;
            InitializeSignalRConnection();
            InitializeWebSocketServer();
        }
        
        private void InitializeWebSocketServer()
        {
            try
            {
                // Синглтон WebSocketServer объект авах
                WebSocketServer instance = WebSocketServer.Instance;
                
                // Хэрэв сервер эхлээд байвал дахин эхлүүлэх шаардлагагүй
                if (instance.HasStarted)
                {
                    Console.WriteLine("WebSocket server is already running");
                    return;
                }
                
                // Серверийг эхлүүлэх
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
            
            // SignalR ашиглан бүх хэрэглэгчдэд мэдээлэл илгээх
            try
            {
                if (_flightHubContext != null)
                {
                    Console.WriteLine("Sending notification using HubContext...");
                    await _flightHubContext.Clients.All.SendAsync("FlightStatusChanged", flightId, (int)newStatus);
                    Console.WriteLine("SignalR notification sent successfully (using HubContext)");
                }
                else if (_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
                {
                    Console.WriteLine($"Sending notification using HubConnection... (State: {_hubConnection.State})");
                    // Use direct HubConnection if HubContext is not available
                    await _hubConnection.SendAsync("NotifyFlightStatusChanged", flightId, (int)newStatus);
                    Console.WriteLine("SignalR notification sent successfully (using HubConnection)");
                }
                else
                {
                    Console.WriteLine($"ERROR: No SignalR connection available! HubContext={_flightHubContext != null}, HubConnection={_hubConnection != null}");
                    
                    // Try to create a new connection if none exists
                    if (_hubConnection == null)
                    {
                        Console.WriteLine("Attempting to create a new connection...");
                        InitializeSignalRConnection();
                        Console.WriteLine("Waiting 3 seconds for connection to stabilize...");
                        await Task.Delay(3000); // Wait for connection to stabilize
                        
                        if (_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
                        {
                            Console.WriteLine("Sending notification through new connection...");
                            await _hubConnection.SendAsync("NotifyFlightStatusChanged", flightId, (int)newStatus);
                            Console.WriteLine("Notification sent through new connection!");
                        }
                    }
                }
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
                // Синглтон WebSocketServer объект авах
                WebSocketServer instance = WebSocketServer.Instance;
                
                // WebSocket ашиглан мэдэгдэл илгээх
                if (instance.HasStarted)
                {
                    Console.WriteLine("Sending notification using WebSocket...");
                    instance.NotifySeatAssigned(flightId, seatNumber, passengerId);
                    Console.WriteLine("WebSocket notification sent successfully");
                }
                else
                {
                    Console.WriteLine("WARNING: WebSocket server is not available, initializing...");
                    InitializeWebSocketServer();
                    
                    // Хэрэв серверийг шинээр эхлүүлсэн бол дахин илгээх оролдлого хийх
                    if (instance.HasStarted)
                    {
                        Console.WriteLine("Retrying notification after WebSocket initialization...");
                        instance.NotifySeatAssigned(flightId, seatNumber, passengerId);
                        Console.WriteLine("WebSocket notification sent successfully after retry");
                    }
                    else
                    {
                        Console.WriteLine("ERROR: Failed to initialize WebSocket server, notification not sent");
                    }
                }
                
                // SignalR ашиглан мэдэгдэл илгээх (опшнл)
                if (_defaultTarget == NotificationTarget.Both || _defaultTarget == NotificationTarget.SignalR)
                {
                    if (_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
                    {
                        Console.WriteLine("Sending seat assignment via SignalR as well...");
                        await _hubConnection.SendAsync("NotifySeatAssigned", flightId, seatNumber, passengerId);
                        Console.WriteLine("SignalR notification for seat assignment sent");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Failed to send seat assignment notification: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
            
            await Task.CompletedTask;
        }
    }
    
    public enum NotificationTarget
    {
        WebSocket,
        SignalR,
        Both
    }
}