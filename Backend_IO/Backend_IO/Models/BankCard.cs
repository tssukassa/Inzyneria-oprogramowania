using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_IO.Models
{
    public class BankCard
    {
        /*
         * Represents a bank card used for payments and refunds within the booking system.
         * 
         * Properties:
         * - CardNumber (string): Unique identifier for the bank card. Acts as the primary key.
         * - CVV2 (string): 3-digit security code for verifying the card during transactions.
         * - ExpirationDate (DateTime): The month and year the card expires.
         * - Balance (decimal): Current available balance on the card, stored as REAL in SQLite.
         * 
         * Notes:
         * - All fields are required.
         * - This entity is used by both booking creation and refund logic.
         */

        [Key]
        [Required]
        public string CardNumber { get; set; }  // Unique card number (Primary Key)

        [Required]
        public string CVV2 { get; set; }  // Card verification code (3-digit security code)

        [Required]
        public DateTime ExpirationDate { get; set; } // Expiration month and year of the card

        [Required]
        [Column(TypeName = "REAL")]
        public decimal Balance { get; set; }  // Available balance on the card (REAL type in SQLite)
    }
}

