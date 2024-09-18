using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace InstaHub.Models
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<WhatsAppMessage> WhatsAppMessages { get; set; 
        public DbSet<Customer> Customers { get; set; }
    }
}
