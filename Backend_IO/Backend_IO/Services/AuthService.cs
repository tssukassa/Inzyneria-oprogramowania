using Backend_IO.Data;
using Backend_IO.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend_IO.DTO;


namespace Backend_IO.Services
{
    /*
     * AuthService
     * 
     * This service handles user authentication, including:
     * - Password hashing and verification
     * - User registration and login
     * - JWT token generation for authenticated sessions
     * 
     * It interacts with the ApplicationDbContext to store and retrieve user data.
     */

    public class AuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * HashPassword
         * 
         * Generates a salted hash of the given plaintext password using PBKDF2.
         * 
         * Parameters:
         * - string password: The plaintext password to hash.
         * 
         * Returns:
         * - A string combining the salt and hash, separated by a period.
         */
        public string HashPassword(string password)
        {
            byte[] salt = new byte[16];

            // Generate a cryptographically secure random salt
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Hash the password with the salt using PBKDF2
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        /*
        * VerifyPassword
        * 
        * Verifies if the provided password matches the stored hashed password.
        * 
        * Parameters:
        * - string hashedPassword: The stored salt+hash format password.
        * - string password: The plaintext password to verify.
        * 
        * Returns:
        * - True if the password is correct, false otherwise.
        */
        public bool VerifyPassword(string hashedPassword, string password)
        {
            var parts = hashedPassword.Split('.');
            var salt = Convert.FromBase64String(parts[0]);
            var hash = parts[1];

            string hashToCheck = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hash == hashToCheck;
        }

        /*
         * Register
         * 
         * Registers a new user if the username is not already taken.
         * Assigns role based on the RoleKey provided in the registration data.
         * 
         * Parameters:
         * - RegisterDto dto: Registration data containing user credentials and profile.
         * 
         * Returns:
         * - True if registration is successful, false if the user already exists.
         */
        public bool Register(RegisterDto dto)
        {
            var existingUser = _context.Users.SingleOrDefault(u => u.Username == dto.Username);
            if (existingUser != null)
            {
                return false; 
            }

            // Determine role based on role key
            string role = dto.RoleKey switch
            {
                "worker123" => "Employee",
                "partner321" => "Partner",
                _ => "Client" 
            };

            string hashedPassword = HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = hashedPassword,
                Role = role,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return true;
        }

        /*
        * Login
        * 
        * Validates user credentials.
        * 
        * Parameters:
        * - string username: The user's username.
        * - string password: The user's plaintext password.
        * 
        * Returns:
        * - True if login is successful, false otherwise.
        */
        public bool Login(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == username);
            if (user == null)
            {
                return false; 
            }

            return VerifyPassword(user.PasswordHash, password);
        }

        /*
         * GenerateJwtToken
         * 
         * Creates a JWT token for the authenticated user.
         * Token contains claims for username, role, and user ID.
         * 
         * Parameters:
         * - User user: The authenticated user object.
         * 
         * Returns:
         * - A signed JWT token as a string.
         */
        public string GenerateJwtToken(User user)
        {
            var jwtSettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("userId", user.Id.ToString()) 
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /*
         * GetUser
         * 
         * Retrieves a user by their username.
         * 
         * Parameters:
         * - string username: The username to search for.
         * 
         * Returns:
         * - The User object if found, otherwise null.
         */
        public User GetUser(string username)
        {
            return _context.Users.SingleOrDefault(u => u.Username == username);
        }

    }

}
