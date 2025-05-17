using BusinessLogic.Services;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace RestApi.Controllers
{
    /// <summary>
    /// Нислэгүүдийн удирдлагын контроллер.
    /// Энэ контроллер нь нислэгүүдийн мэдээллийг авах, үүсгэх болон төлөвийг шинэчлэх үйлдлүүдийг гүйцэтгэнэ.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightService _flightService;
        private readonly INotificationService _notificationService;

        /// <summary>
        /// FlightsController-ийн байгуулагч.
        /// </summary>
        /// <param name="flightService">Нислэгийн үйлчилгээ</param>
        /// <param name="notificationService">Мэдэгдлийн үйлчилгээ</param>
        public FlightsController(IFlightService flightService, INotificationService notificationService)
        {
            _flightService = flightService;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Бүх нислэгүүдийн мэдээллийг авах.
        /// </summary>
        /// <returns>Нислэгүүдийн жагсаалт</returns>
        /// <response code="200">Нислэгүүдийн жагсаалт амжилттай буцаагдсан</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlights()
        {
            var flights = await _flightService.GetAllFlightsAsync();
            return Ok(flights);
        }

        /// <summary>
        /// Нислэгийн дугаараар нислэгийн мэдээлэл авах.
        /// </summary>
        /// <param name="id">Нислэгийн ID</param>
        /// <returns>Нислэгийн мэдээлэл</returns>
        /// <response code="200">Нислэгийн мэдээлэл амжилттай буцаагдсан</response>
        /// <response code="404">Нислэг олдсонгүй</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Flight>> GetFlight(int id)
        {
            var flight = await _flightService.GetFlightByIdAsync(id);

            if (flight == null)
            {
                return NotFound($"Flight with ID {id} not found.");
            }

            return Ok(flight);
        }

        /// <summary>
        /// Нислэгийн дугаараар нислэгийн мэдээлэл авах.
        /// </summary>
        /// <param name="flightNumber">Нислэгийн дугаар</param>
        /// <returns>Нислэгийн мэдээлэл</returns>
        /// <response code="200">Нислэгийн мэдээлэл амжилттай буцаагдсан</response>
        /// <response code="404">Нислэг олдсонгүй</response>
        [HttpGet("number/{flightNumber}")]
        public async Task<ActionResult<Flight>> GetFlightByNumber(string flightNumber)
        {
            var flight = await _flightService.GetFlightByNumberAsync(flightNumber);

            if (flight == null)
            {
                return NotFound($"Flight with number {flightNumber} not found.");
            }

            return Ok(flight);
        }

        /// <summary>
        /// Шинэ нислэг үүсгэх.
        /// </summary>
        /// <param name="flight">Нислэгийн мэдээлэл</param>
        /// <returns>Үүсгэсэн нислэгийн мэдээлэл</returns>
        /// <response code="201">Нислэг амжилттай үүсгэгдсэн</response>
        /// <response code="400">Нислэгийн мэдээлэл буруу</response>
        [HttpPost]
        public async Task<ActionResult<Flight>> CreateFlight(Flight flight)
        {
            try
            {
                await _flightService.AddFlightAsync(flight);
                return CreatedAtAction(nameof(GetFlight), new { id = flight.Id }, flight);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Нислэгийн төлөвийг шинэчлэх.
        /// </summary>
        /// <param name="id">Нислэгийн ID</param>
        /// <param name="statusDto">Нислэгийн шинэ төлөв</param>
        /// <returns>Үйлдлийн үр дүн</returns>
        /// <response code="204">Нислэгийн төлөв амжилттай шинэчлэгдсэн</response>
        /// <response code="404">Нислэг олдсонгүй</response>
        /// <response code="400">Нислэгийн төлөв буруу</response>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateFlightStatus(int id, [FromBody] UpdateFlightStatusDto statusDto)
        {
            try
            {
                if (!await _flightService.FlightExistsAsync(id))
                {
                    return NotFound($"Flight with ID {id} not found.");
                }

                await _flightService.UpdateFlightStatusAsync(id, statusDto.Status);
                await _notificationService.NotifyFlightStatusChangedAsync(id, statusDto.Status);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    /// <summary>
    /// Нислэгийн төлөв шинэчлэх хүсэлтийн загвар.
    /// </summary>
    public class UpdateFlightStatusDto
    {
        /// <summary>
        /// Нислэгийн шинэ төлөв.
        /// </summary>
        [Required]
        public FlightStatus Status { get; set; }
    }
} 