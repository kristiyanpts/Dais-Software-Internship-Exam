using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Потребителското име е задължително")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Потребителското име трябва да е между 3 и 50 символа!")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Паролата е задължителна")]
        [RegularExpression(@"^[a-fA-F0-9]{64}$", ErrorMessage = "Паролата трябва да е 64 символа и да съдържа само шестнайсетични цифри")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Името е задължително")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Името трябва да е между 3 и 255 символа!")]
        public string FullName { get; set; }
    }
}