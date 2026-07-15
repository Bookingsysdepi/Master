using CineBook.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CineBook.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        public async Task<IActionResult> Index()
        {
            var dto = await _movieService.GetMovieIndexAsync();

            return View(dto);
        }

        public async Task<IActionResult> Details(int id)
        {
            var dto = await _movieService.GetMovieDetailsAsync(id);

            if (dto == null)
            {
                return NotFound();
            }

            return View(dto);
        }
    }
}