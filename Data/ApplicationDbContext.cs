using BookingHomestay.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingHomestay.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<BookingService> BookingServices { get; set; }
        public DbSet<Gallery> Gallery { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<HotelFacility> HotelFacilities { get; set; }
        public DbSet<RoomAmenity> RoomAmenities { get; set; }
        public DbSet<Cancellation> Cancellations { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<User> Users { get; set; } // Thêm dòng này

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình khóa chính cho các bảng
            modelBuilder.Entity<Hotel>().HasKey(h => h.HotelId);
            modelBuilder.Entity<Promotion>().HasKey(p => p.PromotionId);
            modelBuilder.Entity<Customer>().HasKey(c => c.CustomerId);
            modelBuilder.Entity<Room>().HasKey(r => r.RoomId);
            modelBuilder.Entity<Service>().HasKey(s => s.ServiceId);
            modelBuilder.Entity<Booking>().HasKey(b => b.BookingId);
            modelBuilder.Entity<Payment>().HasKey(p => p.PaymentId);
            modelBuilder.Entity<Review>().HasKey(r => r.ReviewId);
            modelBuilder.Entity<Feedback>().HasKey(f => f.FeedbackId);
            modelBuilder.Entity<BookingService>().HasKey(bs => bs.BookingServiceId);
            modelBuilder.Entity<Gallery>().HasKey(g => g.GalleryId);
            modelBuilder.Entity<Staff>().HasKey(s => s.StaffId);
            modelBuilder.Entity<Invoice>().HasKey(i => i.InvoiceId);
            modelBuilder.Entity<HotelFacility>().HasKey(f => f.FacilityId);
            modelBuilder.Entity<RoomAmenity>().HasKey(a => a.AmenityId);
            modelBuilder.Entity<Cancellation>().HasKey(c => c.CancellationId);
            modelBuilder.Entity<Location>().HasKey(l => l.LocationId);
            modelBuilder.Entity<PaymentTransaction>().HasKey(pt => pt.TransactionId);
            modelBuilder.Entity<User>().HasKey(u => u.UserId); // Thêm dòng này

            // Cấu hình mối quan hệ
            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(pt => pt.Payment)
                .WithMany(p => p.PaymentTransactions)
                .HasForeignKey(pt => pt.PaymentId);

            modelBuilder.Entity<Hotel>()
                .HasOne(h => h.Location)
                .WithMany()
                .HasForeignKey(h => h.LocationId);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.Location)
                .WithMany()
                .HasForeignKey(r => r.LocationId);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.Hotel)
                .WithMany(h => h.Rooms)
                .HasForeignKey(r => r.HotelId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Customer)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CustomerId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Promotion)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.PromotionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Room)
                .WithMany(r => r.Reviews)
                .HasForeignKey(r => r.RoomId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Customer)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CustomerId);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Customer)
                .WithMany(c => c.Feedbacks)
                .HasForeignKey(f => f.CustomerId);

            modelBuilder.Entity<BookingService>()
                .HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingServices)
                .HasForeignKey(bs => bs.BookingId);

            modelBuilder.Entity<BookingService>()
                .HasOne(bs => bs.Service)
                .WithMany(s => s.BookingServices)
                .HasForeignKey(bs => bs.ServiceId);

            modelBuilder.Entity<Gallery>()
                .HasOne(g => g.Room)
                .WithMany(r => r.Gallery)
                .HasForeignKey(g => g.RoomId);

            modelBuilder.Entity<Gallery>()
                .HasOne(g => g.Hotel)
                .WithMany(h => h.Gallery)
                .HasForeignKey(g => g.HotelId);

            modelBuilder.Entity<Staff>()
                .HasOne(s => s.Hotel)
                .WithMany(h => h.Staffs)
                .HasForeignKey(s => s.HotelId);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Booking)
                .WithMany(b => b.Invoices)
                .HasForeignKey(i => i.BookingId);

            modelBuilder.Entity<HotelFacility>()
                .HasOne(f => f.Hotel)
                .WithMany(h => h.HotelFacilities)
                .HasForeignKey(f => f.HotelId);

            modelBuilder.Entity<RoomAmenity>()
                .HasOne(a => a.Room)
                .WithMany(r => r.RoomAmenities)
                .HasForeignKey(a => a.RoomId);

            modelBuilder.Entity<Cancellation>()
                .HasOne(c => c.Booking)
                .WithMany(b => b.Cancellations)
                .HasForeignKey(c => c.BookingId);
        }
    }
}