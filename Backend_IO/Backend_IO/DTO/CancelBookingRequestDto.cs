namespace Backend_IO.DTO
{
    public class CancelBookingRequestDto
    {
        public string CardNumber { get; set; }
        public string CVV2 { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
