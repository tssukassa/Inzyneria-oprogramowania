using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_IO.Models
{
    public class Booking
    {
        public int Id { get; set; } // Идентификатор бронирования (PK)

        [Required]
        public int UserId { get; set; } // FK: связь с пользователем

        [Required]
        public int FlightId { get; set; } // FK: связь с перелётом

        [Required]
        public DateTime BookingDate { get; set; } // Дата бронирования

        [Required]
        public string Status { get; set; } // Статус (например, Confirmed, Pending, Cancelled)

        public virtual User User { get; set; } // Навигационное свойство для связи с пользователем

        public virtual Flight Flight { get; set; } // Навигационное свойство для связи с перелётом
    }
}
