using CineBook.Dtos.Snacks;
using CineBook.Models;
using CineBook.Services.Interfaces;
using CineBook.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineBook.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Snacks")]
    public class AdminSnacksController : Controller
    {
        private readonly ISnackService _snackService;

        public AdminSnacksController(ISnackService snackService)
        {
            _snackService = snackService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(SearchSnacksDto search)
        {
            var snacksDto = await _snackService.GetSnacksAsync(search);

            var viewModel = new AdminSnackIndexViewModel
            {
                Search = search,
                Snacks = snacksDto.Snacks
            };

            return View("~/Views/Admin/Snacks/Index.cshtml", viewModel);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Snacks/Create.cshtml", new CreateSnackDto
            {
                Category = SnackCategory.Snack,
                IsAvailable = true
            });
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSnackDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/Snacks/Create.cshtml", dto);
            }

            var result = await _snackService.CreateSnackAsync(dto);

            if (!result.Succeeded)
            {
                AddServiceErrors(result);
                return View("~/Views/Admin/Snacks/Create.cshtml", dto);
            }

            TempData["AdminSnackMessage"] = "Snack added successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _snackService.GetSnackForEditAsync(new GetSnackForEditDto
            {
                Id = id
            });

            if (dto == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Snacks/Edit.cshtml", dto);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateSnackDto dto)
        {
            dto.Id = id;

            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/Snacks/Edit.cshtml", dto);
            }

            var result = await _snackService.UpdateSnackAsync(dto);

            if (result.NotFound)
            {
                return NotFound();
            }

            if (!result.Succeeded)
            {
                AddServiceErrors(result);
                return View("~/Views/Admin/Snacks/Edit.cshtml", dto);
            }

            TempData["AdminSnackMessage"] = "Snack updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("ToggleAvailability/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAvailability(int id)
        {
            var result = await _snackService.ToggleSnackAvailabilityAsync(new ToggleSnackAvailabilityDto
            {
                Id = id
            });

            if (result.NotFound)
            {
                return NotFound();
            }

            if (!result.Succeeded)
            {
                TempData["AdminSnackError"] = string.Join(" ", result.Errors);
                return RedirectToAction(nameof(Index));
            }

            TempData["AdminSnackMessage"] = "Snack availability updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        private void AddServiceErrors(SnackOperationResultDto result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }
    }
}
