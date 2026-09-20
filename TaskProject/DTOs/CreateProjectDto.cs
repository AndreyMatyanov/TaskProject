using System.ComponentModel.DataAnnotations;

namespace TaskProject.DTOs
{
    public class CreateProjectDto
    {
        public string Name { get; set; } = String.Empty;

        public string? Description
        {
            get; set;
        }
    }
}
