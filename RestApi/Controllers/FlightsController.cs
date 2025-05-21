using BusinessLogic.Services;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        private readonly ISeatService _seatService;

        /// <summary>
        /// FlightsController-ийн байгуулагч.
        /// </summary>
        /// <param name="flightService">Нислэгийн үйлчилгээ</param>
        /// <param name="seatService">Залгаайн үйлчилгээ</param>
        public FlightsController(IFlightService flightService, ISeatService seatService)
        {
            _flightService = flightService;
            _seatService = seatService;
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
                return NotFound();
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
        public async Task<ActionResult<Flight>> CreateFlight([FromBody] Flight flight)
        {
            try
            {
                // Add the flight first
                await _flightService.AddFlightAsync(flight);

                // Initialize seats for the flight
                var seats = new List<Seat>();
                for (int i = 1; i <= flight.AvailableSeats; i++)
                {
                    seats.Add(new Seat
                    {
                        FlightId = flight.Id,
                        SeatNumber = $"{i}A",
                        IsOccupied = false
                    });
                }

                // Add all seats
                foreach (var seat in seats)
                {
                    await _seatService.AddSeatAsync(seat);
                }

                return CreatedAtAction(nameof(GetFlight), new { id = flight.Id }, flight);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Нислэгийн мэдээллийг шинэчлэх.
        /// </summary>
        /// <param name="id">Нислэгийн ID</param>
        /// <param name="flight">Нислэгийн шинэчлэгдсэн мэдээлэл</param>
        /// <returns>Үйлдлийн үр дүн</returns>
        /// <response code="204">Нислэгийн мэдээлэл амжилттай шинэчлэгдсэн</response>
        /// <response code="404">Нислэг олдсонгүй</response>
        /// <response code="400">Нислэгийн мэдээлэл буруу</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFlight(int id, [FromBody] Flight flight)
        {
            if (id != flight.Id)
                return BadRequest();
            var existing = await _flightService.GetFlightByIdAsync(id);
            if (existing == null)
                return NotFound();
            await _flightService.UpdateFlightAsync(flight);
            return NoContent();
        }

        /// <summary>
        /// Нислэг устгах.
        /// </summary>
        /// <param name="id">Нислэгийн ID</param>
        /// <returns>Үйлдлийн үр дүн</returns>
        /// <response code="204">Нислэг амжилттай устгагдсан</response>
        /// <response code="404">Нислэг олдсонгүй</response>
        /// <response code="409">Нислэгт бүртгэлтэй хэрэглэгч эсвэл суудал байгаа тул устгах боломжгүй</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlight(int id)
        {
            var existing = await _flightService.GetFlightByIdAsync(id);
            if (existing == null)
                return NotFound();
            await _flightService.DeleteFlightAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Нислэгийн төлөвийг шинэчлэх.
        /// </summary>
        /// <param name="id">Нислэгийн ID</param>
        /// <param name="status">Нислэгийн шинэ төлөв</param>
        /// <returns>Шинэчлэгдсэн төлөв</returns>
        /// <response code="200">Нислэгийн төлөв амжилттай шинэчлэгдсэн</response>
        /// <response code="404">Нислэг олдсонгүй</response>
        [HttpPut("{id}/status")]
        public async Task<ActionResult<FlightStatus>> UpdateFlightStatus(int id, [FromBody] FlightStatus status)
        {
            try
            {
                var flight = await _flightService.GetFlightByIdAsync(id);
                if (flight == null)
                    return NotFound($"Нислэг ID {id} олдсонгүй");

                flight.Status = status;
                await _flightService.UpdateFlightAsync(flight);

                return Ok(status);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}