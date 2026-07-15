using CineBook.Data;
using CineBook.Dtos.Showtimes;
using CineBook.Models;
using CineBook.Services.Interfaces;
using CineBook.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CineBook.Services.Implementations
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly ApplicationDbContext _context;

        public ShowtimeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CreateShowtimeFormDto?> GetCreateShowtimeFormAsync(CreateShowtimeFormRequestDto dto)
        {
            var movie = await _context.Movies
                .AsNoTracking()
                .Where(m => m.Id == dto.MovieId)
                .Select(m => new
                {
                    m.Id,
                    m.Title
                })
                .FirstOrDefaultAsync();

            if (movie == null)
            {
                return null;
            }

            var halls = await _context.Halls
                .AsNoTracking()
                .OrderBy(h => h.Name)
                .Select(h => new HallOptionDto
                {
                    Id = h.Id,
                    Name = h.Name
                })
                .ToListAsync();

            var showtime = dto.Showtime ?? new CreateShowtimeDto
            {
                MovieId = movie.Id,
                StartTime = DateTime.Today.AddDays(1).AddHours(18),
                NormalPrice = 60,
                VipPrice = 100
            };

            return new CreateShowtimeFormDto
            {
                MovieId = movie.Id,
                MovieTitle = movie.Title,
                MinimumStartTime = DateTime.Now,
                HallId = showtime.HallId,
                StartTime = showtime.StartTime,
                NormalPrice = showtime.NormalPrice,
                VipPrice = showtime.VipPrice,
                Halls = halls
            };
        }

        public async Task<CreateShowtimeResultDto> CreateShowtimeAsync(CreateShowtimeDto dto)
        {
            var movie = await _context.Movies
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == dto.MovieId);

            if (movie == null)
            {
                return new CreateShowtimeResultDto
                {
                    Result = CreateShowtimeResult.MovieNotFound
                };
            }

            if (dto.StartTime <= DateTime.Now)
            {
                return new CreateShowtimeResultDto
                {
                    Result = CreateShowtimeResult.StartTimeNotUpcoming
                };
            }

            var hallExists = await _context.Halls.AnyAsync(h => h.Id == dto.HallId);

            if (!hallExists)
            {
                return new CreateShowtimeResultDto
                {
                    Result = CreateShowtimeResult.HallNotFound
                };
            }

            if (await IsHallUnavailableAsync(dto, movie.DurationMinutes))
            {
                return new CreateShowtimeResultDto
                {
                    Result = CreateShowtimeResult.HallUnavailable
                };
            }

            var showtime = new Showtime
            {
                MovieId = dto.MovieId,
                HallId = dto.HallId,
                StartTime = dto.StartTime,
                NormalPrice = dto.NormalPrice,
                VipPrice = dto.VipPrice
            };

            _context.Showtimes.Add(showtime);
            await _context.SaveChangesAsync();

            return new CreateShowtimeResultDto
            {
                Result = CreateShowtimeResult.Created
            };
        }

        private async Task<bool> IsHallUnavailableAsync(CreateShowtimeDto dto, int movieDurationMinutes)
        {
            var newStart = dto.StartTime;
            var newEnd = newStart.AddMinutes(movieDurationMinutes);

            var existingShowtimes = await _context.Showtimes
                .AsNoTracking()
                .Include(s => s.Movie)
                .Where(s => s.HallId == dto.HallId && s.StartTime < newEnd)
                .ToListAsync();

            return existingShowtimes.Any(s =>
            {
                var existingEnd = s.StartTime.AddMinutes(s.Movie.DurationMinutes);
                return s.StartTime < newEnd && existingEnd > newStart;
            });
        }

        public async Task<SeatSelectionViewModel?> GetSeatSelectionAsync(
    int showtimeId)
        {
            var showtime = await _context.Showtimes
                .AsNoTracking()
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                .FirstOrDefaultAsync(s => s.Id == showtimeId);

            if (showtime == null)
            {
                return null;
            }

            var seats = await _context.Seats
                .AsNoTracking()
                .Where(s => s.HallId == showtime.HallId)
                .OrderBy(s => s.RowLabel)
                .ThenBy(s => s.SeatNumber)
                .ToListAsync();

            var takenSeatIds = await _context.BookingSeats
                .AsNoTracking()
                .Where(bs =>
                    bs.Booking.ShowtimeId == showtimeId &&
                    bs.Booking.Status == BookingStatus.Confirmed)
                .Select(bs => bs.SeatId)
                .ToListAsync();

            return new SeatSelectionViewModel
            {
                Showtime = showtime,

                SeatsByRow = seats
                    .GroupBy(s => s.RowLabel)
                    .OrderBy(group => group.Key)
                    .ToDictionary(
                        group => group.Key,
                        group => group.ToList()),

                TakenSeatIds = takenSeatIds
            };
        }
    }
}
