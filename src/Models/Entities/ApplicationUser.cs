using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace JWTAuthApp.Models.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [Required , MaxLength(100)]
        public string FirstName { get; set; }
        [Required , MaxLength(100)]
        public string LastName { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
