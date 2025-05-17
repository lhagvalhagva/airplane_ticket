using DataAccess.Models;
using System;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    // Одоогоор нотификэйшн сервис нь хоосон байна
    // Дараа RestApi болон SignalRHub-ийг бүрэн тодорхойлсны дараа хэрэгжүүлнэ
    // Энэ сервис нь WebSocket болон SignalR хэрэгжүүлэлтүүдрүү мэдэгдэл илгээх үүрэгтэй
    public class NotificationService : INotificationService
    {
        private readonly IFlightService _flightService;
        private readonly IPassengerService _passengerService;

        public NotificationService(IFlightService flightService, IPassengerService passengerService)
        {
            _flightService = flightService;
            _passengerService = passengerService;
        }

        public async Task NotifyFlightStatusChangedAsync(int flightId, FlightStatus newStatus)
        {
            // Нислэгийн төлөв өөрчлөгдсөн үед бүх клиент руу WebSocket болон SignalR-ээр мэдэгдэл илгээнэ
            var flight = await _flightService.GetFlightByIdAsync(flightId);
            if (flight == null)
                throw new KeyNotFoundException($"Нислэг {flightId} ID-тай олдсонгүй.");

            Console.WriteLine($"Нислэг {flight.FlightNumber} төлөв {newStatus} болж өөрчлөгдлөө");
            
            // TODO: WebSocket notifications дараа хэрэгжүүлнэ
            // TODO: SignalR notifications дараа хэрэгжүүлнэ
        }

        public async Task NotifyBoardingPassIssuedAsync(BoardingPass boardingPass)
        {
            // Нисэх хуудас авсан үед клиентүүд рүү мэдэгдэл илгээнэ
            var flight = await _flightService.GetFlightByIdAsync(boardingPass.FlightId);
            var passenger = await _passengerService.GetPassengerByIdAsync(boardingPass.PassengerId);
            
            if (flight == null || passenger == null)
                throw new KeyNotFoundException("Нислэг эсвэл зорчигч олдсонгүй.");

            Console.WriteLine($"Зорчигч {passenger.LastName} {passenger.FirstName} нь {flight.FlightNumber} нислэгийн хуудас авлаа");
            
            // TODO: WebSocket notifications дараа хэрэгжүүлнэ  
            // TODO: SignalR notifications дараа хэрэгжүүлнэ
        }

        public async Task NotifySeatAssignedAsync(int flightId, string seatNumber, int passengerId)
        {
            // Суудал хуваарилахад нэгэн зэрэг бусад клиентүүд дээр суудлыг хаах
            var flight = await _flightService.GetFlightByIdAsync(flightId);
            var passenger = await _passengerService.GetPassengerByIdAsync(passengerId);
            
            if (flight == null || passenger == null)
                throw new KeyNotFoundException("Нислэг эсвэл зорчигч олдсонгүй.");

            Console.WriteLine($"{flight.FlightNumber} нислэгийн {seatNumber} суудал зорчигч {passenger.LastName} {passenger.FirstName}-д хуваарилагдлаа");
            
            // TODO: WebSocket notifications дараа хэрэгжүүлнэ
            // TODO: SignalR notifications дараа хэрэгжүүлнэ
        }
    }
} 