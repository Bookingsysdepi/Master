using System.Security.Claims;
using CineBook.Dtos.SnackOrders;
using CineBook.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineBook.Controllers
{
    [Authorize]
    [Route("Bookings/{bookingId}/Snacks")]
    public class SnackOrdersController : Controller
    {
        private readonly ISnackOrderService _snackOrderService;

        public SnackOrdersController(ISnackOrderService snackOrderService)
        {
            _snackOrderService = snackOrderService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Create(int bookingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Challenge();
            }

            var form = await _snackOrderService.GetSnackOrderFormAsync(bookingId, userId);
            if (form == null)
            {
                return NotFound();
            }

            return View("~/Views/SnackOrders/Create.cshtml", form);
        }

        [HttpPost("")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int bookingId, CreateSnackOrderDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Challenge();
            }

            dto.BookingId = bookingId;
            dto.UserId = userId;

            var result = await _snackOrderService.CreateSnackOrderAsync(dto);
            if (result.Result == CreateSnackOrderResult.Created)
            {
                TempData["SnackOrderMessage"] = "Snack order sent to the canteen.";
                return RedirectToAction("MyBookings", "Bookings");
            }

            var form = await _snackOrderService.GetSnackOrderFormAsync(bookingId, userId);
            if (form == null)
            {
                return NotFound();
            }

            AddSnackOrderError(result.Result);
            form.Order = dto;

            return View("~/Views/SnackOrders/Create.cshtml", form);
        }

        private void AddSnackOrderError(CreateSnackOrderResult result)
        {
            var message = result switch
            {
                CreateSnackOrderResult.BookingNotConfirmed => "This booking is not confirmed.",
                CreateSnackOrderResult.ShowtimeEnded => "Snack ordering is closed for this showtime.",
                CreateSnackOrderResult.NoSeatsSelected => "Select at least one delivery seat.",
                CreateSnackOrderResult.InvalidSeats => "Selected seats are not part of this booking.",
                CreateSnackOrderResult.NoItemsSelected => "Choose at least one snack.",
                CreateSnackOrderResult.InvalidQuantity => "Snack quantities must be between 0 and 20.",
                CreateSnackOrderResult.InvalidSnacks => "One or more selected snacks are unavailable.",
                _ => "Could not create snack order."
            };

            ModelState.AddModelError(string.Empty, message);
        }
    }
}
