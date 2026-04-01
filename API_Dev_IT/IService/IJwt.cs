using API_Dev_IT.Model;
using Microsoft.AspNetCore.Mvc;

namespace API_Dev_IT.IService
{
    public interface IJwt
    {
        Task<IActionResult> GenerateToken(User user);
    }
}
