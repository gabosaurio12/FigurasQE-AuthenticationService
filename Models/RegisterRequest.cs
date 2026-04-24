using System.ComponentModel.DataAnnotations;

namespace FigurasQE_AuthenticationService.Models;

public class RegisterRequest
{
    [Required]
    [StringLength(120, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 120 characters.")]
    public string Name { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public int Age { get; set; }
    [Required]
    public char Genre { get; set; }
    [Required]
    public string Country { get; set; }
    [Required]
    public string Role { get; set; }
    public string? Neurodivergency { get; set; }
    public string? Degree { get; set; }
}