using CineBook.Models;

namespace CineBook.ViewModels
{
    public class SeatSelectionViewModel
    {
        public Showtime Showtime { get; set; } = null!;
        public Dictionary<string, List<Seat>> SeatsByRow { get; set; } = new Dictionary<string, List<Seat>>();
        public List<int> TakenSeatIds { get; set; } = new List<int>();
    }
}
