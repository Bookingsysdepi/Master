using System.ComponentModel.DataAnnotations;

namespace CineBook.Dtos.Showtimes
{
    public class CreateShowtimeDto
    {
        public int MovieId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Select a hall.")]
        public int HallId { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }

        [Range(0.01, 9999.99)]
        public decimal NormalPrice { get; set; }

        [Range(0.01, 9999.99)]
        public decimal VipPrice { get; set; }
    }
}
