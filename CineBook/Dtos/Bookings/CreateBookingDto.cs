namespace CineBook.Dtos.Bookings;

public class CreateBookingDto
{
    public string UserId { get; set; } = string.Empty;
    public int ShowtimeId { get; set; }
    public List<int> SeatIds { get; set; } = new();
}
