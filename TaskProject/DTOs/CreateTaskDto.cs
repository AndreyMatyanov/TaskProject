using System.ComponentModel.DataAnnotations;

namespace TaskProject.DTOs
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = String.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = "Todo";
        public string Priority { get; set; } = "Medium";
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
