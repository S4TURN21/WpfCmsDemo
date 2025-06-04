namespace Remake.WebApi.Entities.DataTransferObjects
{
    public class AuthResponseDto
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
