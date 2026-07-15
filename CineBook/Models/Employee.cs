using System.ComponentModel.DataAnnotations;

namespace CineBook.Models
{
    public enum EmployeeDepartment
    {
        Canteen
    }

    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string NationalId { get; set; } = string.Empty;

        public decimal Salary { get; set; }

        public DateTime HireDate { get; set; }

        [Required]
        [MaxLength(100)]
        public string JobTitle { get; set; } = string.Empty;

        public EmployeeDepartment Department { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }
    }
}
