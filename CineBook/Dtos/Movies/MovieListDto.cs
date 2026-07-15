namespace CineBook.Dtos.Movies
{
    public class MovieListDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;

        public int DurationMinutes { get; set; }

        public string Language { get; set; } = string.Empty;

        public DateTime ReleaseDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public int ShowtimesCount { get; set; }
    }
}
