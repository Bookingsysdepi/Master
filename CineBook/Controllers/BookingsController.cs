using System.Security.Claims;
using CineBook.Dtos.Bookings;
using CineBook.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineBook.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int showtimeId,
            List<int> seatIds)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Challenge();
            }

            var result = await _bookingService.CreateBookingAsync(
                new CreateBookingDto
                {
                    UserId = userId,
                    ShowtimeId = showtimeId,
                    SeatIds = seatIds ?? new List<int>()
                });

            switch (result.Result)
            {
                case CreateBookingResult.NoSeatsSelected:
                    TempData["SeatError"] =
                        "Please select at least one seat.";

                    return RedirectToAction(
                        "Select",
                        "Showtimes",
                        new { id = showtimeId });

                case CreateBookingResult.ShowtimeNotFound:
                    return NotFound();

                case CreateBookingResult.InvalidSeats:
                    TempData["SeatError"] =
                        "One or more selected seats are not valid for this showtime.";

                    return RedirectToAction(
                        "Select",
                        "Showtimes",
                        new { id = showtimeId });

                case CreateBookingResult.SeatsAlreadyTaken:
                    TempData["SeatError"] =
                        "One or more selected seats were just booked. Please choose different seats.";

                    return RedirectToAction(
                        "Select",
                        "Showtimes",
                        new { id = showtimeId });

                case CreateBookingResult.Created:
                    return RedirectToAction(
                        nameof(Confirmation),
                        new { id = result.BookingId });

                default:
                    return BadRequest();
            }
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Challenge();
            }

            var booking =
                await _bookingService.GetBookingConfirmationAsync(
                    id,
                    userId);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        public async Task<IActionResult> MyBookings()
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Challenge();
            }

            var bookings =
                await _bookingService.GetUserBookingsAsync(userId);

            return View(bookings);
        }
    }
}