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
    }
}
