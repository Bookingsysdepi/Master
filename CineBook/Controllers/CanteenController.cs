using System.Security.Claims;
using CineBook.Dtos.SnackOrders;
using CineBook.Dtos.Snacks;
using CineBook.Models;
using CineBook.Services.Interfaces;
using CineBook.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineBook.Controllers
{
    [Authorize(Roles = "CanteenEmployee")]
    [Route("Canteen")]
    public class CanteenController : Controller
    {
        private readonly ISnackOrderService _snackOrderService;
        private readonly ISnackService _snackService;

        public CanteenController(ISnackOrderService snackOrderService, ISnackService snackService)
        {
            _snackOrderService = snackOrderService;
            _snackService = snackService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var ordersDto = await _snackOrderService.GetCanteenOrdersAsync();
            var snacksDto = await _snackService.GetSnacksAsync(new SearchSnacksDto());

            var viewModel = new CanteenDashboardViewModel
            {
                Orders = ordersDto.Orders,
                Snacks = snacksDto.Snacks
            };

            return View("~/Views/Canteen/Index.cshtml", viewModel);
        }

        [HttpPost("Orders/{id}/Status")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, SnackOrderStatus status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Challenge();
            }

            var result = await _snackOrderService.UpdateSnackOrderStatusAsync(new UpdateSnackOrderStatusDto
            {
                Id = id,
                EmployeeUserId = userId,
                Status = status
            });

            TempData[result.Result == UpdateSnackOrderStatusResult.Updated ? "CanteenMessage" : "CanteenError"] =
                result.Result == UpdateSnackOrderStatusResult.Updated
                    ? "Order status updated."
                    : "Could not update order status.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Snacks/{id}/ToggleAvailability")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleSnackAvailability(int id)
        {
            var result = await _snackService.ToggleSnackAvailabilityAsync(new ToggleSnackAvailabilityDto
            {
                Id = id
            });

            TempData[result.Succeeded ? "CanteenMessage" : "CanteenError"] =
                result.Succeeded ? "Snack availability updated." : "Could not update snack availability.";

            return RedirectToAction(nameof(Index));
        }
    }
}
