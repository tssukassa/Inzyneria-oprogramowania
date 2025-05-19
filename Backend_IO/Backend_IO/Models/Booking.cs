using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_IO.Models
{
    public class Booking
    {
        /*
         * Represents a booking/reservation made by a user for a specific flight.
         *
         * Properties:
         * - Id (int): Primary key of the booking entry.
         * - UserId (int): Foreign key referencing the user who made the booking.
         * - FlightId (int): Foreign key referencing the flight being booked.
         * - CardNumber (string): The card used to pay for the booking. Used for refunds.
         * - BookingDate (DateTime): Timestamp when the booking was created.
         * - Status (string): Current status of the booking (e.g. "Pending", "Cancelled").
         * 
         * Navigation Properties:
         * - User (User): The user who made the booking.
         * - Flight (Flight): The flight that was booked.
         *
         * Notes:
         * - Used for managing user reservations and flight cancellations.
         * - Refund logic and flight access depend on this entity.
         */

        public int Id { get; set; } // Unique identifier for the booking

        [Required]
        public int UserId { get; set; } // Foreign key to the User who made the booking

        [Required]
        public int FlightId { get; set; } // Foreign key to the booked Flight

        [Required]
        public string CardNumber { get; set; } // Card used for payment (used for refunds too)

        [Required]
        public DateTime BookingDate { get; set; } // Date and time when the booking was created

        [Required]
        public string Status { get; set; }  // Current booking status ("Pending", "Cancelled", etc.)

        public virtual User User { get; set; } // Navigation property to the User

        public virtual Flight Flight { get; set; } // Navigation property to the Flight

    }
}
