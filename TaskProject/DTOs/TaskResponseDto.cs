namespace TaskProject.DTOs
{
    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = String.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = String.Empty;
        public string Priority { get; set; } = String.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public UserBriefDto User { get; set; } = null!;
        public ProjectBriefDto Project { get; set; } = null!;
    }
}
