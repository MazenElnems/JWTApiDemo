using JWTAuthApp.Models.Entities;
using System.Text.Json.Serialization;

namespace JWTAuthApp.Models
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
