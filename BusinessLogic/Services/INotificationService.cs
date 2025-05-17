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
        /// Суудал захиалагдсан тухай мэдэгдэл илгээх.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="seatNumber">Захиалагдсан суудлын дугаар</param>
        /// <param name="passengerId">Зорчигчийн ID</param>
        /// <returns>Үйлдлийн үр дүн</returns>
        Task NotifySeatAssignedAsync(int flightId, string seatNumber, int passengerId);
    }
} 