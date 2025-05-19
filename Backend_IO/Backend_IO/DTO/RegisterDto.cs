using System.ComponentModel.DataAnnotations;

namespace Backend_IO.DTO
{
    /*
     * Data Transfer Object (DTO) for user registration.
     * 
     * This DTO is used to collect and transfer user registration details from the client.
     * It includes all necessary user information for creating a new account.
     * 
     * Properties:
     * - Username (string, required): The desired username for the new user.
     * - Password (string, required): The password for the new user.
     * - RoleKey (string, optional): The role identifier key assigned to the user (e.g., "User", "Employee").
     * - FirstName (string, required): The user's first name.
     * - LastName (string, required): The user's last name.
     * - DateOfBirth (DateTime, required): The user's birth date.
     * - Email (string, optional): The user's email address. Must be a valid email format if provided.
     */

    public class RegisterDto
    {
        [Required]
        public string Username { get; set; } // Desired username
        [Required]
        public string Password { get; set; } // Password for the account
        public string RoleKey { get; set; }  // Optional role key

        [Required]
        public string FirstName { get; set; } // User's first name
        [Required]
        public string LastName { get; set; }  // User's last name
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; } // Date of birth
        public string? Email { get; set; }   // Optional email address
    }

}
