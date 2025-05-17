using DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    /// <summary>
    /// Нислэгүүдтэй ажиллах үйлчилгээний интерфейс.
    /// </summary>
    public interface IFlightService
    {
        /// <summary>
        /// Бүх нислэгүүдийн жагсаалтыг авах.
        /// </summary>
        /// <returns>Нислэгүүдийн жагсаалт</returns>
        Task<IEnumerable<Flight>> GetAllFlightsAsync();

        /// <summary>
        /// ID-аар нислэгийн мэдээлэл авах.
        /// </summary>
        /// <param name="id">Нислэгийн ID</param>
        /// <returns>Нислэгийн мэдээлэл, эсвэл null</returns>
        Task<Flight?> GetFlightByIdAsync(int id);

        /// <summary>
        /// Нислэгийн дугаараар нислэгийн мэдээлэл авах.
        /// </summary>
        /// <param name="flightNumber">Нислэгийн дугаар</param>
        /// <returns>Нислэгийн мэдээлэл, эсвэл null</returns>
        Task<Flight?> GetFlightByNumberAsync(string flightNumber);

        /// <summary>
        /// Шинэ нислэг үүсгэх.
        /// </summary>
        /// <param name="flight">Нислэгийн мэдээлэл</param>
        /// <returns>Үйлдлийн үр дүн</returns>
        Task AddFlightAsync(Flight flight);

        /// <summary>
        /// Нислэгийн төлөвийг шинэчлэх.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="status">Нислэгийн шинэ төлөв</param>
        /// <returns>Үйлдлийн үр дүн</returns>
        Task UpdateFlightStatusAsync(int flightId, FlightStatus status);

        /// <summary>
        /// Нислэг байгаа эсэхийг шалгах.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <returns>Нислэг байгаа эсэх</returns>
        Task<bool> FlightExistsAsync(int flightId);
    }
} 