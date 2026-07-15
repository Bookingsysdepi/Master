namespace CineBook.Dtos.Showtimes
{
    public enum CreateShowtimeResult
    {
        Created,
        MovieNotFound,
        HallNotFound,
        StartTimeNotUpcoming,
        HallUnavailable
    }
}
