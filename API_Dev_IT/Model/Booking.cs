using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API_Dev_IT.Model
{
    public class Booking
    {
        [Key]

        [JsonRequired]
        public int? BookingID { get; set; }
        [JsonRequired]
        public int TenantID { get; set; }
        [JsonRequired]
        public int RoomID { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public string? Status { get; set; }
        public int? UserId { get; set; }
    }
}
