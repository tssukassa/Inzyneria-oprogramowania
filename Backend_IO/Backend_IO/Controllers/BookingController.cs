using Backend_IO.Data;
using Backend_IO.DTO;
using Backend_IO.Models;
using Backend_IO.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class BookingController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly AuthService _authService;

    public BookingController(ApplicationDbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }

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
        {
            return BadRequest("Flight with this ID was not found.");
        }

        var booking = new Booking
        {
            UserId = userId,
            FlightId = bookingDto.FlightId,
            BookingDate = DateTime.Now,  
            Status = "Pending" 
        };

        _context.Bookings.Add(booking);
        _context.SaveChanges();

        return Ok("The order has been successfully created.");
    }

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
            Price = b.Flight.Price,
        }).ToList();

        return Ok(result);
    }

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
            Price = b.Flight.Price
        }).ToList();

        return Ok(results);
    }


    [Authorize]
    [HttpPost("cancel-booking/{bookingId}")]
    public IActionResult CancelBookingByUser(int bookingId)
    {
        int userId = int.Parse(User.FindFirst("userId")?.Value);
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
            return Unauthorized("User no exists.");

        var booking = _context.Bookings
            .FirstOrDefault(b => b.Id == bookingId && b.UserId == userId);

        if (booking == null)
            return NotFound("Booking not found or does not belong to the user.");

        booking.Status = "Cancelled";
        _context.SaveChanges();

        return Ok("Booking cancelled successfully.");
    }

    [Authorize(Roles = "Employee")]
    [HttpPost("cancel-booking-admin/{bookingId}")]
    public IActionResult CancelBookingByEmployee(int bookingId)
    {
        int userId = int.Parse(User.FindFirst("userId")?.Value);
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
            return Unauthorized("User no exists.");

        var booking = _context.Bookings.FirstOrDefault(b => b.Id == bookingId);

        if (booking == null)
            return NotFound("Booking not found.");

        booking.Status = "Cancelled";
        _context.SaveChanges();

        return Ok("Booking cancelled by employee.");
    }

    [Authorize(Roles = "Employee,Partner")]
    [HttpPost("cancel-flight-bookings/{flightId}")]
    public IActionResult CancelAllBookingsByFlight(int flightId)
    {
        int userId = int.Parse(User.FindFirst("userId")?.Value);
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
            return Unauthorized("User no exists.");

        var bookings = _context.Bookings
            .Where(b => b.FlightId == flightId && b.Status != "Cancelled")
            .ToList();

        if (!bookings.Any())
            return NotFound("No active bookings found for this flight.");

        foreach (var booking in bookings)
        {
            booking.Status = "Cancelled";
        }

        _context.SaveChanges();

        return Ok("All bookings for the flight have been cancelled.");
    }

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
            booking.Status = newStatus;
        }

        _context.SaveChanges();

        return Ok($"All bookings for flight ID {flightId} updated to status '{newStatus}'.");
    }

}

