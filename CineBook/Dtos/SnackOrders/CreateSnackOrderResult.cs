namespace CineBook.Dtos.SnackOrders
{
    public enum CreateSnackOrderResult
    {
        Created,
        BookingNotFound,
        BookingNotConfirmed,
        ShowtimeEnded,
        NoSeatsSelected,
        InvalidSeats,
        NoItemsSelected,
        InvalidQuantity,
        InvalidSnacks
    }
}
