namespace CineBook.Dtos.SnackOrders
{
    public class CreateSnackOrderDto
    {
        public string UserId { get; set; } = string.Empty;

        public int BookingId { get; set; }

        public List<int> SeatIds { get; set; } = new List<int>();

        public List<CreateSnackOrderItemDto> Items { get; set; } = new List<CreateSnackOrderItemDto>();
    }
}
