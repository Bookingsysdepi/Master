namespace CineBook.Models
{
    public class SnackOrderSeat
    {
        public int Id { get; set; }

        public int SnackOrderId { get; set; }
        public SnackOrder SnackOrder { get; set; } = null!;

        public int SeatId { get; set; }
        public Seat Seat { get; set; } = null!;
    }
}
