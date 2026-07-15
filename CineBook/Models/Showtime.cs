namespace CineBook.Models
{
    public class Showtime
    {
        public int Id { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        public int HallId { get; set; }
        public Hall Hall { get; set; } = null!;

        public DateTime StartTime { get; set; }

        public decimal NormalPrice { get; set; }

        public decimal VipPrice { get; set; }

        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
