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
    /// Онгоцонд суух, бүртгэлийн үйл явцыг удирдах контроллер.
    /// Энэ контроллер нь суудал захиалах, зорчигчдыг бүртгэх, боломжтой суудлууд болон онгоцны бүртгэлийн талаарх мэдээлэл авах үйлдлүүдийг гүйцэтгэнэ.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BoardingController : ControllerBase
    {
        private readonly IBoardingService _boardingService;
        private readonly INotificationService _notificationService;

        /// <summary>
        /// BoardingController-ийн байгуулагч.
        /// </summary>
        /// <param name="boardingService">Онгоцонд суух үйлчилгээ</param>
        /// <param name="notificationService">Мэдэгдлийн үйлчилгээ</param>
        public BoardingController(
            IBoardingService boardingService,
            INotificationService notificationService)
        {
            _boardingService = boardingService;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Тодорхой нислэгийн боломжтой суудлуудын жагсаалтыг авах.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <returns>Боломжтой суудлуудын жагсаалт</returns>
        /// <response code="200">Боломжтой суудлуудын жагсаалт амжилттай буцаагдсан</response>
        /// <response code="400">Нислэгийн дугаар буруу</response>
        [HttpGet("flights/{flightId}/seats")]
        public async Task<ActionResult<IEnumerable<Seat>>> GetAvailableSeats(int flightId)
        {
            var seats = await _boardingService.GetAvailableSeatsAsync(flightId);
            return Ok(seats);
        }

        /// <summary>
        /// Тодорхой нислэгийн бүх суудлуудын жагсаалтыг авах.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <returns>Нислэгийн бүх суудлуудын жагсаалт</returns>
        /// <response code="200">Суудлуудын жагсаалт амжилттай буцаагдсан</response>
        /// <response code="404">Нислэг олдоогүй</response>
        [HttpGet("seats/{flightId}")]
        public async Task<ActionResult<IEnumerable<Seat>>> GetAllSeatsForFlight(int flightId)
        {
            try
            {
                // Нислэг байгаа эсэхийг шалгах эсвэл бусад логик шалгалт
                // Энд хялбар байлгахын тулд бүх суудлыг буцаая
                var result = await _boardingService.GetAvailableSeatsAsync(flightId);
                return Ok(result);
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
        /// Тодорхой нислэгийн бүх онгоцны бүртгэлийн жагсаалтыг авах.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <returns>Нислэгийн бүх онгоцны бүртгэлийн жагсаалт</returns>
        /// <response code="200">Онгоцны бүртгэлийн жагсаалт амжилттай буцаагдсан</response>
        /// <response code="400">Нислэгийн дугаар буруу</response>
        [HttpGet("flights/{flightId}/passes")]
        public async Task<ActionResult<IEnumerable<BoardingPass>>> GetFlightBoardingPasses(int flightId)
        {
            var boardingPasses = await _boardingService.GetBoardingPassesByFlightAsync(flightId);
            return Ok(boardingPasses);
        }

        /// <summary>
        /// Онгоцны бүртгэлийн мэдээллийг ID-аар авах.
        /// </summary>
        /// <param name="id">Онгоцны бүртгэлийн ID</param>
        /// <returns>Онгоцны бүртгэлийн мэдээлэл</returns>
        /// <response code="200">Онгоцны бүртгэлийн мэдээлэл амжилттай буцаагдсан</response>
        /// <response code="404">Онгоцны бүртгэл олдсонгүй</response>
        [HttpGet("passes/{boardingPassId}")]
        public async Task<ActionResult<BoardingPass>> GetBoardingPass(int boardingPassId)
        {
            var boardingPass = await _boardingService.GetBoardingPassAsync(boardingPassId);
            if (boardingPass == null)
                return NotFound();
            return Ok(boardingPass);
        }

        /// <summary>
        /// Зорчигчийг нислэгт бүртгэх.
        /// </summary>
        /// <param name="request">Бүртгэлийн хүсэлт</param>
        /// <returns>Үүсгэсэн онгоцны бүртгэлийн мэдээлэл</returns>
        /// <response code="201">Зорчигч амжилттай бүртгэгдсэн</response>
        /// <response code="404">Нислэг, зорчигч, эсвэл суудал олдсонгүй</response>
        /// <response code="409">Суудал аль хэдийн захиалагдсан, эсвэл ижил зорчигч өөр нислэгт бүртгэгдсэн</response>
        /// <response code="400">Бусад алдаанууд</response>
        [HttpPost("check-in")]
        public async Task<ActionResult<BoardingPass>> CheckInPassenger([FromBody] CheckInRequest request)
        {
            try
            {
                var boardingPass = await _boardingService.CheckInPassengerAsync(
                    request.FlightId,
                    request.PassportNumber,
                    request.SeatNumber);

                
                await _notificationService.NotifySeatAssignedAsync(
                    request.FlightId,
                    request.SeatNumber,
                    boardingPass.PassengerId);

                
                await _notificationService.NotifyBoardingPassIssuedAsync(boardingPass);

                return CreatedAtAction(
                    nameof(GetBoardingPass),
                    new { id = boardingPass.Id },
                    boardingPass);
            }
            catch (KeyNotFoundException ex)
            {
                // Зорчигч, нислэг эсвэл суудал олдохгүй бол тодорхой алдааны мессеж буцаах
                return NotFound(new { Error = ex.Message, Details = $"SeatNumber: {request.SeatNumber}, FlightId: {request.FlightId}" });
            }
            catch (InvalidOperationException ex)
            {
                // Суудал аль хэдийн бүртгүүлсэн бол тодорхой алдааны мессеж буцаах
                return Conflict(new { Error = ex.Message, Details = $"SeatNumber: {request.SeatNumber}, FlightId: {request.FlightId}" });
            }
            catch (Exception ex)
            {
                // Энд алдааны мессежийг хэвлэх ба алдааны мессежийг буцаах
                Console.WriteLine($"Error in CheckInPassenger: {ex.Message}");
                return BadRequest(new { Error = ex.Message, Details = $"SeatNumber: {request.SeatNumber}, FlightId: {request.FlightId}" });
            }
        }

        /// <summary>
        /// Тодорхой нислэгийн суудлын боломжит байдлыг шалгах.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="seatNumber">Суудлын дугаар</param>
        /// <returns>Суудал боломжтой эсэх</returns>
        /// <response code="200">Суудлын боломжит байдал амжилттай шалгагдсан</response>
        /// <response code="400">Нислэгийн дугаар эсвэл суудлын дугаар буруу</response>
        [HttpGet("flights/{flightId}/seats/{seatNumber}/availability")]
        public async Task<ActionResult<bool>> CheckSeatAvailability(int flightId, string seatNumber)
        {
            try
            {
                var isAvailable = await _boardingService.IsSeatAvailableAsync(flightId, seatNumber);
                return Ok(isAvailable);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        /// <summary>
        /// Хэрэглэгчид суудал оноох.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="passengerId">Зорчигчийн ID</param>
        /// <param name="seatNumber">Суудлын дугаар</param>
        /// <returns>Суудал амжилттай оноогдсон эсэх мэдээлэл</returns>
        /// <response code="200">Суудал амжилттай оноогдсон</response>
        /// <response code="404">Нислэг, зорчигч эсвэл суудал олдсонгүй</response>
        /// <response code="409">Суудал аль хэдийн эзэмшигдсэн</response>
        [HttpPost("flights/{flightId}/passengers/{passengerId}/seat")]
        public async Task<ActionResult<bool>> AssignSeatToPassenger(int flightId, int passengerId, [FromQuery] string seatNumber)
        {
            try
            {
                var result = await _boardingService.AssignSeatToPassengerAsync(flightId, passengerId, seatNumber);
                return Ok(result);
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
    /// Зорчигчийг нислэгт бүртгэх хүсэлтийн загвар.
    /// </summary>
    public class CheckInRequest
    {
        /// <summary>
        /// Нислэгийн ID.
        /// </summary>
        [Required]
        public int FlightId { get; set; }

        /// <summary>
        /// Зорчигчийн паспортын дугаар.
        /// </summary>
        [Required]
        public string PassportNumber { get; set; } = string.Empty;

        /// <summary>
        /// Сонгосон суудлын дугаар.
        /// </summary>
        [Required]
        public string SeatNumber { get; set; } = string.Empty;
    }
} 