using System.ComponentModel.DataAnnotations;

namespace CineBook.Models
{
    public enum SeatType
    {
        Normal,
        VIP
    }

    public class Seat
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(5)]
        public string RowLabel { get; set; } = string.Empty;

        public int SeatNumber { get; set; }

        public SeatType SeatType { get; set; }

        public int HallId { get; set; }
        public Hall Hall { get; set; } = null!;

        public List<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
    }
}
