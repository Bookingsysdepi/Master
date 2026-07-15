using CineBook.Models;
using CineBook.Dtos.Showtimes;


namespace CineBook.Dtos.Movies
{
    public class MovieDetailsDto
    {

        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? PosterUrl { get; set; }

        public string? Genre { get; set; }

        public int DurationMinutes { get; set; }

        public string? Language { get; set; }

        public DateTime ReleaseDate { get; set; }

        public MovieStatus Status { get; set; }

        public List<MovieShowtimeDto> Showtimes { get; set; } = new();

    }
}
