using System.ComponentModel.DataAnnotations;

namespace API_Dev_IT.Model
{
    public class Tenant
    {
        [Key]
        public int TenantID { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }
}
