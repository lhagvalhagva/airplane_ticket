using DataAccess.Models;
using SocketServerLibrary;
using System;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class NotificationService : INotificationService
    {
        private readonly WebSocketServer _socketServer;
        private bool _serverStarted = false;
        
        public NotificationService()
        {
            _socketServer = new WebSocketServer();
        }

        public async Task NotifyFlightStatusChangedAsync(int flightId, FlightStatus newStatus)
        {
            Console.WriteLine($"Нислэг ID: {flightId}, төлөв {newStatus} болж өөрчлөгдлөө");
            
            await Task.CompletedTask; 
        }

        public async Task NotifySeatAssignedAsync(int flightId, string seatNumber, int passengerId)
        {
            Console.WriteLine($"Нислэг ID: {flightId}, {seatNumber} суудал зорчигч ID: {passengerId}-д хуваарилагдлаа");
            
            // Энд WebSocket эсвэл SignalR технологи ашиглан бусад клиентуудад мэдэгдэл илгээх
            // Жишээ нь: await _hubContext.Clients.All.SendAsync("SeatAssigned", flightId, seatNumber, passengerId);
            
            await Task.CompletedTask;
        }
        
        public async Task NotifyPassengerRegisteredAsync(int flightId, int passengerId)
        {
            Console.WriteLine($"Зорчигч ID: {passengerId} нь нислэг ID: {flightId} дээр бүртгэгдлээ");
            
            await Task.CompletedTask; 
        }
        
        public async Task NotifyPassengerUnregisteredAsync(int flightId, int passengerId)
        {
            Console.WriteLine($"Зорчигч ID: {passengerId} нь нислэг ID: {flightId} дээр хасагдлаа");
            
            await Task.CompletedTask; 
        }
    }
}