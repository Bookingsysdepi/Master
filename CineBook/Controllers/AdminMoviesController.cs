using CineBook.Dtos.Movies;
using CineBook.Dtos.Showtimes;
using CineBook.Services.Interfaces;
using CineBook.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineBook.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Movies")]
    public class AdminMoviesController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IShowtimeService _showtimeService;

        public AdminMoviesController(IMovieService movieService, IShowtimeService showtimeService)
        {
            _movieService = movieService;
            _showtimeService = showtimeService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(SearchMoviesDto search)
        {
            var moviesDto = await _movieService.GetMoviesAsync(search);

            var viewModel = new AdminMovieIndexViewModel
            {
                Search = search,
                Movies = moviesDto.Movies
            };

            return View("~/Views/Admin/Movies/Index.cshtml", viewModel);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Movies/Create.cshtml", new CreateMovieDto());
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMovieDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/Movies/Create.cshtml", dto);
            }

            await _movieService.CreateMovieAsync(dto);

            TempData["AdminMovieMessage"] = "Movie added successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _movieService.GetMovieForEditAsync(new GetMovieForEditDto
            {
                Id = id
            });

            if (dto == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Movies/Edit.cshtml", dto);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateMovieDto dto)
        {
            dto.Id = id;

            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/Movies/Edit.cshtml", dto);
            }

            var result = await _movieService.UpdateMovieAsync(dto);

            if (result.Result == UpdateMovieResult.NotFound)
            {
                return NotFound();
            }

            TempData["AdminMovieMessage"] = "Movie updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DeleteMovieDto dto)
        {
            var result = await _movieService.DeleteMovieAsync(dto);

            if (result.Result == DeleteMovieResult.NotFound)
            {
                return NotFound();
            }

            if (result.Result == DeleteMovieResult.HasShowtimes)
            {
                TempData["AdminMovieError"] = "This movie has showtimes and cannot be deleted.";
                return RedirectToAction(nameof(Index));
            }

            TempData["AdminMovieMessage"] = "Movie deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("{movieId}/Showtimes/Create")]
        public async Task<IActionResult> CreateShowtime(int movieId)
        {
            var form = await _showtimeService.GetCreateShowtimeFormAsync(new CreateShowtimeFormRequestDto
            {
                MovieId = movieId
            });

            if (form == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Movies/CreateShowtime.cshtml", form);
        }

        [HttpPost("{movieId}/Showtimes/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateShowtime(int movieId, CreateShowtimeDto dto)
        {
            dto.MovieId = movieId;

            if (!ModelState.IsValid)
            {
                return await CreateShowtimeFormView(movieId, dto);
            }

            var result = await _showtimeService.CreateShowtimeAsync(dto);

            if (result.Result == CreateShowtimeResult.MovieNotFound)
            {
                return NotFound();
            }

            if (result.Result == CreateShowtimeResult.HallNotFound)
            {
                ModelState.AddModelError(nameof(CreateShowtimeDto.HallId), "Select a valid hall.");
                return await CreateShowtimeFormView(movieId, dto);
            }

            if (result.Result == CreateShowtimeResult.StartTimeNotUpcoming)
            {
                ModelState.AddModelError(nameof(CreateShowtimeDto.StartTime), "Showtime must be in the future.");
                return await CreateShowtimeFormView(movieId, dto);
            }

            if (result.Result == CreateShowtimeResult.HallUnavailable)
            {
                ModelState.AddModelError(nameof(CreateShowtimeDto.StartTime), "This hall already has a showtime during that time.");
                return await CreateShowtimeFormView(movieId, dto);
            }

            TempData["AdminMovieMessage"] = "Showtime added successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> CreateShowtimeFormView(int movieId, CreateShowtimeDto dto)
        {
            var form = await _showtimeService.GetCreateShowtimeFormAsync(new CreateShowtimeFormRequestDto
            {
                MovieId = movieId,
                Showtime = dto
            });

            if (form == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Movies/CreateShowtime.cshtml", form);
        }
    }
}
