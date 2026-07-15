using CineBook.Models;

namespace CineBook.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Halls.Any())
            {
                var halls = new List<Hall>
                {
                    new Hall { Name = "Hall 1", RowsCount = 8, SeatsPerRow = 10 },
                    new Hall { Name = "Hall 2", RowsCount = 7, SeatsPerRow = 9 },
                    new Hall { Name = "Hall 3", RowsCount = 6, SeatsPerRow = 8 }
                };

                foreach (var hall in halls)
                {
                    for (var row = 0; row < hall.RowsCount; row++)
                    {
                        var rowLabel = ((char)('A' + row)).ToString();
                        var seatType = row >= hall.RowsCount - 2 ? SeatType.VIP : SeatType.Normal;

                        for (var seatNumber = 1; seatNumber <= hall.SeatsPerRow; seatNumber++)
                        {
                            hall.Seats.Add(new Seat
                            {
                                RowLabel = rowLabel,
                                SeatNumber = seatNumber,
                                SeatType = seatType
                            });
                        }
                    }
                }

                context.Halls.AddRange(halls);
                context.SaveChanges();
            }

            if (!context.Movies.Any())
            {
                var today = DateTime.Today;

                var movies = new List<Movie>
                {
                    new Movie
                    {
                        Title = "Dune: Part Two",
                        Description = "Paul Atreides joins the Fremen while facing a dangerous future across Arrakis.",
                        DurationMinutes = 166,
                        Genre = Genre.SciFi,
                        Language = "English",
                        PosterUrl = "https://via.placeholder.com/300x450?text=Dune+Part+Two",
                        ReleaseDate = today.AddMonths(-4),
                        Status = MovieStatus.NowShowing
                    },
                    new Movie
                    {
                        Title = "Inside Out 2",
                        Description = "Riley enters her teenage years as new emotions arrive at headquarters.",
                        DurationMinutes = 96,
                        Genre = Genre.Animation,
                        Language = "English",
                        PosterUrl = "https://via.placeholder.com/300x450?text=Inside+Out+2",
                        ReleaseDate = today.AddMonths(-1),
                        Status = MovieStatus.NowShowing
                    },
                    new Movie
                    {
                        Title = "The Fall Guy",
                        Description = "A stuntman is pulled into a mystery while working on a major film production.",
                        DurationMinutes = 126,
                        Genre = Genre.Action,
                        Language = "English",
                        PosterUrl = "https://via.placeholder.com/300x450?text=The+Fall+Guy",
                        ReleaseDate = today.AddMonths(-2),
                        Status = MovieStatus.NowShowing
                    },
                    new Movie
                    {
                        Title = "A Quiet Place: Day One",
                        Description = "A woman tries to survive the first day of a terrifying invasion in New York.",
                        DurationMinutes = 99,
                        Genre = Genre.Horror,
                        Language = "English",
                        PosterUrl = "https://via.placeholder.com/300x450?text=A+Quiet+Place",
                        ReleaseDate = today.AddDays(-10),
                        Status = MovieStatus.NowShowing
                    },
                    new Movie
                    {
                        Title = "Bad Boys: Ride or Die",
                        Description = "Two Miami detectives return for another fast and chaotic case.",
                        DurationMinutes = 115,
                        Genre = Genre.Action,
                        Language = "English",
                        PosterUrl = "https://via.placeholder.com/300x450?text=Bad+Boys",
                        ReleaseDate = today.AddDays(-25),
                        Status = MovieStatus.NowShowing
                    },
                    new Movie
                    {
                        Title = "El Hana El Ana Feeh",
                        Description = "An Egyptian comedy about family surprises, friendship, and unexpected plans.",
                        DurationMinutes = 110,
                        Genre = Genre.Comedy,
                        Language = "Arabic",
                        PosterUrl = "https://via.placeholder.com/300x450?text=El+Hana",
                        ReleaseDate = today.AddDays(14),
                        Status = MovieStatus.ComingSoon
                    },
                    new Movie
                    {
                        Title = "Kingdom of the Planet of the Apes",
                        Description = "A young ape begins a journey that changes the future for apes and humans.",
                        DurationMinutes = 145,
                        Genre = Genre.SciFi,
                        Language = "English",
                        PosterUrl = "https://via.placeholder.com/300x450?text=Planet+of+the+Apes",
                        ReleaseDate = today.AddDays(21),
                        Status = MovieStatus.ComingSoon
                    }
                };

                context.Movies.AddRange(movies);
                context.SaveChanges();
            }

            if (!context.Showtimes.Any())
            {
                var halls = context.Halls.OrderBy(h => h.Id).ToList();
                var nowShowingMovies = context.Movies
                    .Where(m => m.Status == MovieStatus.NowShowing)
                    .OrderBy(m => m.Id)
                    .ToList();

                if (halls.Count == 0 || nowShowingMovies.Count == 0)
                {
                    return;
                }

                var startDate = DateTime.Today.AddDays(1);

                var showtimes = new List<Showtime>
                {
                    new Showtime { MovieId = nowShowingMovies[0].Id, HallId = halls[0].Id, StartTime = startDate.AddHours(14), NormalPrice = 60, VipPrice = 100 },
                    new Showtime { MovieId = nowShowingMovies[0].Id, HallId = halls[1].Id, StartTime = startDate.AddHours(20), NormalPrice = 70, VipPrice = 120 },
                    new Showtime { MovieId = nowShowingMovies[1].Id, HallId = halls[0].Id, StartTime = startDate.AddDays(1).AddHours(16), NormalPrice = 60, VipPrice = 100 },
                    new Showtime { MovieId = nowShowingMovies[1].Id, HallId = halls[2].Id, StartTime = startDate.AddDays(2).AddHours(18), NormalPrice = 65, VipPrice = 110 },
                    new Showtime { MovieId = nowShowingMovies[2].Id, HallId = halls[1].Id, StartTime = startDate.AddDays(1).AddHours(21), NormalPrice = 75, VipPrice = 125 },
                    new Showtime { MovieId = nowShowingMovies[3].Id, HallId = halls[2].Id, StartTime = startDate.AddDays(3).AddHours(19), NormalPrice = 70, VipPrice = 115 },
                    new Showtime { MovieId = nowShowingMovies[4].Id, HallId = halls[0].Id, StartTime = startDate.AddDays(3).AddHours(22), NormalPrice = 75, VipPrice = 130 },
                    new Showtime { MovieId = nowShowingMovies[4].Id, HallId = halls[1].Id, StartTime = startDate.AddDays(4).AddHours(17), NormalPrice = 70, VipPrice = 120 }
                };

                context.Showtimes.AddRange(showtimes);
                context.SaveChanges();
            }
        }
    }
}
