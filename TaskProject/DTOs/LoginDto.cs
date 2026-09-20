using System.ComponentModel.DataAnnotations;

namespace TaskProject.DTOs
{
    public class LoginDto
    {
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
    }
}
