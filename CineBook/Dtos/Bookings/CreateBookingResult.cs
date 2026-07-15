namespace CineBook.Dtos.Bookings;

public enum CreateBookingResult
{
    Created,
    ShowtimeNotFound,
    NoSeatsSelected,
    InvalidSeats,
    SeatsAlreadyTaken
}
