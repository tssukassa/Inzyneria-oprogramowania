using System.ComponentModel.DataAnnotations;

namespace Backend_IO.Models
{

    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; } 

        [Required]
        public string Role { get; set; } 

        [Required]
        public string FirstName { get; set; }     

        [Required]
        public string LastName { get; set; }        

        [EmailAddress]
        public string? Email { get; set; }          

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }   
    }
}

