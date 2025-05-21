using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DataAccess.Models
{
    public class Seat
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(10)]
        public string SeatNumber { get; set; } = string.Empty;
        
        [Required]
        public bool IsOccupied { get; set; } = false;
        
        public int? PassengerId { get; set; }
        
        public DateTime? CheckInTime { get; set; }
        
        [Required]
        public int FlightId { get; set; }
        
        [ForeignKey("FlightId")]
        [JsonIgnore]
        public virtual Flight Flight { get; set; } = null!;
        
        [ForeignKey("PassengerId")]
        [JsonIgnore]
        public virtual Passenger? Passenger { get; set; }
    }
}