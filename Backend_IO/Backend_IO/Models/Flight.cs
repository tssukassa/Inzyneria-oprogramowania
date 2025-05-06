using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_IO.Models
{
    public class Flight
    {
        public int Id { get; set; } // Идентификатор перелета (PK)

        [Required]
        public string Origin { get; set; } // Город отправления

        [Required]
        public string Destination { get; set; } // Город назначения

        [Required]
        public DateTime DepartureTime { get; set; } // Время вылета

        [Required]
        public DateTime ArrivalTime { get; set; } // Время прибытия

        [Required]
        public decimal Price { get; set; } // Стоимость билета

        [Required]
        public string FlightNumber { get; set; } // Номер рейса
    }
}
