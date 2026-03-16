using System.ComponentModel.DataAnnotations;

namespace TripManagerWebApp.Models.ViewModels
{
    public class UserLoginVM
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 255 characters.")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
