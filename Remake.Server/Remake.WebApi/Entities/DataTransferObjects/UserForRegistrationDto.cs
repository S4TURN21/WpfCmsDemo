using System.ComponentModel.DataAnnotations;

namespace Remake.WebApi.Entities.DataTransferObjects
{
    public class UserForRegistrationDto
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        [Compare("Password")]
        public string? ConfirmPassword { get; set; }
    }
}
