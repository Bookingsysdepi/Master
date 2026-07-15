using CineBook.Dtos.Bookings;

namespace CineBook.Dtos.SnackOrders
{
    public class SnackOrderFormDto
    {
        public int BookingId { get; set; }

        public string MovieTitle { get; set; } = string.Empty;

        public string HallName { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }

        public List<BookedSeatDto> Seats { get; set; } = new List<BookedSeatDto>();

        public List<AvailableSnackDto> Snacks { get; set; } = new List<AvailableSnackDto>();

        public CreateSnackOrderDto Order { get; set; } = new CreateSnackOrderDto();
    }
}
