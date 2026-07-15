using CineBook.Models;
using System.ComponentModel.DataAnnotations;

namespace CineBook.Dtos.Movies
{
    public class CreateMovieDto
    {

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(1, 500)]
        public int DurationMinutes { get; set; }

        [Required]
        public Genre Genre { get; set; }

        [Required]
        [MaxLength(50)]
        public string Language { get; set; } = string.Empty;

        public string? PosterUrl { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public MovieStatus Status { get; set; }




    }
}
