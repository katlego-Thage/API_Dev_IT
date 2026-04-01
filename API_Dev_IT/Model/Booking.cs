using System.ComponentModel.DataAnnotations;

namespace API_Dev_IT.Model
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }
        public int TenantID { get; set; }
        public int RoomID { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public string? Status { get; set; }
        public int? UserId { get; set; }
    }
}
