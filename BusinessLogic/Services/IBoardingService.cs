using DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    /// <summary>
    /// Онгоцонд суух, бүртгэлийн үйл явцыг удирдах үйлчилгээний интерфейс.
    /// </summary>
    public interface IBoardingService
    {
        /// <summary>
        /// Зорчигчийг нислэгт бүртгэх.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="passportNumber">Зорчигчийн паспортын дугаар</param>
        /// <param name="seatNumber">Сонгосон суудлын дугаар</param>
        /// <returns>Үүсгэсэн онгоцны бүртгэлийн мэдээлэл</returns>
        Task<BoardingPass> CheckInPassengerAsync(int flightId, string passportNumber, string seatNumber);

        /// <summary>
        /// Тодорхой нислэгийн боломжтой суудлуудын жагсаалтыг авах.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <returns>Боломжтой суудлуудын жагсаалт</returns>
        Task<IEnumerable<Seat>> GetAvailableSeatsAsync(int flightId);

        /// <summary>
        /// Онгоцны бүртгэлийн мэдээллийг ID-аар авах.
        /// </summary>
        /// <param name="boardingPassId">Онгоцны бүртгэлийн ID</param>
        /// <returns>Онгоцны бүртгэлийн мэдээлэл, эсвэл null</returns>
        Task<BoardingPass?> GetBoardingPassAsync(int boardingPassId);

        /// <summary>
        /// Тодорхой нислэгийн бүх онгоцны бүртгэлийн жагсаалтыг авах.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <returns>Онгоцны бүртгэлийн жагсаалт</returns>
        Task<IEnumerable<BoardingPass>> GetBoardingPassesByFlightAsync(int flightId);

        /// <summary>
        /// Хэрэглэгчид суудал оноох.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="passengerId">Зорчигчийн ID</param>
        /// <param name="seatNumber">Сонгосон суудлын дугаар</param>
        /// <returns>Суудал оноогдсон бол труе, бусад тохиолдолд алдаа</returns>
        Task<bool> AssignSeatToPassengerAsync(int flightId, int passengerId, string seatNumber);

        /// <summary>
        /// Тодорхой нислэгийн суудлын боломжит байдлыг шалгах.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="seatNumber">Суудлын дугаар</param>
        /// <returns>Суудал боломжтой эсэх</returns>
        Task<bool> IsSeatAvailableAsync(int flightId, string seatNumber);
    }
} 