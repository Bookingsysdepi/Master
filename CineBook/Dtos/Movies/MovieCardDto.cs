namespace CineBook.Dtos.Movies
{
    public class MovieCardDto
    {


        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? PosterUrl { get; set; }

        public int DurationMinutes { get; set; }

        public string? Genre { get; set; }
    }
}
