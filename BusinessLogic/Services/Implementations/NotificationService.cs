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
        // Хамаарлын циклийг арилгахын тулд шууд хамаарлыг арилгаж параметрээр мэдээлэл дамжуулах
        public NotificationService()
        {
            // Хоосон байгуулагч
        }

        public async Task NotifyFlightStatusChangedAsync(int flightId, FlightStatus newStatus)
        {
            // Нислэгийн төлөв өөрчлөгдсөн үед бүх клиент руу WebSocket болон SignalR-ээр мэдэгдэл илгээнэ
            Console.WriteLine($"Нислэг ID: {flightId}, төлөв {newStatus} болж өөрчлөгдлөө");
            
            // Цаашид энд WebSocket/SignalR хэрэгжүүлнэ
            await Task.CompletedTask; 
        }

        public async Task NotifyBoardingPassIssuedAsync(BoardingPass boardingPass)
        {
            // Нисэх хуудас авсан үед клиентүүд рүү мэдэгдэл илгээнэ
            Console.WriteLine($"Тасалбар үүсгэгдсэн: Нислэг ID: {boardingPass.FlightId}, Зорчигч ID: {boardingPass.PassengerId}, Суудал ID: {boardingPass.SeatId}");
            
            // Цаашид энд WebSocket/SignalR хэрэгжүүлнэ
            await Task.CompletedTask; // Асинк методын тулд энгийн зөвхөн
        }

        public async Task NotifySeatAssignedAsync(int flightId, string seatNumber, int passengerId)
        {
            // Суудал хуваарилахад нэгэн зэрэг бусад клиентүүд дээр суудлыг хаах
            Console.WriteLine($"Нислэг ID: {flightId}, {seatNumber} суудал зорчигч ID: {passengerId}-д хуваарилагдлаа");
            
            // Цаашид энд WebSocket/SignalR хэрэгжүүлнэ
            await Task.CompletedTask; // Асинк методын тулд энгийн зөвхөн
        }
        
        public async Task NotifyPassengerRegisteredAsync(int flightId, int passengerId)
        {
            // Зорчигч нислэгт бүртгэгдсэн үед мэдэгдэл илгээнэ
            Console.WriteLine($"Зорчигч ID: {passengerId} нь нислэг ID: {flightId} дээр бүртгэгдлээ");
            
            // Цаашид энд WebSocket/SignalR хэрэгжүүлнэ
            await Task.CompletedTask; // Асинк методын тулд энгийн зөвхөн
        }
        
        public async Task NotifyPassengerUnregisteredAsync(int flightId, int passengerId)
        {
            // Зорчигч нислэгээс хасагдсан үед мэдэгдэл илгээх
            Console.WriteLine($"Зорчигч ID: {passengerId} нь нислэг ID: {flightId} дээр хасагдлаа");
            
            // Цаашид энд WebSocket/SignalR хэрэгжүүлнэ
            await Task.CompletedTask; // Асинк методын тулд энгийн зөвхөн
        }
    }
}