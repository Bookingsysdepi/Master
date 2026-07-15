namespace CineBook.Models
{
    public enum BookingStatus
    {
        Confirmed,
        Cancelled
    }

    public class Booking
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int ShowtimeId { get; set; }
        public Showtime Showtime { get; set; } = null!;

        public DateTime BookingDate { get; set; }

        public decimal TotalPrice { get; set; }

        public BookingStatus Status { get; set; }

        public List<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
    }
}
