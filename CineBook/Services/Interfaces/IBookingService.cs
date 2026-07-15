using CineBook.Dtos.Bookings;

namespace CineBook.Services.Interfaces
{
    public interface IBookingService
    {




        Task<CreateBookingResultDto> CreateBookingAsync(
            CreateBookingDto dto);

        Task<BookingConfirmationDto?> GetBookingConfirmationAsync(
            int bookingId,
            string userId);

        Task<List<BookingListItemDto>> GetUserBookingsAsync(
            string userId);
    }
}
