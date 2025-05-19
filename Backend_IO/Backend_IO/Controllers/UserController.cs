using Backend_IO.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_IO.Controllers
{

    /*
     * UserController
     * 
     * This controller provides endpoints related to user information retrieval and account management.
     * 
     * Authorization:
     * - Most endpoints require the user to be authenticated.
     * - Certain endpoints require the user to have the "Employee" role.
     */
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
          * GET: /api/user/me
          * 
          * Retrieves information about the currently authenticated user.
          * 
          * Authorization:
          * - Requires authentication.
          * 
          * Returns:
          * - 200 OK with user details:
          *   - Id
          *   - Username
          *   - Role
          *   - Email
          *   - FirstName
          *   - LastName
          * - 401 Unauthorized if user no longer exists or not authenticated.
          */
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


        /*
         * GET: /api/user/user/by-username/{username}
         * 
         * Retrieves information about a user identified by their username.
         * 
         * Authorization:
         * - Requires user to have "Employee" role.
         * 
         * Parameters:
         * - username: string - username of the user to retrieve.
         * 
         * Returns:
         * - 200 OK with user details (Id, Username, Role, Email, FirstName, LastName).
         * - 401 Unauthorized if the requesting employee does not exist or is not authenticated.
         * - 404 Not Found if the target user does not exist.
         */
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

        /*
         * DELETE: /api/user/delete-account
         * 
         * Deletes the account of the currently authenticated user.
         * 
         * Authorization:
         * - Requires authentication.
         * 
         * Conditions:
         * - The user cannot have any active bookings (bookings with status other than "Completed" or "Cancelled").
         * 
         * Returns:
         * - 200 OK if account deleted successfully.
         * - 400 BadRequest if active bookings prevent deletion.
         * - 401 Unauthorized if the user no longer exists.
         */
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


        /*
         * DELETE: /api/user/delete-account/{userId}
         * 
         * Deletes the account of a user identified by their userId.
         * 
         * Authorization:
         * - Requires user to have "Employee" role.
         * 
         * Conditions:
         * - The user to be deleted cannot have any active bookings (bookings with status other than "Completed" or "Cancelled").
         * 
         * Parameters:
         * - userId: int - ID of the user to delete.
         * 
         * Returns:
         * - 200 OK if account deleted successfully.
         * - 400 BadRequest if active bookings prevent deletion.
         * - 401 Unauthorized if the requesting employee does not exist or is not authenticated.
         * - 404 Not Found if the target user does not exist.
         */
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

        /*
         * GET: /api/user/all-users
         * 
         * Retrieves a list of all users with selected details.
         * 
         * Authorization:
         * - Requires user to have "Employee" role.
         * 
         * Returns:
         * - 200 OK with list of users (Id, Username, Email, Role, FirstName, LastName, DateOfBirth).
         * - 401 Unauthorized if the requesting employee does not exist or is not authenticated.
         */
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
