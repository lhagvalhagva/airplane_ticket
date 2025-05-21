using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogic.Services;

namespace RestApi.Controllers
{
    /// <summary>
    /// Зорчигчдын удирдлагын контроллер.
    /// Энэ контроллер нь зорчигчдын мэдээллийг авах, үүсгэх болон шинэчлэх үйлдлүүдийг гүйцэтгэнэ.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PassengersController : ControllerBase
    {
        private readonly IPassengerService _passengerService;

        /// <summary>
        /// PassengersController-ийн байгуулагч.
        /// </summary>
        /// <param name="passengerService">Зорчигчийн үйлчилгээ</param>
        public PassengersController(IPassengerService passengerService)
        {
            _passengerService = passengerService;
        }

        /// <summary>
        /// Бүх зорчигчдын мэдээллийг авах.
        /// Нэр эсвэл паспортын дугаараар хайх боломжтой.
        /// </summary>
        /// <param name="name">Зорчигчийн нэрээр хайх (заавал биш)</param>
        /// <param name="passport">Паспортын дугаараар хайх (заавал биш)</param>
        /// <returns>Зорчигчдын жагсаалт</returns>
        /// <response code="200">Зорчигчдын жагсаалт амжилттай буцаагдсан</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Passenger>>> GetPassengers()
        {
            var passengers = await _passengerService.GetAllPassengersAsync();
            return Ok(passengers);
        }

        /// <summary>
        /// Зорчигчийн ID-аар зорчигчийн мэдээлэл авах.
        /// </summary>
        /// <param name="id">Зорчигчийн ID</param>
        /// <returns>Зорчигчийн мэдээлэл</returns>
        /// <response code="200">Зорчигчийн мэдээлэл амжилттай буцаагдсан</response>
        /// <response code="404">Зорчигч олдсонгүй</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Passenger>> GetPassenger(int id)
        {
            var passenger = await _passengerService.GetPassengerByIdAsync(id);
            if (passenger == null)
                return NotFound();
            return Ok(passenger);
        }

        /// <summary>
        /// Шинэ зорчигч үүсгэх.
        /// </summary>
        /// <param name="passenger">Зорчигчийн мэдээлэл</param>
        /// <returns>Үүсгэсэн зорчигчийн мэдээлэл</returns>
        /// <response code="201">Зорчигч амжилттай үүсгэгдсэн</response>
        /// <response code="400">Зорчигчийн мэдээлэл буруу</response>
        [HttpPost]
        public async Task<ActionResult<Passenger>> CreatePassenger([FromBody] Passenger passenger)
        {
            await _passengerService.AddPassengerAsync(passenger);
            return CreatedAtAction(nameof(GetPassenger), new { id = passenger.Id }, passenger);
        }

        /// <summary>
        /// Зорчигчийн мэдээллийг шинэчлэх.
        /// </summary>
        /// <param name="id">Зорчигчийн ID</param>
        /// <param name="passenger">Зорчигчийн шинэчлэгдсэн мэдээлэл</param>
        /// <returns>Үйлдлийн үр дүн</returns>
        /// <response code="204">Зорчигчийн мэдээлэл амжилттай шинэчлэгдсэн</response>
        /// <response code="400">Зорчигчийн мэдээлэл буруу</response>
        /// <response code="404">Зорчигч олдсонгүй</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePassenger(int id, [FromBody] Passenger passenger)
        {
            if (id != passenger.Id)
                return BadRequest();
            var existing = await _passengerService.GetPassengerByIdAsync(id);
            if (existing == null)
                return NotFound();
            await _passengerService.UpdatePassengerAsync(passenger);
            return NoContent();
        }

        /// <summary>
        /// Зорчигчийг устгах.
        /// </summary>
        /// <param name="id">Устгах зорчигчийн ID</param>
        /// <returns>Үйлдлийн үр дүн</returns>
        /// <response code="204">Зорчигч амжилттай устгагдсан</response>
        /// <response code="404">Зорчигч олдсонгүй</response>
        /// <response code="409">Зорчигч хамааралтай мэдээлэлтэй тул устгах боломжгүй</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePassenger(int id)
        {
            var existing = await _passengerService.GetPassengerByIdAsync(id);
            if (existing == null)
                return NotFound();
            await _passengerService.DeletePassengerAsync(id);
            return NoContent();
        }
    }
} 