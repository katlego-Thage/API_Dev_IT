using System.ComponentModel.DataAnnotations;

namespace API_Dev_IT.Model
{
    public class LogIn
    {
        [EmailAddress]
        public string? email { get; set; }

        public string? password { get; set; }
    }
}
