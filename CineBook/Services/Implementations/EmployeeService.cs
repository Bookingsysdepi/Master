using CineBook.Data;
using CineBook.Dtos.Employees;
using CineBook.Models;
using CineBook.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CineBook.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private const string EmployeeRoleName = "CanteenEmployee";
        private static readonly Regex EgyptianNationalIdRegex = new Regex(@"^[23][0-9]{13}$", RegexOptions.Compiled);
        private static readonly Regex EgyptianMobileRegex = new Regex(@"^(01[0125][0-9]{8}|\+201[0125][0-9]{8})$", RegexOptions.Compiled);

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<EmployeesDto> GetEmployeesAsync(SearchEmployeesDto search)
        {
            var query = _context.Employees
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search.SearchTerm))
            {
                var searchTerm = search.SearchTerm.Trim();

                query = query.Where(e =>
                    e.FullName.Contains(searchTerm) ||
                    e.Email.Contains(searchTerm) ||
                    e.NationalId.Contains(searchTerm));
            }

            if (search.IsActive.HasValue)
            {
                query = query.Where(e => e.IsActive == search.IsActive.Value);
            }

            var employees = await query
                .OrderBy(e => e.FullName)
                .Select(e => new EmployeeListDto
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    Email = e.Email,
                    PhoneNumber = e.PhoneNumber,
                    NationalId = e.NationalId,
                    Salary = e.Salary,
                    HireDate = e.HireDate,
                    JobTitle = e.JobTitle,
                    Department = e.Department,
                    IsActive = e.IsActive
                })
                .ToListAsync();

            return new EmployeesDto
            {
                Employees = employees
            };
        }

        public async Task<EmployeeOperationResultDto> CreateEmployeeAsync(CreateEmployeeDto dto)
        {
            var email = dto.Email.Trim();
            var nationalId = dto.NationalId.Trim();
            var phoneNumber = dto.PhoneNumber?.Trim() ?? string.Empty;

            var validationErrors = await GetCreateValidationErrorsAsync(dto, email, nationalId, phoneNumber);
            if (validationErrors.Any())
            {
                return Failed(validationErrors);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var user = new ApplicationUser
            {
                FullName = dto.FullName.Trim(),
                Email = email,
                UserName = email,
                PhoneNumber = phoneNumber,
                EmailConfirmed = true
            };

            var createUserResult = await _userManager.CreateAsync(user, dto.TemporaryPassword);
            if (!createUserResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return Failed(createUserResult.Errors.Select(error => error.Description));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, EmployeeRoleName);
            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return Failed(roleResult.Errors.Select(error => error.Description));
            }

            var employee = new Employee
            {
                ApplicationUserId = user.Id,
                FullName = dto.FullName.Trim(),
                Email = email,
                PhoneNumber = phoneNumber,
                NationalId = nationalId,
                Salary = dto.Salary,
                HireDate = dto.HireDate,
                JobTitle = dto.JobTitle.Trim(),
                Department = dto.Department,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Succeeded();
        }

        public async Task<UpdateEmployeeDto?> GetEmployeeForEditAsync(GetEmployeeForEditDto dto)
        {
            return await _context.Employees
                .AsNoTracking()
                .Where(e => e.Id == dto.Id)
                .Select(e => new UpdateEmployeeDto
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    Email = e.Email,
                    PhoneNumber = e.PhoneNumber,
                    NationalId = e.NationalId,
                    Salary = e.Salary,
                    HireDate = e.HireDate,
                    JobTitle = e.JobTitle,
                    Department = e.Department,
                    IsActive = e.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<EmployeeOperationResultDto> UpdateEmployeeAsync(UpdateEmployeeDto dto)
        {
            var employee = await _context.Employees
                .Include(e => e.ApplicationUser)
                .FirstOrDefaultAsync(e => e.Id == dto.Id);

            if (employee == null)
            {
                return NotFound();
            }

            var email = dto.Email.Trim();
            var nationalId = dto.NationalId.Trim();
            var phoneNumber = dto.PhoneNumber?.Trim() ?? string.Empty;

            var validationErrors = await GetUpdateValidationErrorsAsync(dto, employee.ApplicationUserId, email, nationalId, phoneNumber);
            if (validationErrors.Any())
            {
                return Failed(validationErrors);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var user = employee.ApplicationUser;
            user.FullName = dto.FullName.Trim();

            var emailResult = await _userManager.SetEmailAsync(user, email);
            if (!emailResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return Failed(emailResult.Errors.Select(error => error.Description));
            }

            var userNameResult = await _userManager.SetUserNameAsync(user, email);
            if (!userNameResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return Failed(userNameResult.Errors.Select(error => error.Description));
            }

            var phoneResult = await _userManager.SetPhoneNumberAsync(user, phoneNumber);
            if (!phoneResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return Failed(phoneResult.Errors.Select(error => error.Description));
            }

            var updateUserResult = await _userManager.UpdateAsync(user);
            if (!updateUserResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return Failed(updateUserResult.Errors.Select(error => error.Description));
            }

            employee.FullName = dto.FullName.Trim();
            employee.Email = email;
            employee.PhoneNumber = phoneNumber;
            employee.NationalId = nationalId;
            employee.Salary = dto.Salary;
            employee.HireDate = dto.HireDate;
            employee.JobTitle = dto.JobTitle.Trim();
            employee.Department = dto.Department;
            employee.IsActive = dto.IsActive;
            employee.UpdatedAtUtc = DateTime.UtcNow;

            if (dto.IsActive)
            {
                user.LockoutEnd = null;
                user.LockoutEnabled = true;
            }
            else
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.MaxValue;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Succeeded();
        }

        public async Task<EmployeeOperationResultDto> DeactivateEmployeeAsync(DeactivateEmployeeDto dto)
        {
            var employee = await _context.Employees
                .Include(e => e.ApplicationUser)
                .FirstOrDefaultAsync(e => e.Id == dto.Id);

            if (employee == null)
            {
                return NotFound();
            }

            employee.IsActive = false;
            employee.UpdatedAtUtc = DateTime.UtcNow;

            employee.ApplicationUser.LockoutEnabled = true;
            employee.ApplicationUser.LockoutEnd = DateTimeOffset.MaxValue;

            await _context.SaveChangesAsync();

            return Succeeded();
        }

        private async Task<List<string>> GetCreateValidationErrorsAsync(CreateEmployeeDto dto, string email, string nationalId, string phoneNumber)
        {
            var errors = new List<string>();
            AddCommonValidationErrors(errors, dto.FullName, nationalId, phoneNumber, dto.Salary, dto.HireDate, dto.JobTitle);

            if (await _userManager.FindByEmailAsync(email) != null)
            {
                errors.Add("Email is already used by another account.");
            }

            if (await _context.Employees.AnyAsync(e => e.Email == email))
            {
                errors.Add("Email is already used by another employee.");
            }

            if (await _context.Employees.AnyAsync(e => e.NationalId == nationalId))
            {
                errors.Add("National ID is already used by another employee.");
            }

            return errors;
        }

        private async Task<List<string>> GetUpdateValidationErrorsAsync(UpdateEmployeeDto dto, string applicationUserId, string email, string nationalId, string phoneNumber)
        {
            var errors = new List<string>();
            AddCommonValidationErrors(errors, dto.FullName, nationalId, phoneNumber, dto.Salary, dto.HireDate, dto.JobTitle);

            var userWithEmail = await _userManager.FindByEmailAsync(email);
            if (userWithEmail != null && userWithEmail.Id != applicationUserId)
            {
                errors.Add("Email is already used by another account.");
            }

            if (await _context.Employees.AnyAsync(e => e.Id != dto.Id && e.Email == email))
            {
                errors.Add("Email is already used by another employee.");
            }

            if (await _context.Employees.AnyAsync(e => e.Id != dto.Id && e.NationalId == nationalId))
            {
                errors.Add("National ID is already used by another employee.");
            }

            return errors;
        }

        private static void AddCommonValidationErrors(List<string> errors, string fullName, string nationalId, string phoneNumber, decimal salary, DateTime hireDate, string jobTitle)
        {
            if (string.IsNullOrWhiteSpace(fullName) || fullName.Trim().Length < 3)
            {
                errors.Add("Employee name must be at least 3 characters.");
            }

            if (!EgyptianNationalIdRegex.IsMatch(nationalId))
            {
                errors.Add("National ID must be exactly 14 digits and start with 2 or 3.");
            }

            if (!string.IsNullOrWhiteSpace(phoneNumber) && !EgyptianMobileRegex.IsMatch(phoneNumber))
            {
                errors.Add("Phone number must be a valid Egyptian mobile number.");
            }

            if (salary <= 0)
            {
                errors.Add("Salary must be greater than 0.");
            }

            if (hireDate.Date > DateTime.Today)
            {
                errors.Add("Hire date cannot be in the future.");
            }

            if (string.IsNullOrWhiteSpace(jobTitle) || jobTitle.Trim().Length < 2)
            {
                errors.Add("Job title must be at least 2 characters.");
            }
        }

        private static EmployeeOperationResultDto Succeeded()
        {
            return new EmployeeOperationResultDto
            {
                Succeeded = true
            };
        }

        private static EmployeeOperationResultDto Failed(IEnumerable<string> errors)
        {
            return new EmployeeOperationResultDto
            {
                Succeeded = false,
                Errors = errors.ToList()
            };
        }

        private static EmployeeOperationResultDto NotFound()
        {
            return new EmployeeOperationResultDto
            {
                Succeeded = false,
                NotFound = true
            };
        }
    }
}
