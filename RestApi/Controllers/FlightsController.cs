using BusinessLogic.Services;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using RestApi.Models;
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

        /// <summary>
        /// FlightsController-ийн байгуулагч.
        /// </summary>
        /// <param name="flightService">Нислэгийн үйлчилгээ</param>
        public FlightsController(IFlightService flightService)
        {
            _flightService = flightService;
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
        /// Нислэгийн мэдээллийг шинэчлэх.
        /// </summary>
        /// <param name="id">Нислэгийн ID</param>
        /// <param name="flightDto">Нислэгийн шинэчлэгдсэн мэдээлэл</param>
        /// <returns>Үйлдлийн үр дүн</returns>
        /// <response code="204">Нислэгийн мэдээлэл амжилттай шинэчлэгдсэн</response>
        /// <response code="404">Нислэг олдсонгүй</response>
        /// <response code="400">Нислэгийн мэдээлэл буруу</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFlight(int id, [FromBody] UpdateFlightDto flightDto)
        {
            try
            {
                if (id != flightDto.Id)
                {
                    return BadRequest("Flight ID in URL does not match ID in request body.");
                }

                if (!await _flightService.FlightExistsAsync(id))
                {
                    return NotFound($"Flight with ID {id} not found.");
                }
                
                // DTO-г домэйн модел болгон хөрвүүлэх
                var existingFlight = await _flightService.GetFlightByIdAsync(id);
                if (existingFlight == null)
                {
                    return NotFound($"Flight with ID {id} not found.");
                }
                
                // Зөвхөн шаардлагатай талбаруудын утгыг өөрчлөх
                existingFlight.FlightNumber = flightDto.FlightNumber;
                existingFlight.DepartureCity = flightDto.DepartureCity;
                existingFlight.ArrivalCity = flightDto.ArrivalCity;
                existingFlight.DepartureTime = flightDto.DepartureTime;
                existingFlight.ArrivalTime = flightDto.ArrivalTime;
                existingFlight.Status = flightDto.Status;
                
                // Шинэчлэгдсэн нислэгийг хадгалах
                await _flightService.UpdateFlightAsync(existingFlight);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
            try
            {
                if (!await _flightService.FlightExistsAsync(id))
                {
                    return NotFound($"Flight with ID {id} not found.");
                }

                await _flightService.DeleteFlightAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
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