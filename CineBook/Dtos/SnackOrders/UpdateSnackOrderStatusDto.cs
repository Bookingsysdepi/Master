using CineBook.Models;

namespace CineBook.Dtos.SnackOrders
{
    public class UpdateSnackOrderStatusDto
    {
        public int Id { get; set; }

        public string EmployeeUserId { get; set; } = string.Empty;

        public SnackOrderStatus Status { get; set; }
    }
}
