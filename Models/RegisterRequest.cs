using System.ComponentModel.DataAnnotations;

namespace FigurasQE_AuthenticationService.Models;

public class RegisterRequest
{
    [Required]
    [StringLength(120, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 120 characters.")]
    public string Name { get; set; }

    public string Email { get; set; }
    public string Password { get; set; }
    public int Age { get; set; }
    public char Genre { get; set; }
    public string Country { get; set; }
    public string Neurodivergency { get; set; }
    public string Role { get; set; }
}