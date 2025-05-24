using DataAccess.Models;
using Microsoft.AspNetCore.SignalR;
using SignalRHubLibrary;
using SocketServerLibrary;
using System;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<FlightHub> _flightHubContext;
        
        public NotificationService(IHubContext<FlightHub> flightHubContext)
        {
            _flightHubContext = flightHubContext;
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
                Console.WriteLine($"Сервер эхлүүлэхэд алдаа гарлаа: {ex.Message}");
                // Port conflict байвал сервер аль хэдийн ажиллаж байна гэж үзнэ
                if (ex.Message.Contains("Only one usage of each socket address"))
                {
                    Console.WriteLine("WebSocket server is already running on port 9009");
                }
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
                if (_flightHubContext != null)
                {
                    Console.WriteLine($"Sending SignalR notification using IHubContext...");
                    await _flightHubContext.Clients.All.SendAsync("FlightStatusChanged", flightId, (int)newStatus);
                    Console.WriteLine("SignalR notification sent successfully!");
                }
                else
                {
                    Console.WriteLine("IHubContext is null. Notification not sent.");
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