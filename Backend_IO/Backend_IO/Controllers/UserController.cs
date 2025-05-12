using Backend_IO.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace Backend_IO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetMyInfo()
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return Unauthorized("User no longer exists.");

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Role,
                user.Email,
                user.FirstName,
                user.LastName
            });
        }


        [Authorize(Roles = "Employee")]
        [HttpGet("user/by-username/{username}")]
        public IActionResult GetUserInfoByUsername(string username)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return Unauthorized("Employee no exists.");

            user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return NotFound("User not found.");

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Role,
                user.Email,
                user.FirstName,
                user.LastName
            });
        }

        [Authorize]
        [HttpDelete("delete-account")]
        public IActionResult DeleteAccount()
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return Unauthorized("Employee no exists.");

            var activeBookings = _context.Bookings
            .Where(b => b.UserId == userId && b.Status != "Completed" && b.Status != "Cancelled")
            .ToList();

            if (activeBookings.Any())
            {
                return BadRequest("Cannot delete user because there are active bookings.");
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            HttpContext.SignOutAsync();

            return Ok("Account deleted successfully.");
        }


        [Authorize(Roles = "Employee")]
        [HttpDelete("delete-account/{userId}")]
        public IActionResult DeleteAccountById(int userId)
        {
            var userId_adm = int.Parse(User.FindFirst("userId")?.Value);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId_adm);

            if (user == null)
                return Unauthorized("Employee no exists.");

            user = _context.Users.Find(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }


            var activeBookings = _context.Bookings
            .Where(b => b.UserId == userId && b.Status != "Completed" && b.Status != "Cancelled")
            .ToList();

            if (activeBookings.Any())
            {
                return BadRequest("Cannot delete user because there are active bookings.");
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            return Ok("Account deleted successfully.");
        }

        [HttpGet("all-users")]
        [Authorize(Roles = "Employee")]
        public IActionResult AllUsers()
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return Unauthorized("Employee no exists.");

            var users = _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.Role,  
                    u.FirstName,  
                    u.LastName, 
                    u.DateOfBirth
                })
                .ToList();

            return Ok(users);
        }
    }
}
