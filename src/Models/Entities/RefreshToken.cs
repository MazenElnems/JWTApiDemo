using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWTAuthApp.Models.Entities
{
    [Owned]
    [Table("RefreshTokens")]
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public bool IsExpired => DateTime.UtcNow > ExpiresOn;
        public bool IsActive => !RevokedOn.HasValue && !IsExpired;
    }
}
