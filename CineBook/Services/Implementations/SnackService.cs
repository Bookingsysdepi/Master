using CineBook.Data;
using CineBook.Dtos.Snacks;
using CineBook.Models;
using CineBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineBook.Services.Implementations
{
    public class SnackService : ISnackService
    {
        private readonly ApplicationDbContext _context;

        public SnackService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SnacksDto> GetSnacksAsync(SearchSnacksDto search)
        {
            var query = _context.Snacks
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search.SearchTerm))
            {
                var searchTerm = search.SearchTerm.Trim();

                query = query.Where(s =>
                    s.Name.Contains(searchTerm) ||
                    s.Description.Contains(searchTerm));
            }

            if (search.Category.HasValue)
            {
                query = query.Where(s => s.Category == search.Category.Value);
            }

            if (search.IsAvailable.HasValue)
            {
                query = query.Where(s => s.IsAvailable == search.IsAvailable.Value);
            }

            var snacks = await query
                .OrderBy(s => s.Category)
                .ThenBy(s => s.Name)
                .Select(s => new SnackListDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Category = s.Category,
                    Price = s.Price,
                    ImageUrl = s.ImageUrl,
                    IsAvailable = s.IsAvailable
                })
                .ToListAsync();

            return new SnacksDto
            {
                Snacks = snacks
            };
        }

        public async Task<SnackOperationResultDto> CreateSnackAsync(CreateSnackDto dto)
        {
            var name = dto.Name.Trim();
            var description = dto.Description?.Trim() ?? string.Empty;
            var imageUrl = dto.ImageUrl?.Trim() ?? string.Empty;

            var validationErrors = await GetCreateValidationErrorsAsync(name, dto.Price);
            if (validationErrors.Any())
            {
                return Failed(validationErrors);
            }

            var snack = new Snack
            {
                Name = name,
                Description = description,
                Category = dto.Category,
                Price = dto.Price,
                ImageUrl = imageUrl,
                IsAvailable = dto.IsAvailable,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.Snacks.Add(snack);
            await _context.SaveChangesAsync();

            return Succeeded();
        }

        public async Task<UpdateSnackDto?> GetSnackForEditAsync(GetSnackForEditDto dto)
        {
            return await _context.Snacks
                .AsNoTracking()
                .Where(s => s.Id == dto.Id)
                .Select(s => new UpdateSnackDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Category = s.Category,
                    Price = s.Price,
                    ImageUrl = s.ImageUrl,
                    IsAvailable = s.IsAvailable
                })
                .FirstOrDefaultAsync();
        }

        public async Task<SnackOperationResultDto> UpdateSnackAsync(UpdateSnackDto dto)
        {
            var snack = await _context.Snacks.FindAsync(dto.Id);

            if (snack == null)
            {
                return NotFound();
            }

            var name = dto.Name.Trim();
            var description = dto.Description?.Trim() ?? string.Empty;
            var imageUrl = dto.ImageUrl?.Trim() ?? string.Empty;

            var validationErrors = await GetUpdateValidationErrorsAsync(dto.Id, name, dto.Price);
            if (validationErrors.Any())
            {
                return Failed(validationErrors);
            }

            snack.Name = name;
            snack.Description = description;
            snack.Category = dto.Category;
            snack.Price = dto.Price;
            snack.ImageUrl = imageUrl;
            snack.IsAvailable = dto.IsAvailable;
            snack.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Succeeded();
        }

        public async Task<SnackOperationResultDto> ToggleSnackAvailabilityAsync(ToggleSnackAvailabilityDto dto)
        {
            var snack = await _context.Snacks.FindAsync(dto.Id);

            if (snack == null)
            {
                return NotFound();
            }

            snack.IsAvailable = !snack.IsAvailable;
            snack.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Succeeded();
        }

        private async Task<List<string>> GetCreateValidationErrorsAsync(string name, decimal price)
        {
            var errors = GetCommonValidationErrors(name, price);

            if (await _context.Snacks.AnyAsync(s => s.Name == name))
            {
                errors.Add("A snack with this name already exists.");
            }

            return errors;
        }

        private async Task<List<string>> GetUpdateValidationErrorsAsync(int snackId, string name, decimal price)
        {
            var errors = GetCommonValidationErrors(name, price);

            if (await _context.Snacks.AnyAsync(s => s.Id != snackId && s.Name == name))
            {
                errors.Add("A snack with this name already exists.");
            }

            return errors;
        }

        private static List<string> GetCommonValidationErrors(string name, decimal price)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            {
                errors.Add("Snack name must be at least 2 characters.");
            }

            if (price <= 0)
            {
                errors.Add("Snack price must be greater than 0.");
            }

            return errors;
        }

        private static SnackOperationResultDto Succeeded()
        {
            return new SnackOperationResultDto
            {
                Succeeded = true
            };
        }

        private static SnackOperationResultDto Failed(IEnumerable<string> errors)
        {
            return new SnackOperationResultDto
            {
                Succeeded = false,
                Errors = errors.ToList()
            };
        }

        private static SnackOperationResultDto NotFound()
        {
            return new SnackOperationResultDto
            {
                Succeeded = false,
                NotFound = true
            };
        }
    }
}
