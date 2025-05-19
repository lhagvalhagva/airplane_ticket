using DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    /// <summary>
    /// Нислэгт зорчигчдын бүртгэлийг удирдах үйлчилгээний интерфейс
    /// </summary>
    public interface IFlightPassengerService
    {
        /// <summary>
        /// Нислэгийн бүх зорчигчдыг авах
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <returns>Нислэгийн зорчигчдын жагсаалт</returns>
        Task<IEnumerable<Passenger>> GetPassengersByFlightIdAsync(int flightId);

        /// <summary>
        /// Зорчигчийн бүх нислэгүүдийг авах
        /// </summary>
        /// <param name="passengerId">Зорчигчийн ID</param>
        /// <returns>Зорчигчийн нислэгүүдийн жагсаалт</returns>
        Task<IEnumerable<Flight>> GetFlightsByPassengerIdAsync(int passengerId);
        
        /// <summary>
        /// Зорчигчийг нислэгт бүртгэх
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="passengerId">Зорчигчийн ID</param>
        /// <returns>Үүсгэгдсэн FlightPassenger холболт</returns>
        Task<FlightPassenger> RegisterPassengerToFlightAsync(int flightId, int passengerId);
        
        /// <summary>
        /// Зорчигчийг нислэгээс хасах
        /// </summary>
        /// <param name="id">FlightPassenger холболтын ID</param>
        /// <returns>Амжилттай эсэх</returns>
        Task RemovePassengerFromFlightAsync(int id);
        
        /// <summary>
        /// Зорчигчийг нислэгээс хасах
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="passengerId">Зорчигчийн ID</param>
        /// <returns>Амжилттай эсэх</returns>
        Task RemovePassengerFromFlightAsync(int flightId, int passengerId);
        
        /// <summary>
        /// FlightPassenger холболт байгаа эсэхийг шалгах
        /// </summary>
        /// <param name="id">FlightPassenger холболтын ID</param>
        /// <returns>Байгаа эсэх</returns>
        Task<bool> FlightPassengerExistsAsync(int id);
        
        /// <summary>
        /// Зорчигч нислэгт бүртгэлтэй эсэхийг шалгах
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="passengerId">Зорчигчийн ID</param>
        /// <returns>Бүртгэлтэй эсэх</returns>
        Task<bool> PassengerIsRegisteredToFlightAsync(int flightId, int passengerId);
    }
}
