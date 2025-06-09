using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid email address!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^[a-fA-F0-9]{64}$", ErrorMessage = "Password must be 64 characters long and contain only hexadecimal characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 255 characters!")]
        public string FullName { get; set; }
    }
}