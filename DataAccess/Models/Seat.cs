using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        
        [Required]
        public int FlightId { get; set; }
        
        [ForeignKey("FlightId")]
        public virtual Flight Flight { get; set; } = null!;
        
        public virtual BoardingPass? BoardingPass { get; set; }
    }
} 