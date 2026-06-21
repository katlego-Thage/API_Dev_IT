using API_Dev_IT.Model;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_Dev_IT.IService
{
    public interface IJwt
    {
        Task<AuthResponseDto> GenerateToken(User user);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
