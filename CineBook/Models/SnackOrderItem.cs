namespace CineBook.Models
{
    public class SnackOrderItem
    {
        public int Id { get; set; }

        public int SnackOrderId { get; set; }
        public SnackOrder SnackOrder { get; set; } = null!;

        public int SnackId { get; set; }
        public Snack Snack { get; set; } = null!;

        public string ItemNameSnapshot { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
