using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_Dev_IT.Context
{
    public class BookingContext : DbContext
    {
        public BookingContext(DbContextOptions<BookingContext> options) : base(options)
        {
        }
        public DbSet<Model.User> User { get; set; }
        public DbSet<Model.Booking> booking { get; set; }
        public DbSet<Model.Payment> payment { get; set; }
        public DbSet<Model.Role> role { get; set; }
        public DbSet<Model.Room> room { get; set; }
        public DbSet<Model.Tenant> tenant { get; set; }
    }
}
