namespace CineBook.Dtos.Movies
{
    public class MovieIndexDto
    {



        public List<MovieCardDto> NowShowingMovies { get; set; } = new();

        public List<MovieCardDto> ComingSoonMovies { get; set; } = new();
    }
}
