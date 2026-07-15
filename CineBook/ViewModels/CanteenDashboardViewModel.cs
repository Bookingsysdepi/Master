using CineBook.Dtos.SnackOrders;
using CineBook.Dtos.Snacks;

namespace CineBook.ViewModels
{
    public class CanteenDashboardViewModel
    {
        public List<SnackOrderListDto> Orders { get; set; } = new List<SnackOrderListDto>();

        public List<SnackListDto> Snacks { get; set; } = new List<SnackListDto>();
    }
}
