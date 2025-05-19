using DataAccess.Models;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    /// <summary>
    /// Мэдэгдэл илгээх үйлчилгээний интерфейс.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Нислэгийн төлөв өөрчлөгдсөн тухай мэдэгдэл илгээх.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="newStatus">Нислэгийн шинэ төлөв</param>
        /// <returns>Үйлдлийн үр дүн</returns>
        Task NotifyFlightStatusChangedAsync(int flightId, FlightStatus newStatus);

        /// <summary>
        /// Онгоцны бүртгэл үүсгэсэн тухай мэдэгдэл илгээх.
        /// </summary>
        /// <param name="boardingPass">Онгоцны бүртгэлийн мэдээлэл</param>
        /// <returns>Үйлдлийн үр дүн</returns>
        Task NotifyBoardingPassIssuedAsync(BoardingPass boardingPass);

        /// <summary>
        /// Зорчигч нислэгт бүртгэгдсэн тухай мэдэгдэл илгээх.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="passengerId">Зорчигчийн ID</param>
        /// <returns>Үйлдлийн үр дүн</returns>
        Task NotifyPassengerRegisteredAsync(int flightId, int passengerId);
        
        /// <summary>
        /// Зорчигч нислэгээс хасагдсан тухай мэдэгдэл илгээх.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="passengerId">Зорчигчийн ID</param>
        /// <returns>Үйлдлийн үр дүн</returns>
        Task NotifyPassengerUnregisteredAsync(int flightId, int passengerId);
    }
}