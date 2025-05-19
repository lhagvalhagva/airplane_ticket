using DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    /// <summary>
    /// Зорчигчидтай ажиллах үйлчилгээний интерфейс.
    /// </summary>
    public interface IPassengerService
    {
        /// <summary>
        /// Бүх зорчигчдын жагсаалтыг авах.
        /// </summary>
        /// <param name="nameFilter">Нэрээр шүүх (заавал биш)</param>
        /// <param name="passportNumber">Паспортын дугаараар шүүх (заавал биш)</param>
        /// <returns>Зорчигчдын жагсаалт</returns>
        Task<IEnumerable<Passenger>> GetAllPassengersAsync(string? nameFilter = null, string? passportNumber = null);

        /// <summary>
        /// ID-аар зорчигчийн мэдээлэл авах.
        /// </summary>
        /// <param name="id">Зорчигчийн ID</param>
        /// <returns>Зорчигчийн мэдээлэл, эсвэл null</returns>
        Task<Passenger?> GetPassengerByIdAsync(int id);

        /// <summary>
        /// Паспортын дугаараар зорчигчийн мэдээлэл авах.
        /// </summary>
        /// <param name="passportNumber">Паспортын дугаар</param>
        /// <returns>Зорчигчийн мэдээлэл, эсвэл null</returns>
        Task<Passenger?> GetPassengerByPassportNumberAsync(string passportNumber);

        /// <summary>
        /// Шинэ зорчигч үүсгэх.
        /// </summary>
        /// <param name="passenger">Зорчигчийн мэдээлэл</param>
        /// <returns>Үйлдлийн үр дүн</returns>
        Task AddPassengerAsync(Passenger passenger);

        /// <summary>
        /// Зорчигчийн мэдээллийг шинэчлэх.
        /// </summary>
        /// <param name="passenger">Зорчигчийн шинэчлэгдсэн мэдээлэл</param>
        /// <returns>Үйлдлийн үр дүн</returns>
        Task UpdatePassengerAsync(Passenger passenger);

        /// <summary>
        /// Зорчигч устгах.
        /// </summary>
        /// <param name="id">Зорчигчийн ID</param>
        /// <returns>Үйлдлийн үр дүн</returns>
        Task DeletePassengerAsync(int id);

        /// <summary>
        /// Зорчигч байгаа эсэхийг шалгах.
        /// </summary>
        /// <param name="passengerId">Зорчигчийн ID</param>
        /// <returns>Зорчигч байгаа эсэх</returns>
        Task<bool> PassengerExistsAsync(int passengerId);


    }
}