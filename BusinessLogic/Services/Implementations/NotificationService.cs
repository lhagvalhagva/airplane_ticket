using DataAccess.Models;
using System;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationTarget _defaultTarget;
        
        public NotificationService()
        {
            _defaultTarget = NotificationTarget.Both; // Дефолтоор хоёр системийг хэрэглэх
        }

        public async Task NotifyFlightStatusChangedAsync(int flightId, FlightStatus newStatus)
        {
            Console.WriteLine($"Нислэг ID: {flightId}, төлөв {newStatus} болж өөрчлөгдлөө");
            
            // Энд SignalR ашиглах код байна
            await Task.CompletedTask;
        }

        public async Task NotifySeatAssignedAsync(int flightId, string seatNumber, int passengerId)
        {
            Console.WriteLine($"Нислэг ID: {flightId}, {seatNumber} суудал зорчигч ID: {passengerId}-д хуваарилагдлаа");
            
            // Энд WebSocket ашиглах код байна
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