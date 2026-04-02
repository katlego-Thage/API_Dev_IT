using API_Dev_IT.IService;
using API_Dev_IT.Model;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_Dev_IT.Helper
{
    public class UserRoleHelper //note to self: not sure will use this, but leave it as the code expands 
    {

        public static string GetRole()// ok use to validate if login user has the role
        {
            ClaimsPrincipal role = new();
            return role.FindFirst(System.Security.Claims
                   .ClaimTypes.Role)?.Value;
        }

        public static int GetUserId()
        {
            ClaimsPrincipal userId = new();
            return int.Parse(userId.FindFirst(System.Security
                   .Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        }
    }
}