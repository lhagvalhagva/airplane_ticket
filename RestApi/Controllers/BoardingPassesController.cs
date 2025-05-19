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
    /// Тасалбар ба Check-In үйл явцыг удирдах контроллер.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BoardingPassesController : ControllerBase
    {
        private readonly IBoardingService _boardingService;

        public BoardingPassesController(IBoardingService boardingService)
        {
            _boardingService = boardingService;
        }

        /// <summary>
        /// Тодорхой ID-тай тасалбарын мэдээлэл авах.
        /// </summary>
        /// <param name="id">Тасалбарын ID</param>
        /// <returns>Тасалбарын дэлгэрэнгүй мэдээлэл</returns>
        /// <response code="200">Тасалбарын мэдээлэл амжилттай буцаагдсан</response>
        /// <response code="404">Тасалбар олдсонгүй</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<BoardingPass>> GetBoardingPass(int id)
        {
            var boardingPass = await _boardingService.GetBoardingPassByIdAsync(id);
            if (boardingPass == null)
                return NotFound($"Тасалбар ID {id} олдсонгүй.");

            return Ok(boardingPass);
        }

        /// <summary>
        /// Зорчигч болон нислэгийн ID-аар тасалбар хайх.
        /// </summary>
        /// <param name="passengerId">Зорчигчийн ID (optional)</param>
        /// <param name="flightId">Нислэгийн ID (optional)</param>
        /// <returns>Олдсон тасалбарууд</returns>
        /// <response code="200">Тасалбарууд амжилттай буцаагдсан</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoardingPass>>> GetBoardingPasses(
            [FromQuery] int? passengerId, 
            [FromQuery] int? flightId)
        {
            // Хэрэв аль нэг нь өгөгдөөгүй бол алдаа буцаана
            if (!passengerId.HasValue && !flightId.HasValue)
                return BadRequest("Хайлт хийхэд passengerId эсвэл flightId заавал өгөх шаардлагатай.");

            // Хэрэв хоёулаа өгөгдсөн бол тодорхой тасалбар хайна
            if (passengerId.HasValue && flightId.HasValue)
            {
                var boardingPass = await _boardingService.GetBoardingPassByFlightAndPassengerAsync(
                    flightId.Value, passengerId.Value);
                
                if (boardingPass == null)
                    return new List<BoardingPass>(); // Хоосон жагсаалт буцаах

                return Ok(new List<BoardingPass> { boardingPass });
            }

            // Зөвхөн нислэгийн ID өгөгдсөн бол
            if (flightId.HasValue)
            {
                var boardingPasses = await _boardingService.GetBoardingPassesByFlightAsync(flightId.Value);
                return Ok(boardingPasses);
            }

            // Бусад тохиолдолд (зөвхөн зорчигчийн ID өгөгдсөн гэх мэт) - NotImplemented
            return BadRequest("Энэ төрлийн хайлт хараахан хэрэгжээгүй байна.");
        }

        /// <summary>
        /// Тасалбар үүсгэх (Check-in хийх).
        /// Нислэг, зорчигч болон суудлын мэдээлэл авах нь хангалттай.
        /// Суудлын ID эсвэл суудлын дугаарын аль нэгийг оруулж болно.
        /// </summary>
        /// <param name="request">Тасалбар үүсгэх хүсэлт</param>
        /// <returns>Шинээр үүсгэсэн тасалбарын мэдээлэл</returns>
        /// <response code="201">Тасалбар амжилттай үүсгэгдсэн</response>
        /// <response code="400">Буруу өгөгдөл оруулсан</response>
        /// <response code="404">Нислэг, зорчигч эсвэл суудал олдсонгүй</response>
        /// <response code="409">Зорчигч аль хэдийн тасалбар авсан, эсвэл суудал захиалагдсан</response>
        [HttpPost]
        public async Task<ActionResult<BoardingPass>> CreateBoardingPass([FromBody] BoardingPassCreateRequest request)
        {
            try
            {
                BoardingPass createdBoardingPass;
                
                // Суудлын дугаар өгөгдсөн бол түүгээр үүсгэх
                if (!string.IsNullOrEmpty(request.SeatNumber))
                {
                    createdBoardingPass = await _boardingService.CreateBoardingPassWithSeatInfoAsync(
                        request.FlightId,
                        request.PassengerId,
                        request.SeatNumber
                    );
                }
                // Суудлын ID өгөгдсөн бол уламжлалт аргаар үүсгэх
                else if (request.SeatId.HasValue) 
                {
                    // Үүсгэх объект бэлтгэх
                    var boardingPass = new BoardingPass
                    {
                        FlightId = request.FlightId,
                        PassengerId = request.PassengerId,
                        SeatId = request.SeatId.Value,
                        CheckInTime = DateTime.UtcNow
                    };

                    // Тасалбар үүсгэх
                    createdBoardingPass = await _boardingService.CreateBoardingPassAsync(boardingPass);
                }
                else
                {
                    return BadRequest("Тасалбар үүсгэхэд SeatId эсвэл SeatNumber заавал оруулах хэрэгтэй.");
                }

                // Амжилттай үүсгэгдсэн тасалбарыг буцаах
                return CreatedAtAction(
                    nameof(GetBoardingPass), 
                    new { id = createdBoardingPass.Id }, 
                    createdBoardingPass);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
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
        /// Тасалбар устгах (унасан, цуцлагдсан гэх мэт).
        /// </summary>
        /// <param name="id">Тасалбарын ID</param>
        /// <returns>Устгасан эсэхийн мэдээлэл</returns>
        /// <response code="204">Тасалбар амжилттай устгагдсан</response>
        /// <response code="404">Тасалбар олдсонгүй</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBoardingPass(int id)
        {
            try
            {
                await _boardingService.DeleteBoardingPassAsync(id);
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
    /// Тасалбар үүсгэх хүсэлтийн модель.
    /// </summary>
    public class BoardingPassCreateRequest
    {
        /// <summary>
        /// Зорчигчийн ID
        /// </summary>
        [Required]
        public int PassengerId { get; set; }

        /// <summary>
        /// Нислэгийн ID
        /// </summary>
        [Required]
        public int FlightId { get; set; }

        /// <summary>
        /// Суудлын ID - SeatNumber оруулахгүй бол заавал шаардлагатай
        /// </summary>
        public int? SeatId { get; set; }

        /// <summary>
        /// Суудлын дугаар - SeatId оруулахгүй бол заавал шаардлагатай
        /// </summary>
        public string? SeatNumber { get; set; }
    }
}
