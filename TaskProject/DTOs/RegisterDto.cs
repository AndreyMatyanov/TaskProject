using System.ComponentModel.DataAnnotations;

namespace TaskProject.DTOs
{
    public class RegisterDto
    {
        public string Name { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string Role { get; set; } = "User";
    }
}
