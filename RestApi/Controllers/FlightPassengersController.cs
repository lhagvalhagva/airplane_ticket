using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BusinessLogic.Services;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controllers
{
    /// <summary>
    /// Нислэг болон зорчигчдын холбоосыг удирдах контроллер
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FlightPassengersController : ControllerBase
    {
        private readonly IFlightPassengerService _flightPassengerService;

        public FlightPassengersController(IFlightPassengerService flightPassengerService)
        {
            _flightPassengerService = flightPassengerService;
        }

        /// <summary>
        /// Нислэгт зорчигч бүртгэх
        /// </summary>
        /// <param name="request">Нислэг болон зорчигчийн ID</param>
        /// <returns>Бүртгэгдсэн холбоос</returns>
        /// <response code="201">Зорчигч амжилттай бүртгэгдсэн</response>
        /// <response code="400">Буруу өгөгдөл оруулсан</response>
        /// <response code="404">Нислэг эсвэл зорчигч олдсонгүй</response>
        /// <response code="409">Зорчигч аль хэдийн бүртгэлтэй байна</response>
        [HttpPost]
        public async Task<ActionResult<FlightPassenger>> RegisterPassenger([FromBody] FlightPassengerRegisterRequest request)
        {
            try
            {
                var flightPassenger = await _flightPassengerService.RegisterPassengerToFlightAsync(
                    request.FlightId, 
                    request.PassengerId);
                
                return CreatedAtAction(
                    nameof(GetFlightPassengers), 
                    new { flightId = request.FlightId },
                    flightPassenger);
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

        /// <summary>
        /// Тухайн нислэгийн зорчигчдын жагсаалт авах
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <returns>Зорчигчдын жагсаалт</returns>
        /// <response code="200">Зорчигчдын жагсаалт амжилттай буцаагдсан</response>
        /// <response code="404">Нислэг олдсонгүй</response>
        [HttpGet("~/api/flights/{flightId}/passengers")]
        public async Task<ActionResult<IEnumerable<Passenger>>> GetFlightPassengers(int flightId)
        {
            try
            {
                var passengers = await _flightPassengerService.GetPassengersByFlightIdAsync(flightId);
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
        /// Зорчигчийг нислэгээс хасах
        /// </summary>
        /// <param name="id">FlightPassenger холболтын ID</param>
        /// <returns>Үр дүн</returns>
        /// <response code="204">Зорчигч амжилттай хасагдсан</response>
        /// <response code="404">Холболт олдсонгүй</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult> RemovePassengerFromFlight(int id)
        {
            try
            {
                if (!await _flightPassengerService.FlightPassengerExistsAsync(id))
                {
                    return NotFound($"FlightPassenger ID {id} олдсонгүй.");
                }

                await _flightPassengerService.RemovePassengerFromFlightAsync(id);
                return NoContent();
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
    }

    /// <summary>
    /// Нислэгт зорчигч бүртгэх хүсэлтийн модель
    /// </summary>
    public class FlightPassengerRegisterRequest
    {
        /// <summary>
        /// Нислэгийн ID
        /// </summary>
        [Required]
        public int FlightId { get; set; }

        /// <summary>
        /// Зорчигчийн ID
        /// </summary>
        [Required]
        public int PassengerId { get; set; }
    }
}
