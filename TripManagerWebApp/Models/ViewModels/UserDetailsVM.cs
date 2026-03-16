using System.ComponentModel.DataAnnotations;

namespace TripManagerWebApp.Models.ViewModels
{
    public class UserDetailsVM
    {
        public int Id { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }
    }
}
