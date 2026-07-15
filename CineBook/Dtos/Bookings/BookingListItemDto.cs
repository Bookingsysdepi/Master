using CineBook.Models;

namespace CineBook.Dtos.Bookings;

public class BookingListItemDto
{
    public int Id { get; set; }
    public DateTime BookingDate { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public string HallName { get; set; } = string.Empty;
    public bool CanOrderSnacks { get; set; }
    public List<BookedSeatDto> Seats { get; set; } = new();
}
