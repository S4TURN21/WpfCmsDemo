
using Microsoft.AspNetCore.Identity;

namespace Remake.WebApi.Entities.Models
{
    public class User : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
