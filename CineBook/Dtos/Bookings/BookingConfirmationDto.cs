using CineBook.Models;

namespace CineBook.Dtos.Bookings;

public class BookingConfirmationDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;

    public DateTime BookingDate { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
    public int ShowtimeId { get; set; }
    public DateTime StartTime { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public string HallName { get; set; } = string.Empty;
    public List<BookedSeatDto> Seats { get; set; } = new();
}
