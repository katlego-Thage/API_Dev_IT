using System.ComponentModel.DataAnnotations;

namespace API_Dev_IT.Model
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }
        public int BookingID { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? PaymentMethod { get; set; }
    }
}
