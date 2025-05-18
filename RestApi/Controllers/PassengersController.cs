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
        /// </summary>
        /// <returns>Зорчигчдын жагсаалт</returns>
        /// <response code="200">Зорчигчдын жагсаалт амжилттай буцаагдсан</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Passenger>>> GetPassengers()
        {
            var passengers = await _passengerService.GetAllPassengersAsync();
            return Ok(passengers);
        }

        /// <summary>
        /// Нислэгийн зорчигчдыг авах
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <returns>Зорчигчдын жагсаалт</returns>
        /// <response code="200">Зорчигчдын жагсаалт амжилттай буцаагдсан</response>
        /// <response code="404">Нислэг олдсонгүй</response>
        [HttpGet("flight/{flightId}")]
        public async Task<ActionResult<IEnumerable<Passenger>>> GetFlightPassengers(int flightId)
        {
            try
            {
                var passengers = await _passengerService.GetPassengersByFlightIdAsync(flightId);
                return Ok(passengers);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
            {
                return NotFound($"Passenger with ID {id} not found.");
            }

            return Ok(passenger);
        }

        /// <summary>
        /// Паспортын дугаараар зорчигчийн мэдээлэл авах.
        /// </summary>
        /// <param name="passportNumber">Паспортын дугаар</param>
        /// <returns>Зорчигчийн мэдээлэл</returns>
        /// <response code="200">Зорчигчийн мэдээлэл амжилттай буцаагдсан</response>
        /// <response code="404">Зорчигч олдсонгүй</response>
        [HttpGet("passport/{passportNumber}")]
        public async Task<ActionResult<Passenger>> GetPassengerByPassport(string passportNumber)
        {
            var passenger = await _passengerService.GetPassengerByPassportNumberAsync(passportNumber);

            if (passenger == null)
            {
                return NotFound($"Passenger with passport number {passportNumber} not found.");
            }

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
        public async Task<ActionResult<Passenger>> CreatePassenger(Passenger passenger)
        {
            try
            {
                await _passengerService.AddPassengerAsync(passenger);
                return CreatedAtAction(nameof(GetPassenger), new { id = passenger.Id }, passenger);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
        public async Task<IActionResult> UpdatePassenger(int id, Passenger passenger)
        {
            if (id != passenger.Id)
            {
                return BadRequest("Passenger ID in URL does not match ID in request body.");
            }

            try
            {
                if (!await _passengerService.PassengerExistsAsync(id))
                {
                    return NotFound($"Passenger with ID {id} not found.");
                }

                await _passengerService.UpdatePassengerAsync(passenger);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
} 