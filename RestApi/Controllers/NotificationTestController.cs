using BusinessLogic.Services;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace RestApi.Controllers
{
    /// <summary>
    /// Мэдэгдэл илгээх системийг туршихад зориулсан туршилтын контроллер.
    /// Энэ контроллер нь бодит үйлдлүүдээс тусдаа мэдэгдэл илгээх функцүүдийг дуудах боломжийг олгоно.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationTestController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        /// <summary>
        /// NotificationTestController-ийн байгуулагч.
        /// </summary>
        /// <param name="notificationService">Мэдэгдлийн үйлчилгээ</param>
        public NotificationTestController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Нислэгийн төлөв өөрчлөгдсөн тухай мэдэгдэл илгээх.
        /// </summary>
        /// <param name="request">Мэдэгдлийн хүсэлт</param>
        /// <returns>Үйлдлийн үр дүн</returns>
        /// <response code="200">Мэдэгдэл амжилттай илгээгдсэн</response>
        /// <response code="400">Хүсэлтийн мэдээлэл буруу</response>
        [HttpPost("flight-status")]
        public async Task<IActionResult> SendFlightStatusNotification(FlightStatusNotificationRequest request)
        {
            try
            {
                await _notificationService.NotifyFlightStatusChangedAsync(request.FlightId, request.Status);
                return Ok($"Flight status notification for flight ID {request.FlightId} sent successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Суудал захиалагдсан тухай мэдэгдэл илгээх.
        /// </summary>
        /// <param name="request">Мэдэгдлийн хүсэлт</param>
        /// <returns>Үйлдлийн үр дүн</returns>
        /// <response code="200">Мэдэгдэл амжилттай илгээгдсэн</response>
        /// <response code="400">Хүсэлтийн мэдээлэл буруу</response>
        [HttpPost("seat-assigned")]
        public async Task<IActionResult> SendSeatAssignedNotification(SeatAssignedNotificationRequest request)
        {
            try
            {
                await _notificationService.NotifySeatAssignedAsync(
                    request.FlightId, 
                    request.SeatNumber, 
                    request.PassengerId);
                
                return Ok($"Seat {request.SeatNumber} assigned notification for flight ID {request.FlightId} sent successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    /// <summary>
    /// Нислэгийн төлөв өөрчлөгдсөн тухай мэдэгдэл илгээх хүсэлтийн загвар.
    /// </summary>
    public class FlightStatusNotificationRequest
    {
        /// <summary>
        /// Нислэгийн ID.
        /// </summary>
        [Required]
        public int FlightId { get; set; }

        /// <summary>
        /// Нислэгийн шинэ төлөв.
        /// </summary>
        [Required]
        public FlightStatus Status { get; set; }
    }

    /// <summary>
    /// Суудал захиалагдсан тухай мэдэгдэл илгээх хүсэлтийн загвар.
    /// </summary>
    public class SeatAssignedNotificationRequest
    {
        /// <summary>
        /// Нислэгийн ID.
        /// </summary>
        [Required]
        public int FlightId { get; set; }

        /// <summary>
        /// Захиалагдсан суудлын дугаар.
        /// </summary>
        [Required]
        public string SeatNumber { get; set; } = string.Empty;

        /// <summary>
        /// Зорчигчийн ID.
        /// </summary>
        [Required]
        public int PassengerId { get; set; }
    }
} 