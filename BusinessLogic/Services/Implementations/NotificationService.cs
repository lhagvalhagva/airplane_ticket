using DataAccess.Models;
using Microsoft.AspNetCore.SignalR;
using SignalRHubLibrary;
using System;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationTarget _defaultTarget;
        private readonly IHubContext<FlightHub>? _hubContext;
        
        public NotificationService(IHubContext<FlightHub>? hubContext = null)
        {
            _defaultTarget = NotificationTarget.Both;
            _hubContext = hubContext;
        }

        public async Task NotifyFlightStatusChangedAsync(int flightId, FlightStatus newStatus)
        {
            Console.WriteLine($"Нислэг ID: {flightId}, төлөв {newStatus} болж өөрчлөгдлөө");
            
            // Нислэгийн дугаарыг олох логик (жишээ болгож авлаа)
            string flightNumber = $"MGL{flightId}"; // Жинхэнэ апп дээр нислэгийн дугаарыг repository-оос авах хэрэгтэй
            
            // SignalR Hub руу мэдээлэл илгээх
            if (_hubContext != null)
            {
                await _hubContext.Clients.Group(flightNumber)
                    .SendAsync("FlightStatusUpdated", flightNumber, newStatus.ToString());
                
                Console.WriteLine($"SignalR ашиглан {flightNumber} нислэгийн төлөв өөрчлөлтийг мэдэгдлээ");
            }
            else
            {
                Console.WriteLine("SignalR HubContext олдсонгүй - мэдэгдэл илгээгдсэнгүй");
            }
        }

        public async Task NotifySeatAssignedAsync(int flightId, string seatNumber, int passengerId)
        {
            Console.WriteLine($"Нислэг ID: {flightId}, {seatNumber} суудал зорчигч ID: {passengerId}-д хуваарилагдлаа");
            
            // Энд WebSocket ашиглах код байна
            await Task.CompletedTask;
        }
        
        // public async Task NotifyPassengerRegisteredAsync(int flightId, int passengerId){     Console.WriteLine($"Зорчигч ID: {passengerId} нь нислэг ID: {flightId} дээр бүртгэгдлээ");     await Task.CompletedTask;}
        // public async Task NotifyPassengerUnregisteredAsync(int flightId, int passengerId){     Console.WriteLine($"Зорчигч ID: {passengerId} нь нислэг ID: {flightId} дээр хасагдлаа");     await Task.CompletedTask;}
    }
}