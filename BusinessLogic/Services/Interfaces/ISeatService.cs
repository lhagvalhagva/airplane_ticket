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
        /// Суудлыг чөлөөлөх эсвэл өөр зорчигчид шилжүүлэх.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="seatId">Суудлын ID</param>
        /// <param name="newPassengerId">Шинэ зорчигчийн ID (заавал биш)</param>
        /// <returns>Суудал амжилттай чөлөөлөгдсөн эсвэл шилжүүлэгдсэн эсэх</returns>
        Task<bool> ReleaseSeatAsync(int flightId, int seatId, int? newPassengerId);

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
        
        /// <summary>
        /// Зорчигчийн суудлыг олох.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="passengerId">Зорчигчийн ID</param>
        /// <returns>Зорчигчийн суудлын мэдээлэл, эсвэл null</returns>
        Task<Seat?> GetPassengerSeatAsync(int flightId, int passengerId);

        /// <summary>
        /// Add a new seat to the flight
        /// </summary>
        /// <param name="seat">The seat to add</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task AddSeatAsync(Seat seat);
    }
}
