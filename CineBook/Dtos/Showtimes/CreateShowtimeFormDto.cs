namespace CineBook.Dtos.Showtimes
{
    public class CreateShowtimeFormDto : CreateShowtimeDto
    {
        public string MovieTitle { get; set; } = string.Empty;

        public DateTime MinimumStartTime { get; set; }

        public List<HallOptionDto> Halls { get; set; } = new List<HallOptionDto>();
    }
}
