namespace JWTAuthApp.Models
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public DateTime ExpiredOn { get; set; }
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
