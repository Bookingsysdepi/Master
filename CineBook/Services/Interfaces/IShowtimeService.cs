using CineBook.Dtos.Showtimes;
using CineBook.ViewModels;

namespace CineBook.Services.Interfaces
{
    public interface IShowtimeService
    {
        Task<CreateShowtimeResultDto> CreateShowtimeAsync(CreateShowtimeDto dto);
        Task<CreateShowtimeFormDto?> GetCreateShowtimeFormAsync(CreateShowtimeFormRequestDto dto);

        Task<SeatSelectionViewModel?> GetSeatSelectionAsync(
            int showtimeId);
    }
}
