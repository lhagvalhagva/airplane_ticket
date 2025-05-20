using DataAccess.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SocketServerLibrary;

namespace BusinessLogic.Services
{
    public class NotificationService : INotificationService
    {
        private readonly WebSocketServer? _webSocketServer;
        private readonly IHubContext<FlightHub>? _flightHubContext;
        private readonly NotificationTarget _defaultTarget;
        
        public NotificationService(WebSocketServer? webSocketServer = null, IHubContext<FlightHub>? flightHubContext = null)
        {
            _webSocketServer = webSocketServer;
            _flightHubContext = flightHubContext;
            _defaultTarget = NotificationTarget.Both; // Дефолтоор хоёр системийг хэрэглэх
        }

        public async Task NotifyFlightStatusChangedAsync(int flightId, FlightStatus newStatus)
        {
            Console.WriteLine($"Нислэг ID: {flightId}, төлөв {newStatus} болж өөрчлөгдлөө");
            
            var data = new
            {
                FlightId = flightId,
                NewStatus = newStatus,
                StatusName = newStatus.ToString(),
                Timestamp = DateTime.UtcNow
            };
            
            // SignalR технологи ашиглах
            await SendNotificationAsync("FlightStatusChanged", data, NotificationTarget.SignalR);
        }

        public async Task NotifySeatAssignedAsync(int flightId, string seatNumber, int passengerId)
        {
            Console.WriteLine($"Нислэг ID: {flightId}, {seatNumber} суудал зорчигч ID: {passengerId}-д хуваарилагдлаа");
            
            var seatData = new 
            {
                FlightId = flightId,
                SeatNumber = seatNumber,
                PassengerId = passengerId,
                Timestamp = DateTime.UtcNow
            };
            
            // WebSocket технологи ашиглах
            await SendNotificationAsync("SeatAssigned", seatData, NotificationTarget.WebSocket);
        }
        
        /// <summary>
        /// Хувийн мэдэгдэл илгээх метод - төрлөөс хамааран тохирох систем рүү илгээнэ
        /// </summary>
        private async Task SendNotificationAsync(string eventName, object data, NotificationTarget target = NotificationTarget.Both)
        {
            // Төрлөөс хамааран илгээх системийг сонгох
            bool useSignalR = target == NotificationTarget.SignalR || target == NotificationTarget.Both;
            bool useWebSocket = target == NotificationTarget.WebSocket || target == NotificationTarget.Both;
            
            if (useSignalR && _flightHubContext != null)
            {
                await _flightHubContext.Clients.All.SendAsync(eventName, data);
            }
            
            if (useWebSocket && _webSocketServer != null)
            {
                await _webSocketServer.SendMessageToAllAsync(eventName, data);
            }
        }
        
        // public async Task NotifyPassengerRegisteredAsync(int flightId, int passengerId)
        // {
        //     Console.WriteLine($"Зорчигч ID: {passengerId} нь нислэг ID: {flightId} дээр бүртгэгдлээ");
            
        //     var data = new
        //     {
        //         FlightId = flightId,
        //         PassengerId = passengerId,
        //         Timestamp = DateTime.UtcNow
        //     };
            
        //     await SendNotificationAsync("PassengerRegistered", data, NotificationTarget.Both);
        // }
        
        // public async Task NotifyPassengerUnregisteredAsync(int flightId, int passengerId)
        // {
        //     Console.WriteLine($"Зорчигч ID: {passengerId} нь нислэг ID: {flightId} дээр хасагдлаа");
            
        //     var data = new
        //     {
        //         FlightId = flightId,
        //         PassengerId = passengerId,
        //         Timestamp = DateTime.UtcNow
        //     };
            
        //     await SendNotificationAsync("PassengerUnregistered", data, NotificationTarget.Both);
        // }
    }
}