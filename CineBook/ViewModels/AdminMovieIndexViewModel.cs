using CineBook.Dtos.Movies;

namespace CineBook.ViewModels
{
    public class AdminMovieIndexViewModel
    {
        public SearchMoviesDto Search { get; set; } = new SearchMoviesDto();

        public List<MovieListDto> Movies { get; set; } = new List<MovieListDto>();
    }
}
