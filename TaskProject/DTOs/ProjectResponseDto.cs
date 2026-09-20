namespace TaskProject.DTOs
{
    public class ProjectResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
