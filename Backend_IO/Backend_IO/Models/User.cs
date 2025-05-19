using System.ComponentModel.DataAnnotations;

namespace Backend_IO.Models
{

    public class User
    {
        /*
         * Represents a registered user of the booking system.
         * 
         * Properties:
         * - Id (int): Unique identifier for the user (Primary Key).
         * - Username (string): The login name used for authentication.
         * - PasswordHash (string): Secure hash of the user's password.
         * - Role (string): User's access level (e.g., "User", "Employee", "Admin").
         * - FirstName (string): User's given name.
         * - LastName (string): User's surname.
         * - Email (string?, optional): User's email address (must be in valid email format if provided).
         * - DateOfBirth (DateTime): User's date of birth (validated as a date).
         * 
         * Notes:
         * - All fields except Email are required.
         * - Role determines access to role-protected endpoints.
         */

        public int Id { get; set; }  // Primary Key: unique user ID

        [Required]
        public string Username { get; set; } // Login username

        [Required]
        public string PasswordHash { get; set; } // Hashed password for secure authentication

        [Required]
        public string Role { get; set; } // User role (e.g., "User", "Employee")

        [Required]
        public string FirstName { get; set; }  // First/given name

        [Required]
        public string LastName { get; set; } // Last/family name

        [EmailAddress]
        public string? Email { get; set; } // Optional email, must be in valid format if provided        

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; } // User's date of birth
    }
}


