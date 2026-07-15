using CineBook.Models;

namespace CineBook.Dtos.Snacks
{
    public class SearchSnacksDto
    {
        public string? SearchTerm { get; set; }

        public SnackCategory? Category { get; set; }

        public bool? IsAvailable { get; set; }
    }
}
