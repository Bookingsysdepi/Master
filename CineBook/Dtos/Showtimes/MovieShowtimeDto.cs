namespace CineBook.Dtos.Showtimes
{
    public class MovieShowtimeDto
    {



        public int Id { get; set; }

        public DateTime StartTime { get; set; }

        public decimal NormalPrice { get; set; }

        public decimal VipPrice { get; set; }

        public int HallId { get; set; }

        public string HallName { get; set; } = string.Empty;
    }
}
