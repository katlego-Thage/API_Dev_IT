using System.ComponentModel.DataAnnotations;

namespace API_Dev_IT.Model
{
    public class LogIn
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(256)]
        public string email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6)]
        public string password { get; set; } = string.Empty;
    }
}
