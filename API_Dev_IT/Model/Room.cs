using System.ComponentModel.DataAnnotations;

namespace API_Dev_IT.Model
{
    public class Room
    {
        [Key]
        public int RoomID { get; set; }
        public string? RoomNumber { get; set; }
        public string? RoomType { get; set; }
        public decimal? PricePerNight { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
