using CineBook.Models;
using System.ComponentModel.DataAnnotations;

namespace CineBook.Dtos.Employees
{
    public class UpdateEmployeeDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [MaxLength(20)]
        [RegularExpression(@"^(01[0125][0-9]{8}|\+201[0125][0-9]{8})$", ErrorMessage = "Enter a valid Egyptian mobile number.")]
        public string? PhoneNumber { get; set; }

        [Required]
        [RegularExpression(@"^[23][0-9]{13}$", ErrorMessage = "Enter a valid 14-digit Egyptian national ID.")]
        public string NationalId { get; set; } = string.Empty;

        [Range(1, 999999.99)]
        public decimal Salary { get; set; }

        [Required]
        public DateTime HireDate { get; set; }

        [Required]
        [MaxLength(100)]
        public string JobTitle { get; set; } = string.Empty;

        [Required]
        public EmployeeDepartment Department { get; set; }

        public bool IsActive { get; set; }
    }
}
