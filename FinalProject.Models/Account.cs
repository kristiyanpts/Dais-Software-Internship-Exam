using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public class Account
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Номерът на сметката е задължителен")]
        [StringLength(22, MinimumLength = 22, ErrorMessage = "Номерът на сметката трябва да е 22 символа!")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Балансът е задължителен")]
        [Range(0, double.MaxValue, ErrorMessage = "Балансът трябва да е по-голям или равен на 0!")]
        public decimal Balance { get; set; }
    }
}