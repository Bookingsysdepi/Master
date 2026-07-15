using CineBook.Models;

namespace CineBook.ViewModels
{
    public class MovieIndexViewModel
    {
        public List<Movie> NowShowingMovies { get; set; } = new List<Movie>();
        public List<Movie> ComingSoonMovies { get; set; } = new List<Movie>();
    }
}
