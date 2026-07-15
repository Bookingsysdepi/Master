using CineBook.Dtos.Employees;

namespace CineBook.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<EmployeeOperationResultDto> CreateEmployeeAsync(CreateEmployeeDto dto);
        Task<EmployeeOperationResultDto> DeactivateEmployeeAsync(DeactivateEmployeeDto dto);
        Task<UpdateEmployeeDto?> GetEmployeeForEditAsync(GetEmployeeForEditDto dto);
        Task<EmployeesDto> GetEmployeesAsync(SearchEmployeesDto search);
        Task<EmployeeOperationResultDto> UpdateEmployeeAsync(UpdateEmployeeDto dto);
    }
}
