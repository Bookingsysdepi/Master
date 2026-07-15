using CineBook.Data;
using CineBook.Dtos.Bookings;
using CineBook.Dtos.SnackOrders;
using CineBook.Models;
using CineBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineBook.Services.Implementations
{
    public class SnackOrderService : ISnackOrderService
    {
        private readonly ApplicationDbContext _context;

        public SnackOrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SnackOrderFormDto?> GetSnackOrderFormAsync(int bookingId, string userId)
        {
            var booking = await _context.Bookings
                .AsNoTracking()
                .Where(b => b.Id == bookingId && b.UserId == userId)
                .Select(b => new
                {
                    b.Id,
                    b.Status,
                    b.Showtime.StartTime,
                    b.Showtime.Movie.DurationMinutes,
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

            if (booking == null || booking.Status != BookingStatus.Confirmed)
            {
                return null;
            }

            if (booking.StartTime.AddMinutes(booking.DurationMinutes) <= DateTime.Now)
            {
                return null;
            }

            var snacks = await _context.Snacks
                .AsNoTracking()
                .Where(s => s.IsAvailable)
                .OrderBy(s => s.Category)
                .ThenBy(s => s.Name)
                .Select(s => new AvailableSnackDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Category = s.Category.ToString(),
                    Price = s.Price
                })
                .ToListAsync();

            return new SnackOrderFormDto
            {
                BookingId = booking.Id,
                MovieTitle = booking.MovieTitle,
                HallName = booking.HallName,
                StartTime = booking.StartTime,
                Seats = booking.Seats,
                Snacks = snacks,
                Order = new CreateSnackOrderDto
                {
                    BookingId = booking.Id,
                    UserId = userId
                }
            };
        }

        public async Task<CreateSnackOrderResultDto> CreateSnackOrderAsync(CreateSnackOrderDto dto)
        {
            var selectedSeatIds = dto.SeatIds.Distinct().ToList();
            if (selectedSeatIds.Count == 0)
            {
                return CreateResult(CreateSnackOrderResult.NoSeatsSelected);
            }

            if (dto.Items.Any(i => i.Quantity < 0 || i.Quantity > 20))
            {
                return CreateResult(CreateSnackOrderResult.InvalidQuantity);
            }

            var selectedItems = dto.Items
                .Where(i => i.Quantity > 0)
                .GroupBy(i => i.SnackId)
                .Select(g => new CreateSnackOrderItemDto
                {
                    SnackId = g.Key,
                    Quantity = g.Sum(i => i.Quantity)
                })
                .ToList();

            if (selectedItems.Count == 0)
            {
                return CreateResult(CreateSnackOrderResult.NoItemsSelected);
            }

            var booking = await _context.Bookings
                .AsNoTracking()
                .Where(b => b.Id == dto.BookingId && b.UserId == dto.UserId)
                .Select(b => new
                {
                    b.Id,
                    b.UserId,
                    b.Status,
                    b.Showtime.StartTime,
                    b.Showtime.Movie.DurationMinutes,
                    SeatIds = b.BookingSeats.Select(bs => bs.SeatId).ToList()
                })
                .FirstOrDefaultAsync();

            if (booking == null)
            {
                return CreateResult(CreateSnackOrderResult.BookingNotFound);
            }

            if (booking.Status != BookingStatus.Confirmed)
            {
                return CreateResult(CreateSnackOrderResult.BookingNotConfirmed);
            }

            if (booking.StartTime.AddMinutes(booking.DurationMinutes) <= DateTime.Now)
            {
                return CreateResult(CreateSnackOrderResult.ShowtimeEnded);
            }

            if (selectedSeatIds.Any(id => !booking.SeatIds.Contains(id)))
            {
                return CreateResult(CreateSnackOrderResult.InvalidSeats);
            }

            var snackIds = selectedItems.Select(i => i.SnackId).ToList();
            var snacks = await _context.Snacks
                .AsNoTracking()
                .Where(s => snackIds.Contains(s.Id) && s.IsAvailable)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Price
                })
                .ToListAsync();

            if (snacks.Count != snackIds.Distinct().Count())
            {
                return CreateResult(CreateSnackOrderResult.InvalidSnacks);
            }

            var orderItems = selectedItems
                .Select(item =>
                {
                    var snack = snacks.First(s => s.Id == item.SnackId);
                    return new SnackOrderItem
                    {
                        SnackId = snack.Id,
                        ItemNameSnapshot = snack.Name,
                        Quantity = item.Quantity,
                        UnitPrice = snack.Price
                    };
                })
                .ToList();

            var order = new SnackOrder
            {
                UserId = dto.UserId,
                BookingId = dto.BookingId,
                Status = SnackOrderStatus.Pending,
                TotalPrice = orderItems.Sum(i => i.UnitPrice * i.Quantity),
                CreatedAtUtc = DateTime.UtcNow,
                Items = orderItems,
                Seats = selectedSeatIds
                    .Select(id => new SnackOrderSeat
                    {
                        SeatId = id
                    })
                    .ToList()
            };

            _context.SnackOrders.Add(order);
            await _context.SaveChangesAsync();

            return new CreateSnackOrderResultDto
            {
                Result = CreateSnackOrderResult.Created,
                SnackOrderId = order.Id
            };
        }

        public async Task<SnackOrdersDto> GetCanteenOrdersAsync()
        {
            var orders = await _context.SnackOrders
                .AsNoTracking()
                .OrderBy(o => o.Status == SnackOrderStatus.Delivered)
                .ThenBy(o => o.Status == SnackOrderStatus.Cancelled)
                .ThenBy(o => o.CreatedAtUtc)
                .Select(o => new SnackOrderListDto
                {
                    Id = o.Id,
                    CustomerName = o.User.FullName,
                    MovieTitle = o.Booking.Showtime.Movie.Title,
                    HallName = o.Booking.Showtime.Hall.Name,
                    StartTime = o.Booking.Showtime.StartTime,
                    Status = o.Status,
                    TotalPrice = o.TotalPrice,
                    CreatedAtUtc = o.CreatedAtUtc,
                    Seats = o.Seats
                        .OrderBy(s => s.Seat.RowLabel)
                        .ThenBy(s => s.Seat.SeatNumber)
                        .Select(s => s.Seat.RowLabel + s.Seat.SeatNumber)
                        .ToList(),
                    Items = o.Items
                        .OrderBy(i => i.ItemNameSnapshot)
                        .Select(i => new SnackOrderItemListDto
                        {
                            Name = i.ItemNameSnapshot,
                            Quantity = i.Quantity,
                            UnitPrice = i.UnitPrice
                        })
                        .ToList()
                })
                .ToListAsync();

            return new SnackOrdersDto
            {
                Orders = orders
            };
        }

        public async Task<UpdateSnackOrderStatusResultDto> UpdateSnackOrderStatusAsync(UpdateSnackOrderStatusDto dto)
        {
            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.ApplicationUserId == dto.EmployeeUserId && e.IsActive);

            if (employee == null)
            {
                return StatusResult(UpdateSnackOrderStatusResult.EmployeeNotFound);
            }

            var order = await _context.SnackOrders.FindAsync(dto.Id);
            if (order == null)
            {
                return StatusResult(UpdateSnackOrderStatusResult.NotFound);
            }

            if (!IsValidStatusTransition(order.Status, dto.Status))
            {
                return StatusResult(UpdateSnackOrderStatusResult.InvalidTransition);
            }

            order.Status = dto.Status;
            order.AssignedEmployeeId ??= employee.Id;

            if (dto.Status == SnackOrderStatus.Delivered)
            {
                order.DeliveredAtUtc = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return StatusResult(UpdateSnackOrderStatusResult.Updated);
        }

        private static bool IsValidStatusTransition(SnackOrderStatus current, SnackOrderStatus next)
        {
            if (current == SnackOrderStatus.Delivered || current == SnackOrderStatus.Cancelled)
            {
                return false;
            }

            return next switch
            {
                SnackOrderStatus.Preparing => current == SnackOrderStatus.Pending,
                SnackOrderStatus.OutForDelivery => current == SnackOrderStatus.Preparing,
                SnackOrderStatus.Delivered => current == SnackOrderStatus.OutForDelivery,
                SnackOrderStatus.Cancelled => current == SnackOrderStatus.Pending,
                _ => false
            };
        }

        private static CreateSnackOrderResultDto CreateResult(CreateSnackOrderResult result)
        {
            return new CreateSnackOrderResultDto
            {
                Result = result
            };
        }

        private static UpdateSnackOrderStatusResultDto StatusResult(UpdateSnackOrderStatusResult result)
        {
            return new UpdateSnackOrderStatusResultDto
            {
                Result = result
            };
        }
    }
}
