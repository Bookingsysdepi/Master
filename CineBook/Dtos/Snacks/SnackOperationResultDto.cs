namespace CineBook.Dtos.Snacks
{
    public class SnackOperationResultDto
    {
        public bool Succeeded { get; set; }

        public bool NotFound { get; set; }

        public List<string> Errors { get; set; } = new List<string>();
    }
}
