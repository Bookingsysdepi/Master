using CineBook.Models;

namespace CineBook.Dtos.Bookings;

public class BookedSeatDto
{
    public int Id { get; set; }
    public string RowLabel { get; set; } = string.Empty;
    public int SeatNumber { get; set; }
    public SeatType SeatType { get; set; }
}
