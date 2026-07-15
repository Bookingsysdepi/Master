using CineBook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CineBook.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Hall> Halls { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingSeat> BookingSeats { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Snack> Snacks { get; set; }
        public DbSet<SnackOrder> SnackOrders { get; set; }
        public DbSet<SnackOrderItem> SnackOrderItems { get; set; }
        public DbSet<SnackOrderSeat> SnackOrderSeats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Showtime>()
                .Property(s => s.NormalPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Showtime>()
                .Property(s => s.VipPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Employee>()
                .Property(e => e.Salary)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.ApplicationUserId)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.NationalId)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.ApplicationUser)
                .WithOne()
                .HasForeignKey<Employee>(e => e.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Snack>()
                .Property(s => s.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Snack>()
                .HasIndex(s => s.Name)
                .IsUnique();

            modelBuilder.Entity<SnackOrder>()
                .Property(o => o.TotalPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<SnackOrder>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SnackOrder>()
                .HasOne(o => o.AssignedEmployee)
                .WithMany()
                .HasForeignKey(o => o.AssignedEmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<SnackOrderItem>()
                .Property(i => i.UnitPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<SnackOrderItem>()
                .HasOne(i => i.Snack)
                .WithMany()
                .HasForeignKey(i => i.SnackId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SnackOrderSeat>()
                .HasOne(s => s.Seat)
                .WithMany()
                .HasForeignKey(s => s.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookingSeat>()
                .HasOne(bs => bs.Seat)
                .WithMany(s => s.BookingSeats)
                .HasForeignKey(bs => bs.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookingSeat>()
                .HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingSeats)
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
