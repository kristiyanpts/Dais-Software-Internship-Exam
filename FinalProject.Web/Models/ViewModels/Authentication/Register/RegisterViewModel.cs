using System.ComponentModel.DataAnnotations;

namespace FinalProject.Web.Models.ViewModels.Authentication.Register
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username is required!")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be at least 3 characters and maximum 50 characters long!")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters and maximum 16 characters long!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required!")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Full name is required!")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be at least 3 characters long!")]
        public string FullName { get; set; }

        public string? ReturnUrl { get; set; }
    }
}