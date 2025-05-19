using BusinessLogic.Services;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestApi.Controllers
{
    /// <summary>
    /// Суудлын удирдлагын контроллер.
    /// /flights/{flightId}/seats/ эндпойнтын бүлэг
    /// </summary>
    [ApiController]
    [Route("api/flights/{flightId}/seats")]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;

        /// <summary>
        /// SeatsController байгуулагч
        /// </summary>
        public SeatsController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        /// <summary>
        /// Тодорхой нислэгийн бүх суудлуудын жагсаалтыг авах (шүүж болно)
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="isOccupied">Суудал захиалагдсан эсэх (null = бүгдийг)</param>
        /// <returns>Суудлуудын жагсаалт</returns>
        /// <response code="200">Суудлуудын жагсаалт амжилттай буцаагдсан</response>
        /// <response code="404">Нислэг олдоогүй</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Seat>>> GetSeats(int flightId, [FromQuery] bool? isOccupied = null)
        {
            try
            {
                var seats = await _seatService.GetSeatsByFlightIdAsync(flightId, isOccupied);
                return Ok(seats);
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
        /// Тодорхой нислэгийн боломжтой (сул) суудлуудын жагсаалтыг авах
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <returns>Боломжтой суудлуудын жагсаалт</returns>
        /// <response code="200">Боломжтой суудлуудын жагсаалт амжилттай буцаагдсан</response>
        /// <response code="404">Нислэг олдоогүй</response>
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<Seat>>> GetAvailableSeats(int flightId)
        {
            try
            {
                var seats = await _seatService.GetAvailableSeatsByFlightIdAsync(flightId);
                return Ok(seats);
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
        /// Суудал оноох (зорчигч + суудал ID)
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="assignRequest">Оноох мэдээлэл</param>
        /// <returns>Амжилттай бол 200 OK</returns>
        /// <response code="200">Суудал амжилттай оноогдсон</response>
        /// <response code="400">Оноох боломжгүй</response>
        /// <response code="404">Нислэг, зорчигч эсвэл суудал олдоогүй</response>
        /// <response code="409">Суудал аль хэдийн эзэмшигдсэн</response>
        [HttpPost("assign")]
        public async Task<ActionResult> AssignSeat(int flightId, [FromBody] SeatAssignRequest assignRequest)
        {
            try
            {
                var result = await _seatService.AssignSeatAsync(flightId, assignRequest.PassengerId, assignRequest.SeatId);
                return Ok(new { success = result });
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
        /// Суудлыг чөлөөлөх
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="seatId">Суудлын ID</param>
        /// <returns>Амжилттай бол 200 OK</returns>
        /// <response code="200">Суудал амжилттай чөлөөлөгдсөн</response>
        /// <response code="404">Нислэг эсвэл суудал олдоогүй</response>
        [HttpPut("{seatId}/release")]
        public async Task<ActionResult> ReleaseSeat(int flightId, int seatId)
        {
            try
            {
                var result = await _seatService.ReleaseSeatAsync(flightId, seatId);
                return Ok(new { success = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    /// <summary>
    /// Суудал оноох хүсэлтийн моделийн класс
    /// </summary>
    public class SeatAssignRequest
    {
        /// <summary>
        /// Зорчигчийн ID
        /// </summary>
        public int PassengerId { get; set; }

        /// <summary>
        /// Суудлын ID
        /// </summary>
        public int SeatId { get; set; }
    }
}
