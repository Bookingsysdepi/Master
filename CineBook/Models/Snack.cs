using System.ComponentModel.DataAnnotations;

namespace CineBook.Models
{
    public enum SnackCategory
    {
        Popcorn,
        Drink,
        Candy,
        Combo,
        Snack
    }

    public class Snack
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public SnackCategory Category { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }
    }
}
