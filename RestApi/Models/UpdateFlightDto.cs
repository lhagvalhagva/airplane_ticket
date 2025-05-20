using DataAccess.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace RestApi.Models
{
    /// <summary>
    /// Нислэгийн мэдээлэл шинэчлэх DTO
    /// </summary>
    public class UpdateFlightDto
    {
        /// <summary>
        /// Нислэгийн дугаар
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Нислэгийн нэр
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string FlightNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// Хөөрөх хот
        /// </summary>
        [Required]
        public string DepartureCity { get; set; } = string.Empty;
        
        /// <summary>
        /// Буух хот
        /// </summary>
        [Required]
        public string ArrivalCity { get; set; } = string.Empty;
        
        /// <summary>
        /// Хөөрөх цаг
        /// </summary>
        [Required]
        public DateTime DepartureTime { get; set; }
        
        /// <summary>
        /// Буух цаг
        /// </summary>
        [Required]
        public DateTime ArrivalTime { get; set; }
        
        /// <summary>
        /// Нислэгийн төлөв
        /// </summary>
        [Required]
        public FlightStatus Status { get; set; } = FlightStatus.CheckingIn;
    }
}