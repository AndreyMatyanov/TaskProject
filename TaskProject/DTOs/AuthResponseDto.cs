namespace TaskProject.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = String.Empty;
        public int UserId { get; set; }
        public string Email { get; set; } = String.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
