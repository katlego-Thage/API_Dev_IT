using System.ComponentModel.DataAnnotations;

namespace API_Dev_IT.Model
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        public string? Username { get; set; }
        public string? PasswordHash { get; set; }
        public string? Email { get; set; }
        public int? RoleID { get; set; } = null;
        public DateTime? CreatedAt { get; set; }
    }
}
