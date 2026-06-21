using API_Dev_IT.Context;
using API_Dev_IT.IService;
using API_Dev_IT.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace API_Dev_IT.Service
{
    public class JwtService : IJwt
    {
        //private readonly IConfiguration _configuration;
        //private readonly BookingContext _context;
        //public JwtService(IConfiguration configuration, BookingContext context)
        //{
        //    _configuration = configuration;
        //    _context = context;
        //}
        //public async Task<IActionResult> GenerateToken(User user)
        //{
        //    var role = await _context.role
        //               .FirstOrDefaultAsync(x => x.RoleID == user.RoleID);

        //    var claim = new Claim[]
        //    {
        //        new Claim(ClaimTypes.NameIdentifier,user.UserID.ToString()),
        //        new Claim(ClaimTypes.Email,user?.Email??string.Empty),
        //        new Claim(ClaimTypes.Role,role?.RoleName??string.Empty)
        //    };

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes
        //                  (_configuration["JWT:SecreteKey"]??string.Empty));

        //    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(issuer: _configuration["JWT:Issuer"],
        //                                     audience: _configuration["JWT:Audiance"],
        //                                     claims: claim,
        //                                     expires: DateTime.Now.AddDays(7),
        //                                     signingCredentials: credentials);

        //    var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        //    return new OkObjectResult(jwt);
        //}
        /// <summary>
        /// Secure JWT token generation and validation service with structured logging
        /// </summary>
        private readonly IConfiguration _configuration;
        private readonly BookingContext _context;
        private readonly ILogger<JwtService> _logger;

        public JwtService(IConfiguration configuration,BookingContext context,
            ILogger<JwtService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            ValidateConfiguration();
        }

        private void ValidateConfiguration()
        {
            var secretKey = _configuration["JWT:SecretKey"];

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                _logger.LogError($"JWT:SecretKey is not configured");
                throw new InvalidOperationException("JWT:SecretKey is not configured. " +
                                                    "Add a strong key (min 32 chars) " +
                                                    "to appsettings.json or environment variables.");
            }

            var keyBytes = Encoding.UTF8.GetBytes(secretKey);

            if (keyBytes.Length < int.Parse(_configuration["JWT:MinimumKeyLengthBytes"] ?? "32"))
            {
                var requiredBytes = int.Parse(_configuration["JWT:MinimumKeyLengthBytes"] ?? "32");
                _logger.LogError($"JWT:SecretKey is too short. Required: {requiredBytes} bytes, " +
                                 $"Actual: {keyBytes.Length} bytes");

                throw new InvalidOperationException(
                                                    $"JWT:SecretKey must be at least " +
                                                    $"{requiredBytes} bytes (256 bits). " +
                                                    $"Current length: {keyBytes.Length} bytes. " +
                                                    "Generate a new key: openssl rand -base64 32");
            }

            var issuer = _configuration["JWT:Issuer"];
            var audience = _configuration["JWT:Audience"];

            if (string.IsNullOrWhiteSpace(issuer))
            {
                _logger.LogWarning($"JWT:Issuer is not configured");
            }

            if (string.IsNullOrWhiteSpace(audience))
            {
                _logger.LogWarning($"JWT:Audience is not configured");
            }

            _logger.LogInformation($"JWT configuration validated successfully. " +
                                   $"Key length: {keyBytes.Length} bytes");
        }

        public async Task<AuthResponseDto> GenerateToken(User user)
        {
            if (user == null)
            {
                _logger.LogWarning("Token generation failed: user is null");
                throw new ArgumentNullException(nameof(user));
            }

            try
            {
                _logger.LogInformation($"Generating token for user {user.UserID} with email {user.Email}");

                var role = await _context.role
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.RoleID == user.RoleID);

                var roleName = role?.RoleName ?? "User";

                var claims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Sub, user.UserID.ToString()),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                        ClaimValueTypes.Integer64),
                    new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                    new(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                    new(ClaimTypes.Email, user.Email ?? string.Empty),
                    new(ClaimTypes.Role, roleName)
                };

                var key = GetSigningKey();
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var issuer = _configuration["JWT:Issuer"] ?? "API_Dev_IT";
                var audience = _configuration["JWT:Audience"] ?? "BookingClient";
                var accessTokenExpiry = DateTime.UtcNow.AddHours(int.Parse(_configuration["JWT:AccessTokenExpiryHours"] ?? "2"));

                var tokenDescriptor = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: accessTokenExpiry,
                    signingCredentials: credentials
                );

                var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
                var refreshToken = GenerateRefreshToken();
                var refreshTokenExpiry = DateTime.UtcNow.AddDays(int.Parse(_configuration["JWT:RefreshTokenExpiryDays"] ?? "7"));

                _logger.LogInformation("Token generated successfully for user {UserId}. " +
                                       "TokenId: {TokenId}, Expires: {ExpiresAt}",user.UserID,
                                       claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value,
                                       accessTokenExpiry);

                return new AuthResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = accessTokenExpiry,
                    User = new UserDto
                    {
                        UserId = user.UserID,
                        Username = user.Username ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        RoleName = roleName,
                        CreatedAt = user.CreatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Token generation failed for user {UserId}",
                    user.UserID);
                throw new InvalidOperationException("Authentication failed. Please try again.");
            }
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("Token validation failed: token is null or empty");
                return null;
            }

            try
            {
                _logger.LogDebug("Validating JWT token");

                var handler = new JwtSecurityTokenHandler();
                var key = GetSigningKey();
                var issuer = _configuration["JWT:Issuer"] ?? "API_Dev_IT";
                var audience = _configuration["JWT:Audience"] ?? "BookingClient";

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5),
                    RequireExpirationTime = true,
                    RequireSignedTokens = true
                };

                var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);

                _logger.LogDebug(
                    "Token validated successfully. TokenId: {TokenId}",
                    principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value);

                return principal;
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogWarning(ex, "JWT token has expired");
                return null;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "JWT token validation failed: invalid signature or format");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during JWT validation");
                return null;
            }
        }

        private static string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private SymmetricSecurityKey GetSigningKey()
        {
            var secretKey = _configuration["JWT:SecretKey"]!;
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }
    }
}
