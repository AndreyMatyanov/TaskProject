using System.ComponentModel.DataAnnotations;

namespace TaskProject.DTOs
{
    public class CreateUserDto
    {
        public string Name { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
    }
}
