using CineBook.Models;

namespace CineBook.Dtos.Snacks
{
    public class SnackListDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public SnackCategory Category { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsAvailable { get; set; }
    }
}
