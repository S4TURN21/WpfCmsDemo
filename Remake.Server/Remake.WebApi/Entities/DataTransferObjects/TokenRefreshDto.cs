using System.ComponentModel.DataAnnotations;

namespace Remake.WebApi.Entities.DataTransferObjects
{
    public class TokenRefreshDto
    {
        [Required]
        public string? AccessToken { get; set; }

        [Required]
        public string? RefreshToken { get; set; }
    }
}
