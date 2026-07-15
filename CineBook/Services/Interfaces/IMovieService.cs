using CineBook.Dtos.Movies;

namespace CineBook.Services.Interfaces
{
    public interface IMovieService
    {
        Task CreateMovieAsync(CreateMovieDto dto);
        Task<DeleteMovieResultDto> DeleteMovieAsync(DeleteMovieDto dto);
        Task<UpdateMovieDto?> GetMovieForEditAsync(GetMovieForEditDto dto);
        Task<MoviesDto> GetMoviesAsync(SearchMoviesDto search);
        Task<UpdateMovieResultDto> UpdateMovieAsync(UpdateMovieDto dto);


        Task<MovieIndexDto> GetMovieIndexAsync();

        Task<MovieDetailsDto?> GetMovieDetailsAsync(int movieId);
    }
}
