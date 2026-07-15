using CineBook.Dtos.Snacks;

namespace CineBook.ViewModels
{
    public class AdminSnackIndexViewModel
    {
        public SearchSnacksDto Search { get; set; } = new SearchSnacksDto();

        public List<SnackListDto> Snacks { get; set; } = new List<SnackListDto>();
    }
}
