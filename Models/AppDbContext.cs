using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace InstaHub.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Customer - Ticket: One-to-Many Relationship
            modelBuilder.Entity<Customer>()
                .HasMany<Ticket>(c => c.Tickets)          // A customer has many tickets
                .WithOne()                               // Each ticket has one customer
                .HasForeignKey(t => t.CustomerId)       // CustomerId in the Ticket entity is the foreign key
                .IsRequired();                         // CustomerId is a required field


            // Ticket - Message: One-to-Many Relationship
            modelBuilder.Entity<Message>()
                .HasOne<Ticket>()
                .WithMany(t => t.Messages)
                .HasForeignKey(m => m.TicketId)
                .IsRequired(); // TicketId is a required field

            // Ticket - Category: One-to-Many Relationship
            modelBuilder.Entity<Ticket>()
                .HasOne<Category>(t => t.Category)
                .WithMany()
                .IsRequired(); // CategoryId is a required field

            modelBuilder.Entity<TicketAdmin>()
              .HasKey(ta => new { ta.TicketId, ta.AdminId });

            modelBuilder.Entity<TicketAdmin>()
                .HasOne(ta => ta.Ticket)
                .WithMany(t => t.TicketAdmins)
                .HasForeignKey(ta => ta.TicketId);

            modelBuilder.Entity<TicketAdmin>()
                .HasOne(ta => ta.Admin)
                .WithMany(a => a.TicketAdmins)
                .HasForeignKey(ta => ta.AdminId);

            base.OnModelCreating(modelBuilder);
        }

        // DbSets representing your entities in the database.
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<WhatsAppMessage> WhatsAppMessages { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }
}
