using Microsoft.AspNetCore.Identity;

namespace CineBook.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}
