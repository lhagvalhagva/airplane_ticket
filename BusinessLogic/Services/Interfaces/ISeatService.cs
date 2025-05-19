using DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    /// <summary>
    /// Суудлуудтай ажиллах үйлчилгээний интерфейс.
    /// </summary>
    public interface ISeatService
    {
        /// <summary>
        /// Тодорхой нислэгийн бүх суудлуудын жагсаалтыг авах.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="isOccupied">Суудал захиалагдсан эсэх (null бол бүгдийг)</param>
        /// <returns>Суудлуудын жагсаалт</returns>
        Task<IEnumerable<Seat>> GetSeatsByFlightIdAsync(int flightId, bool? isOccupied = null);

        /// <summary>
        /// Тодорхой нислэгийн боломжтой (сул) суудлуудын жагсаалтыг авах.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <returns>Боломжтой суудлуудын жагсаалт</returns>
        Task<IEnumerable<Seat>> GetAvailableSeatsByFlightIdAsync(int flightId);

        /// <summary>
        /// Суудлыг ID-аар авах.
        /// </summary>
        /// <param name="seatId">Суудлын ID</param>
        /// <returns>Суудлын мэдээлэл</returns>
        Task<Seat?> GetSeatByIdAsync(int seatId);

        /// <summary>
        /// Зорчигчид суудал оноох.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="passengerId">Зорчигчийн ID</param>
        /// <param name="seatId">Суудлын ID</param>
        /// <returns>Амжилттай оноогдсон эсэх үр дүн</returns>
        Task<bool> AssignSeatAsync(int flightId, int passengerId, int seatId);

        /// <summary>
        /// Суудлыг чөлөөлөх (захиалга цуцлах).
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="seatId">Суудлын ID</param>
        /// <returns>Суудал амжилттай чөлөөлөгдсөн эсэх</returns>
        Task<bool> ReleaseSeatAsync(int flightId, int seatId);

        /// <summary>
        /// Суудал байгаа эсэхийг шалгах.
        /// </summary>
        /// <param name="seatId">Суудлын ID</param>
        /// <returns>Суудал байгаа эсэх</returns>
        Task<bool> SeatExistsAsync(int seatId);

        /// <summary>
        /// Суудал сул эсэхийг шалгах.
        /// </summary>
        /// <param name="seatId">Суудлын ID</param>
        /// <returns>Суудал сул эсэх</returns>
        Task<bool> IsSeatAvailableAsync(int seatId);
    }
}
