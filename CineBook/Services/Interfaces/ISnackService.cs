using CineBook.Dtos.Snacks;

namespace CineBook.Services.Interfaces
{
    public interface ISnackService
    {
        Task<SnackOperationResultDto> CreateSnackAsync(CreateSnackDto dto);
        Task<UpdateSnackDto?> GetSnackForEditAsync(GetSnackForEditDto dto);
        Task<SnacksDto> GetSnacksAsync(SearchSnacksDto search);
        Task<SnackOperationResultDto> ToggleSnackAvailabilityAsync(ToggleSnackAvailabilityDto dto);
        Task<SnackOperationResultDto> UpdateSnackAsync(UpdateSnackDto dto);
    }
}
