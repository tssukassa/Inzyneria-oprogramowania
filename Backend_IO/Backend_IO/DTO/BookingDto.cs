using System.ComponentModel.DataAnnotations;

namespace Backend_IO.DTO
{
    public class BookingDto
    {
        [Required]
        public int FlightId { get; set; }
    }
}
