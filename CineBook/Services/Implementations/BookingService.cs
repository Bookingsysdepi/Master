using CineBook.Data;
using CineBook.Dtos.Bookings;
using CineBook.Models;
using CineBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineBook.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CreateBookingResultDto> CreateBookingAsync(
            CreateBookingDto dto)
        {
            if (dto.SeatIds == null || dto.SeatIds.Count == 0)
            {
                return new CreateBookingResultDto
                {
                    Result = CreateBookingResult.NoSeatsSelected
                };
            }

            var seatIds = dto.SeatIds
                .Distinct()
                .ToList();

            var showtime = await _context.Showtimes
                .AsNoTracking()
                .Where(s => s.Id == dto.ShowtimeId)
                .Select(s => new
                {
                    s.Id,
                    s.HallId,
                    s.NormalPrice,
                    s.VipPrice
                })
                .FirstOrDefaultAsync();

            if (showtime == null)
            {
                return new CreateBookingResultDto
                {
                    Result = CreateBookingResult.ShowtimeNotFound
                };
            }

            var seats = await _context.Seats
                .AsNoTracking()
                .Where(s =>
                    seatIds.Contains(s.Id) &&
                    s.HallId == showtime.HallId)
                .Select(s => new
                {
                    s.Id,
                    s.SeatType
                })
                .ToListAsync();

            if (seats.Count != seatIds.Count)
            {
                return new CreateBookingResultDto
                {
                    Result = CreateBookingResult.InvalidSeats
                };
            }

            var seatsAlreadyTaken = await _context.BookingSeats
                .AsNoTracking()
                .AnyAsync(bs =>
                    bs.Booking.ShowtimeId == dto.ShowtimeId &&
                    bs.Booking.Status == BookingStatus.Confirmed &&
                    seatIds.Contains(bs.SeatId));

            if (seatsAlreadyTaken)
            {
                return new CreateBookingResultDto
                {
                    Result = CreateBookingResult.SeatsAlreadyTaken
                };
            }

            var totalPrice = seats.Sum(seat =>
                seat.SeatType == SeatType.VIP
                    ? showtime.VipPrice
                    : showtime.NormalPrice);

            var booking = new Booking
            {
                UserId = dto.UserId,
                ShowtimeId = dto.ShowtimeId,
                BookingDate = DateTime.Now,
                TotalPrice = totalPrice,
                Status = BookingStatus.Confirmed,
                BookingSeats = seats
                    .Select(seat => new BookingSeat
                    {
                        SeatId = seat.Id
                    })
                    .ToList()
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return new CreateBookingResultDto
            {
                Result = CreateBookingResult.Created,
                BookingId = booking.Id
            };
        }

        public async Task<BookingConfirmationDto?>
            GetBookingConfirmationAsync(
                int bookingId,
                string userId)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(b =>
                    b.Id == bookingId &&
                    b.UserId == userId)
                .Select(b => new BookingConfirmationDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    BookingDate = b.BookingDate,
                    TotalPrice = b.TotalPrice,
                    Status = b.Status,
                    ShowtimeId = b.ShowtimeId,
                    StartTime = b.Showtime.StartTime,
                    MovieTitle = b.Showtime.Movie.Title,
                    HallName = b.Showtime.Hall.Name,

                    Seats = b.BookingSeats
                        .OrderBy(bs => bs.Seat.RowLabel)
                        .ThenBy(bs => bs.Seat.SeatNumber)
                        .Select(bs => new BookedSeatDto
                        {
                            Id = bs.Seat.Id,
                            RowLabel = bs.Seat.RowLabel,
                            SeatNumber = bs.Seat.SeatNumber,
                            SeatType = bs.Seat.SeatType
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<BookingListItemDto>>
            GetUserBookingsAsync(string userId)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .Select(b => new BookingListItemDto
                {
                    Id = b.Id,
                    BookingDate = b.BookingDate,
                    TotalPrice = b.TotalPrice,
                    Status = b.Status,
                    StartTime = b.Showtime.StartTime,
                    MovieTitle = b.Showtime.Movie.Title,
                    HallName = b.Showtime.Hall.Name,
                    CanOrderSnacks = b.Status == BookingStatus.Confirmed &&
                        b.Showtime.StartTime.AddMinutes(b.Showtime.Movie.DurationMinutes) > DateTime.Now,

                    Seats = b.BookingSeats
                        .OrderBy(bs => bs.Seat.RowLabel)
                        .ThenBy(bs => bs.Seat.SeatNumber)
                        .Select(bs => new BookedSeatDto
                        {
                            Id = bs.Seat.Id,
                            RowLabel = bs.Seat.RowLabel,
                            SeatNumber = bs.Seat.SeatNumber,
                            SeatType = bs.Seat.SeatType
                        })
                        .ToList()
                })
                .ToListAsync();
        }
    }
}
