using System.ComponentModel.DataAnnotations;

namespace Remake.WebApi.Entities.DataTransferObjects
{
    public class UserForAuthenticationDto
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
