namespace CineBook.Dtos.SnackOrders
{
    public class SnackOrderItemListDto
    {
        public string Name { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
