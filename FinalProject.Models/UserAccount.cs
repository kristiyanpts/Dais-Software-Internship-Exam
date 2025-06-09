using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public class UserAccount
    {
        [Required(ErrorMessage = "Потребителският ID е задължителен")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Сметковият ID е задължителен")]
        public int AccountId { get; set; }
    }
}