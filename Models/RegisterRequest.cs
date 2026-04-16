namespace FigurasQE_AuthenticationService.Models;

public class RegisterRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public int Age { get; set; }
    public char Genre { get; set; }
    public string Country { get; set; }
    public string Neurodivergency { get; set; }
    public string Role { get; set; }
}