using System.ComponentModel.DataAnnotations;

namespace Backend_IO.DTO
{
    public class BookingDto
    {
        /*
         * Data Transfer Object (DTO) used for creating a new booking.
         * 
         * This object carries the necessary information from client to server
         * when a user wants to book a flight.
         * 
         * Properties:
         * - FlightId (int): The identifier of the flight being booked.
         * - CardNumber (string): The bank card number used for payment.
         * - CVV2 (string): The CVV2 security code of the card.
         * - ExpirationDate (DateTime): Expiration date of the bank card.
         * 
         * Note:
         * - All properties are required to process the booking and payment.
         */

        public int FlightId { get; set; } // Flight identifier for the booking
        public string CardNumber { get; set; } // Payment card number
        public string CVV2 { get; set; } // Card security code
        public DateTime ExpirationDate { get; set; } // Card expiration date
    }
}
