using System.ComponentModel.DataAnnotations;

namespace CineBook.Models
{
    public class Hall
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public int RowsCount { get; set; }

        public int SeatsPerRow { get; set; }

        public List<Seat> Seats { get; set; } = new List<Seat>();
        public List<Showtime> Showtimes { get; set; } = new List<Showtime>();
    }
}
