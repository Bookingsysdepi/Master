using CineBook.Dtos.SnackOrders;

namespace CineBook.Services.Interfaces
{
    public interface ISnackOrderService
    {
        Task<CreateSnackOrderResultDto> CreateSnackOrderAsync(CreateSnackOrderDto dto);
        Task<SnackOrderFormDto?> GetSnackOrderFormAsync(int bookingId, string userId);
        Task<SnackOrdersDto> GetCanteenOrdersAsync();
        Task<UpdateSnackOrderStatusResultDto> UpdateSnackOrderStatusAsync(UpdateSnackOrderStatusDto dto);
    }
}
