using System.ComponentModel.DataAnnotations;

namespace API_Dev_IT.Model
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }
        public string? RoleName { get; set; }
    }
}
