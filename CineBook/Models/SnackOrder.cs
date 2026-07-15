namespace CineBook.Models
{
    public enum SnackOrderStatus
    {
        Pending,
        Preparing,
        OutForDelivery,
        Delivered,
        Cancelled
    }

    public class SnackOrder
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;

        public int? AssignedEmployeeId { get; set; }
        public Employee? AssignedEmployee { get; set; }

        public SnackOrderStatus Status { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? DeliveredAtUtc { get; set; }

        public List<SnackOrderItem> Items { get; set; } = new List<SnackOrderItem>();

        public List<SnackOrderSeat> Seats { get; set; } = new List<SnackOrderSeat>();
    }
}
