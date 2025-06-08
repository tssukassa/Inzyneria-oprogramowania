using Backend_IO.Models;
using Backend_IO.Services;
using Microsoft.AspNetCore.Mvc;
using Backend_IO.DTO;
using Microsoft.AspNetCore.Authorization;


namespace Backend_IO.Controllers
{
    /*
      * AuthController
      * 
      * This controller handles authentication-related actions, including:
      * - Registering a new user
      * - Logging in an existing user and generating a JWT token
      * 
      * Endpoints:
      * - POST /api/auth/register
      * - POST /api/auth/login
      */

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        // Constructor injecting the authentication service
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /*
         * Register
         * 
         * Registers a new user with the provided registration data.
         * 
         * Request Body:
         * - RegisterDto dto: Contains username, password, role key, personal details, etc.
         * 
         * Returns:
         * - 200 OK if the registration was successful
         * - 400 BadRequest if a user with the same username already exists
         */
        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            var success = _authService.Register(dto);
            if (!success)
            {
                // Registration failed because user already exists
                return BadRequest("A user with this login already exists.");
            }

            var user = _authService.GetUser(dto.Username);
            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token });
            // Registration succeeded
            //return Ok("The user has been successfully registered.");
            //return Ok(true);
        }

        /*
         * Login
         * 
         * Authenticates a user and generates a JWT token for session-based access.
         * 
         * Request Body:
         * - LoginDto dto: Contains the username and plaintext password.
         * 
         * Returns:
         * - 200 OK with a JWT token if authentication is successful
         * - 401 Unauthorized if credentials are invalid
         */
        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            // Fetch user by username
            var user = _authService.GetUser(dto.Username);

            // Validate credentials
            if (user == null || !_authService.VerifyPassword(user.PasswordHash, dto.Password))
            {
                return Unauthorized("Incorrect login or password.");
            }

            // Generate JWT token and return it
            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token });
        }
    }
}
