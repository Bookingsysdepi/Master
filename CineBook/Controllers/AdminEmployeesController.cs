using CineBook.Dtos.Employees;
using CineBook.Models;
using CineBook.Services.Interfaces;
using CineBook.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineBook.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Employees")]
    public class AdminEmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public AdminEmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(SearchEmployeesDto search)
        {
            var employeesDto = await _employeeService.GetEmployeesAsync(search);

            var viewModel = new AdminEmployeeIndexViewModel
            {
                Search = search,
                Employees = employeesDto.Employees
            };

            return View("~/Views/Admin/Employees/Index.cshtml", viewModel);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Employees/Create.cshtml", new CreateEmployeeDto
            {
                HireDate = DateTime.Today,
                JobTitle = "Canteen Employee",
                Department = EmployeeDepartment.Canteen
            });
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/Employees/Create.cshtml", dto);
            }

            var result = await _employeeService.CreateEmployeeAsync(dto);

            if (!result.Succeeded)
            {
                AddServiceErrors(result);
                return View("~/Views/Admin/Employees/Create.cshtml", dto);
            }

            TempData["AdminEmployeeMessage"] = "Employee added successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _employeeService.GetEmployeeForEditAsync(new GetEmployeeForEditDto
            {
                Id = id
            });

            if (dto == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Employees/Edit.cshtml", dto);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateEmployeeDto dto)
        {
            dto.Id = id;

            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/Employees/Edit.cshtml", dto);
            }

            var result = await _employeeService.UpdateEmployeeAsync(dto);

            if (result.NotFound)
            {
                return NotFound();
            }

            if (!result.Succeeded)
            {
                AddServiceErrors(result);
                return View("~/Views/Admin/Employees/Edit.cshtml", dto);
            }

            TempData["AdminEmployeeMessage"] = "Employee updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Deactivate/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _employeeService.DeactivateEmployeeAsync(new DeactivateEmployeeDto
            {
                Id = id
            });

            if (result.NotFound)
            {
                return NotFound();
            }

            if (!result.Succeeded)
            {
                TempData["AdminEmployeeError"] = string.Join(" ", result.Errors);
                return RedirectToAction(nameof(Index));
            }

            TempData["AdminEmployeeMessage"] = "Employee deactivated successfully.";
            return RedirectToAction(nameof(Index));
        }

        private void AddServiceErrors(EmployeeOperationResultDto result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }
    }
}
