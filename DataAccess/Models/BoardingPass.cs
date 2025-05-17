using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class BoardingPass
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public DateTime CheckInTime { get; set; }
        
        [Required]
        public int PassengerId { get; set; }
        
        [Required]
        public int SeatId { get; set; }
        
        [Required]
        public int FlightId { get; set; }
        
        [ForeignKey("PassengerId")]
        public virtual Passenger Passenger { get; set; } = null!;
        
        [ForeignKey("SeatId")]
        public virtual Seat Seat { get; set; } = null!;
        
        [ForeignKey("FlightId")]
        public virtual Flight Flight { get; set; } = null!;
    }
} 