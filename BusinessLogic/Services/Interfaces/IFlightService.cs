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
        /// Шинэ нислэг үүсгэх.
        /// </summary>
        /// <param name="flight">Нислэгийн мэдээлэл</param>
        /// <returns>Үйлдлийн үр дүн</returns>
        Task AddFlightAsync(Flight flight);

        /// <summary>
        /// Нислэгийн мэдээллийг шинэчлэх.
        /// </summary>
        /// <param name="flight">Нислэгийн шинэчлэгдсэн мэдээлэл</param>
        /// <returns>Үйлдлийн үр дүн</returns>
        Task UpdateFlightAsync(Flight flight);

        /// <summary>
        /// Нислэг устгах.
        /// </summary>
        /// <param name="id">Нислэгийн ID</param>
        /// <returns>Үйлдлийн үр дүн</returns>
        Task DeleteFlightAsync(int id);

        /// <summary>
        /// Нислэг байгаа эсэхийг шалгах.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <returns>Нислэг байгаа эсэх</returns>
        Task<bool> FlightExistsAsync(int flightId);
    }
}