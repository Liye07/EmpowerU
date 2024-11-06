using Microsoft.EntityFrameworkCore;
using EmpowerU.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace EmpowerU.Models.Data
{
    public class EmpowerUContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public EmpowerUContext(DbContextOptions<EmpowerUContext> options)
            : base(options)
        {
        }

        public DbSet<Consumer> Consumers { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<LocationService> LocationServices { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<BookingHistory> BookingHistories { get; set; }
        public DbSet<PaymentEmpowerU> Payments { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Conversation> Conversations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure TPT inheritance
            modelBuilder.Entity<User>()
                .ToTable("User"); // Common user fields

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            // Configure Consumer entity
            modelBuilder.Entity<Consumer>()
                .ToTable("Consumer")
                .Property(c => c.PreferredCategories)
                .HasMaxLength(255)
                .IsRequired(); // Make required if applicable

            // Configure Business entity
            modelBuilder.Entity<Business>()
                .ToTable("Business")
                .Property(b => b.Description)
                .HasMaxLength(1000);

            modelBuilder.Entity<Business>()
                .Property(b => b.Rating)
                .HasColumnType("decimal(5, 2)");

            // Define relationships and constraints for Business
            modelBuilder.Entity<Business>()
                .HasOne(b => b.LocationService)
                .WithMany()
                .HasForeignKey(b => b.LocationID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Business>()
                .HasMany(b => b.Reviews)
                .WithOne(r => r.Business)
                .HasForeignKey(r => r.BusinessID)
                .OnDelete(DeleteBehavior.Restrict); // Or whatever behavior you want

            // Configure LocationService entity
            modelBuilder.Entity<LocationService>()
                .ToTable("LocationService");

            // Configure Appointment entity
            modelBuilder.Entity<Appointment>()
                .ToTable("Appointment")
                .HasOne(a => a.Business)
                .WithMany()
                .HasForeignKey(a => a.BusinessID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Consumer)
                .WithMany()
                .HasForeignKey(a => a.ConsumerID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Review entity
            modelBuilder.Entity<Review>()
                .ToTable("Review")
                .HasOne(r => r.Business)
                .WithMany()
                .HasForeignKey(r => r.BusinessID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Consumer)
                .WithMany()
                .HasForeignKey(r => r.ConsumerID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Report entity
            modelBuilder.Entity<Report>()
                .ToTable("Report")
                .HasOne(r => r.Business)
                .WithMany()
                .HasForeignKey(r => r.BusinessID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Service entity
            modelBuilder.Entity<Service>()
                .ToTable("Service")
                .HasOne(s => s.Business)
                .WithMany()
                .HasForeignKey(s => s.BusinessID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure BookingHistory entity
            modelBuilder.Entity<BookingHistory>()
                .ToTable("BookingHistory")
                .HasOne(bh => bh.Appointment)
                .WithMany()
                .HasForeignKey(bh => bh.AppointmentID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Payment entity
            modelBuilder.Entity<PaymentEmpowerU>()
                .ToTable("Payment")
                .HasOne(p => p.Consumer)
                .WithMany()
                .HasForeignKey(p => p.ConsumerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentEmpowerU>()
                .HasOne(p => p.Business)
                .WithMany()
                .HasForeignKey(p => p.BusinessID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Subscription entity
            modelBuilder.Entity<Subscription>()
                .ToTable("Subscription")
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Discount entity
            modelBuilder.Entity<Discount>()
                .ToTable("Discount")
                .HasOne(d => d.Business)
                .WithMany()
                .HasForeignKey(d => d.BusinessID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Message entity
            modelBuilder.Entity<Message>()
                .ToTable("Message")
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Notification entity
            modelBuilder.Entity<Notification>()
                .ToTable("Notification")
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
