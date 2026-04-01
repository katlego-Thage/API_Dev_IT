using API_Dev_IT.Context;
using API_Dev_IT.IService;
using API_Dev_IT.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_Dev_IT.Service
{
    public class JwtService : IJwt
    {
        private readonly IConfiguration _configuration;
        private readonly BookingContext _context;
        public JwtService(IConfiguration configuration, BookingContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public async Task<IActionResult> GenerateToken(User user)
        {
            var role = await _context.role
                       .FirstOrDefaultAsync(x => x.RoleID == user.RoleID);

            var claim = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier,user?.Username??string.Empty),
                new Claim(ClaimTypes.Email,user?.Email??string.Empty),
                new Claim(ClaimTypes.Role,role?.RoleName??string.Empty)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes
                          (_configuration["JWT:SecreteKey"]??string.Empty));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer: _configuration["JWT:Issuer"],
                                             audience: _configuration["JWT:Audiance"],
                                             claims: claim,
                                             expires: DateTime.Now.AddDays(7),
                                             signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new OkObjectResult(jwt);
        }
    }
}
