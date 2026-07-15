using CineBook.Models;

namespace CineBook.Dtos.Employees
{
    public class EmployeeListDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string NationalId { get; set; } = string.Empty;

        public decimal Salary { get; set; }

        public DateTime HireDate { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public EmployeeDepartment Department { get; set; }

        public bool IsActive { get; set; }
    }
}
