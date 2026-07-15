namespace CineBook.Dtos.Showtimes
{
    public class CreateShowtimeFormRequestDto
    {
        public int MovieId { get; set; }

        public CreateShowtimeDto? Showtime { get; set; }
    }
}
