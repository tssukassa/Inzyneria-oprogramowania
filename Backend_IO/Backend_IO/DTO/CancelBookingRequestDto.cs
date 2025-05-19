namespace Backend_IO.DTO
{
    /*
     * Data Transfer Object (DTO) representing the card information
     * required to process a booking cancellation refund.
     * 
     * This DTO is used when a user or employee requests to cancel a booking
     * and optionally provides bank card details to which the refund should be made.
     * 
     * Properties:
     * - CardNumber (string): The bank card number for refund.
     * - CVV2 (string): The CVV2 security code of the refund card.
     * - ExpirationDate (DateTime): Expiration date of the refund card.
     * 
     * Note:
     * - All fields are required to validate the refund card.
     * - This DTO can be optionally passed; if not provided, refund
     *   will be processed using the card linked to the booking.
     */
    public class CancelBookingRequestDto
    {
        public string CardNumber { get; set; }  // Refund card number
        public string CVV2 { get; set; } // Refund card CVV2 security code
        public DateTime ExpirationDate { get; set; }  // Refund card expiration date
    }
}
