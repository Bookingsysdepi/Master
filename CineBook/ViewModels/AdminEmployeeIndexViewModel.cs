using CineBook.Dtos.Employees;

namespace CineBook.ViewModels
{
    public class AdminEmployeeIndexViewModel
    {
        public SearchEmployeesDto Search { get; set; } = new SearchEmployeesDto();

        public List<EmployeeListDto> Employees { get; set; } = new List<EmployeeListDto>();
    }
}
