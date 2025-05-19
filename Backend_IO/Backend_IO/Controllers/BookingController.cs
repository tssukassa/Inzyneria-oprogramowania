using Backend_IO.Data;
using Backend_IO.DTO;
using Backend_IO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


/*
 * BookingController
 * 
 * Handles all booking-related operations for flights including:
 * - Creating bookings
 * - Viewing user's bookings
 * - Searching bookings (for employees)
 * - Cancelling bookings (by users and staff)
 * - Mass cancelling bookings for a flight
 * - Updating booking status in bulk
 * 
 * Authorization:
 * - Most endpoints require JWT-based authorization
 * - Some endpoints restricted to Employee or Partner roles
 */
[Route("api/[controller]")]
[ApiController]
public class BookingController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly BankDbContext _bankContext;

    public BookingController(ApplicationDbContext context, BankDbContext bankContext)
    {
        _context = context;
        _bankContext = bankContext;
    }

    /*
     * POST: /api/booking/create
     * 
     * Creates a booking for a flight by an authenticated user.
     * Validates flight existence, card information, and available balance.
     * Deducts funds and saves the booking.
     * 
     * Request Body: BookingDto
     * Returns:
     * - 200 OK if booking is successful
     * - 400 BadRequest if validation fails
     * - 401 Unauthorized if user not found
     */
    [Authorize]
    [HttpPost("create")]
    public IActionResult CreateBooking([FromBody] BookingDto bookingDto)
    {
        int userId = int.Parse(User.FindFirst("userId")?.Value);
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
            return Unauthorized("User no exists.");

        var flight = _context.Flights.SingleOrDefault(f => f.Id == bookingDto.FlightId);
        if (flight == null)
            return BadRequest("Flight with this ID was not found.");

        var card = _bankContext.BankCards.FirstOrDefault(c =>
        c.CardNumber == bookingDto.CardNumber &&
        c.CVV2 == bookingDto.CVV2 &&
        c.ExpirationDate.Date == bookingDto.ExpirationDate.Date);

        if (card == null)
            return BadRequest("Bank card not found or invalid.");

        if (card.Balance < flight.Price)
            return BadRequest("Insufficient funds.");


        var booking = new Booking
        {
            UserId = userId,
            FlightId = bookingDto.FlightId,
            BookingDate = DateTime.Now,  
            Status = "Pending",
            CardNumber = bookingDto.CardNumber
        };

        _context.Bookings.Add(booking);

        _context.SaveChanges();

        card.Balance -= flight.Price;

        _bankContext.SaveChanges();

        return Ok("The order has been successfully created.");
    }


    /*
   * GET: /api/booking/my-bookings
   * 
   * Retrieves all bookings made by the currently authenticated user.
   * 
   * Returns:
   * - 200 OK with booking details including flight information
   * - 401 Unauthorized if user is not found
   */
    [HttpGet("my-bookings")] 
    [Authorize]
    public IActionResult GetBookings()
    {
        var userId = int.Parse(User.FindFirst("userId")?.Value);
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
            return Unauthorized("User no exists.");

        IQueryable<Booking> bookings;

        bookings = _context.Bookings
            .Where(b => b.UserId == user.Id)
            .Include(b => b.Flight);
        
        var result = bookings.Select(b => new
        {
            b.Id,
            b.Status,
            b.BookingDate,
            FlightNumber = b.Flight.FlightNumber,
            From = b.Flight.Origin,
            To = b.Flight.Destination,
            Departure = b.Flight.DepartureTime,
            Arrival = b.Flight.ArrivalTime,
            b.CardNumber,
            Price = b.Flight.Price
        }).ToList();

        return Ok(result);
    }


    /*
     * GET: /api/booking/search-bookings
     * 
     * Searches all bookings based on optional filters (userId, flightId, date, status).
     * Accessible only by Employees.
     * 
     * Query Parameters:
     * - int? userId
     * - int? flightId
     * - DateTime? date
     * - string? status
     * 
     * Returns:
     * - 200 OK with filtered booking list
     * - 401 Unauthorized if employee is not found
     */
    [Authorize(Roles = "Employee")]
    [HttpGet("search-bookings")]
    public IActionResult SearchBookings(
     int? userId,
     int? flightId,
     DateTime? date,
     string? status)
    {
        var userId_adm = int.Parse(User.FindFirst("userId")?.Value);
        var user_adm = _context.Users.FirstOrDefault(u => u.Id == userId_adm);

        if (user_adm == null)
            return Unauthorized("Employee no exists.");

        var query = _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Flight)
            .AsQueryable();

        if (userId.HasValue)
            query = query.Where(b => b.UserId == userId.Value);

        if (flightId.HasValue)
            query = query.Where(b => b.FlightId == flightId.Value);

        if (date.HasValue)
            query = query.Where(b => b.BookingDate.Date == date.Value.Date);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(b => b.Status == status);

        var results = query.Select(b => new
        {
            b.Id,
            b.Status,
            b.BookingDate,
            User = b.User.Username,
            FlightNumber = b.Flight.FlightNumber,
            From = b.Flight.Origin,
            To = b.Flight.Destination,
            Departure = b.Flight.DepartureTime,
            Arrival = b.Flight.ArrivalTime,
            //b.CardNumber,
            Price = b.Flight.Price
        }).ToList();

        return Ok(results);
    }


    /*
     * POST: /api/booking/cancel-booking/{bookingId}
     * 
     * Allows a user to cancel their own booking.
     * Refunds 90% of the flight price to the associated bank card.
     * 
     * Path Parameter:
     * - bookingId (int): ID of the booking to cancel
     * 
     * Request Body:
     * - CancelBookingRequestDto (optional): card details for refund
     * 
     * Returns:
     * - 200 OK on successful cancellation
     * - 400/404 for validation failures
     */
    [Authorize]
    [HttpPost("cancel-booking/{bookingId}")]
    public IActionResult CancelBookingByUser(int bookingId, [FromBody] CancelBookingRequestDto? cardInfo)
    {
        int userId = int.Parse(User.FindFirst("userId")?.Value);
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
            return Unauthorized("User no exists.");

        var booking = _context.Bookings
            .Include(b => b.Flight)
            .FirstOrDefault(b => b.Id == bookingId && b.UserId == userId);

        if (booking == null)
            return NotFound("Booking not found or does not belong to the user.");

        if (booking.Status == "Cancelled")
            return BadRequest("Booking is already cancelled.");

        BankCard? refundCard = null;

        if (cardInfo != null)
        {
            refundCard = _bankContext.BankCards.FirstOrDefault(c =>
                c.CardNumber == cardInfo.CardNumber &&
                c.CVV2 == cardInfo.CVV2 &&
                c.ExpirationDate.Date == cardInfo.ExpirationDate.Date);
        }
        else
        {
            refundCard = _bankContext.BankCards.FirstOrDefault(c => c.CardNumber == booking.CardNumber);
        }

        if (refundCard == null)
            return BadRequest("Valid refund card information is required.");

        decimal refundAmount = booking.Flight.Price * 0.9m;
        refundCard.Balance += refundAmount;

        booking.Status = "Cancelled";

        _context.SaveChanges();
        _bankContext.SaveChanges();

        return Ok($"Booking cancelled. Refund of {refundAmount} has been processed.");
    }

    /*
     * POST: /api/booking/cancel-booking-admin/{bookingId}
     * 
     * Allows an Employee to cancel any booking with a full refund.
     * 
     * Returns:
     * - 200 OK on success
     * - 400/404 for validation failures
     */
    [Authorize(Roles = "Employee")]
    [HttpPost("cancel-booking-admin/{bookingId}")]
    public IActionResult CancelBookingByEmployee(int bookingId)
    {
        int userId = int.Parse(User.FindFirst("userId")?.Value);
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
            return Unauthorized("User no exists.");

        var booking = _context.Bookings
            .Include(b => b.Flight)
            .FirstOrDefault(b => b.Id == bookingId);

        if (booking == null)
            return NotFound("Booking not found.");

        if (booking.Status == "Cancelled")
            return BadRequest("Booking is already cancelled.");

        var refundCard = _bankContext.BankCards.FirstOrDefault(c => c.CardNumber == booking.CardNumber);

        if (refundCard == null)
            return BadRequest("Refund card not found in bank database.");

        decimal refundAmount = booking.Flight.Price;
        refundCard.Balance += refundAmount;

        booking.Status = "Cancelled";

        _context.SaveChanges();
        _bankContext.SaveChanges();

        return Ok($"Booking cancelled by employee. Refund of {refundAmount} has been processed.");

    }

    /*
     * POST: /api/booking/cancel-flight-bookings/{flightId}
     * 
     * Allows Employees or Partners to cancel all active bookings for a flight.
     * Issues full refunds.
     * 
     * Returns:
     * - 200 OK if successful
     * - 404 if no active bookings found
     */
    [Authorize(Roles = "Employee,Partner")]
    [HttpPost("cancel-flight-bookings/{flightId}")]
    public IActionResult CancelAllBookingsByFlight(int flightId)
    {
        int userId = int.Parse(User.FindFirst("userId")?.Value);
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
            return Unauthorized("User no exists.");

        var bookings = _context.Bookings
            .Include(b => b.Flight)
            .Where(b => b.FlightId == flightId && b.Status != "Cancelled")
            .ToList();

        if (!bookings.Any())
            return NotFound("No active bookings found for this flight.");

        foreach (var booking in bookings)
        {

            var refundCard = _bankContext.BankCards
                .FirstOrDefault(c => c.CardNumber == booking.CardNumber);

            if (refundCard != null)
            {
                decimal refundAmount = booking.Flight.Price;
                refundCard.Balance += refundAmount;
            }

            booking.Status = "Cancelled";
        }

        _context.SaveChanges();
        _bankContext.SaveChanges();

        return Ok("All bookings for the flight have been cancelled and refunds processed.");

    }

    /*
     * POST: /api/booking/update-flight-bookings-status/{flightId}
     * 
     * Updates the status of all bookings for a given flight.
     * Can be used for mass-updating bookings (e.g., mark all as "Confirmed").
     * 
     * Request Body:
     * - string newStatus: new status to apply
     * 
     * Returns:
     * - 200 OK if successful
     * - 400/404 on validation failure
     */
    [Authorize(Roles = "Employee,Partner")]
    [HttpPost("update-flight-bookings-status/{flightId}")]
    public IActionResult UpdateFlightBookingsStatus(int flightId, [FromBody] string newStatus)
    {
        int userId = int.Parse(User.FindFirst("userId")?.Value);
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
            return Unauthorized("User no exists.");

        if (string.IsNullOrWhiteSpace(newStatus))
            return BadRequest("Status is required.");

        var bookings = _context.Bookings
            .Where(b => b.FlightId == flightId)
            .ToList();

        if (!bookings.Any())
            return NotFound("No bookings found for this flight.");

        foreach (var booking in bookings)
        {
            if (booking.Status != "Cancelled")
                booking.Status = newStatus;
        }

        _context.SaveChanges();

        return Ok($"All bookings for flight ID {flightId} updated to status '{newStatus}'.");
    }

}

