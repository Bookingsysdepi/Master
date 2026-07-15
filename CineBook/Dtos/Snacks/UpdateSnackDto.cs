using CineBook.Models;
using System.ComponentModel.DataAnnotations;

namespace CineBook.Dtos.Snacks
{
    public class UpdateSnackDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public SnackCategory Category { get; set; }

        [Range(0.01, 9999.99)]
        public decimal Price { get; set; }

        [Url]
        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; }
    }
}
