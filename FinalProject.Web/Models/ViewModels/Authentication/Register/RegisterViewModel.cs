using System.ComponentModel.DataAnnotations;

namespace FinalProject.Web.Models.ViewModels.Authentication.Register
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Invalid email address!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required!")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Full name is required!")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be at least 3 characters long!")]
        public string FullName { get; set; }

        public string? ReturnUrl { get; set; }
    }
}