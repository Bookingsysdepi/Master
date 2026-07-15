using CineBook.Models;

namespace CineBook.Dtos.SnackOrders
{
    public class SnackOrderListDto
    {
        public int Id { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string MovieTitle { get; set; } = string.Empty;

        public string HallName { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }

        public SnackOrderStatus Status { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public List<string> Seats { get; set; } = new List<string>();

        public List<SnackOrderItemListDto> Items { get; set; } = new List<SnackOrderItemListDto>();
    }
}
