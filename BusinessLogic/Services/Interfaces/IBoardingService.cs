using DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    /// <summary>
    /// Зорчигчийн тасалбар бүртгэл, check-in үйл явцыг удирдах үйлчилгээний интерфейс.
    /// </summary>
    public interface IBoardingService
    {
        /// <summary>
        /// Тодорхой ID-тай тасалбарыг авах.
        /// </summary>
        /// <param name="id">Тасалбарын ID</param>
        /// <returns>Тасалбарын мэдээлэл, эсвэл null</returns>
        Task<BoardingPass?> GetBoardingPassByIdAsync(int id);

        /// <summary>
        /// Тодорхой нислэг, зорчигчийн тасалбарыг авах.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="passengerId">Зорчигчийн ID</param>
        /// <returns>Тасалбарын мэдээлэл, эсвэл null</returns>
        Task<BoardingPass?> GetBoardingPassByFlightAndPassengerAsync(int flightId, int passengerId);

        /// <summary>
        /// Тасалбар үүсгэх (Check-in хийх).
        /// </summary>
        /// <param name="boardingPass">Үүсгэх тасалбарын мэдээлэл</param>
        /// <returns>Үүсгэсэн тасалбарын мэдээлэл</returns>
        Task<BoardingPass> CreateBoardingPassAsync(BoardingPass boardingPass);

        /// <summary>
        /// Суудлын мэдээлэл ашиглан тасалбар үүсгэх (Нэгдсэн арга).
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="passengerId">Зорчигчийн ID</param>
        /// <param name="seatNumber">Суудлын дугаар</param>
        /// <returns>Үүсгэсэн тасалбарын мэдээлэл</returns>
        Task<BoardingPass> CreateBoardingPassWithSeatInfoAsync(int flightId, int passengerId, string seatNumber);

        /// <summary>
        /// Тасалбар устгах.
        /// </summary>
        /// <param name="id">Тасалбарын ID</param>
        /// <returns>Амжилттай устгасан эсэх</returns>
        Task DeleteBoardingPassAsync(int id);

        /// <summary>
        /// Тухайн нислэгийн бүх тасалбаруудыг авах.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <returns>Нислэгийн бүх тасалбаруудын жагсаалт</returns>
        Task<IEnumerable<BoardingPass>> GetBoardingPassesByFlightAsync(int flightId);

        /// <summary>
        /// Тасалбар үүсгэхэд хэрэгтэй өгөгдлүүдийг шалгах.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="passengerId">Зорчигчийн ID</param>
        /// <param name="seatId">Суудлын ID</param>
        /// <returns>Шалгалтын үр дүн</returns>
        Task<(bool IsValid, string? ErrorMessage)> ValidateBoardingPassDataAsync(int flightId, int passengerId, int seatId);

        /// <summary>
        /// Суудлын дугаараар суудлын ID-г олох.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="seatNumber">Суудлын дугаар</param>
        /// <returns>Суудлын ID эсвэл null</returns>
        Task<int?> GetSeatIdByFlightAndNumberAsync(int flightId, string seatNumber);
    }
}