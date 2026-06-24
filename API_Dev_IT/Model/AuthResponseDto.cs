using System.ComponentModel.DataAnnotations;

namespace API_Dev_IT.Model
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; } = new();
    }

    public class RefreshTokenRequestDto
    {
        [Required]
        public string AccessToken { get; set; } = string.Empty;

        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
    public class JwtConfiguration
    {
        public string? SecretKey { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public int AccessTokenExpiryHours { get; set; }
        public int RefreshTokenExpiryDays { get; set; }
        public int MinimumKeyLengthBytes { get; set; }
    }
}
