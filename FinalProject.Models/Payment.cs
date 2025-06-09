using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public enum PaymentStatus
    {
        PENDING,
        PROCESSED,
        CANCELLED
    }

    public class Payment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Потребителският ID е задължителен")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Сметковият ID на изпращача е задължителен")]
        public int FromAccountId { get; set; }

        [Required(ErrorMessage = "Сметковият ID на получателя е задължителен")]
        public int ToAccountId { get; set; }

        [Required(ErrorMessage = "Сума е задължителна")]
        [Range(0, double.MaxValue, ErrorMessage = "Сума трябва да е по-голяма от 0!")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Описание е задължително")]
        [StringLength(32, MinimumLength = 3, ErrorMessage = "Описание трябва да е между 3 и 32 символа!")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Статусът е задължителен")]
        [EnumDataType(typeof(PaymentStatus), ErrorMessage = "Невалиден статус")]
        public PaymentStatus Status { get; set; }

        [Required(ErrorMessage = "Дата на създаване е задължителна")]
        public DateTime CreatedAt { get; set; }
    }
}