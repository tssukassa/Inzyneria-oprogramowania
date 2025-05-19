namespace Backend_IO.DTO
{
    /*
     * Data Transfer Object (DTO) for user login credentials.
     * 
     * This DTO is used to receive the login data from the client,
     * containing the username and password required for authentication.
     * 
     * Properties:
     * - Username (string): The username of the user attempting to log in.
     * - Password (string): The user's password.
     * 
     * Both fields are required for the authentication process.
     */
    public class LoginDto
    {
        public string Username { get; set; }  // User's login name

        public string Password { get; set; } // User's password
    }
}
