namespace FinalProject.Web.Models.ViewModels.Payment
{
    public enum PaymentStatus
    {
        PENDING,
        PROCESSED,
        CANCELLED
    }

    public class PaymentViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int FromAccountId { get; set; }
        public string FromAccountNumber { get; set; }
        public int ToAccountId { get; set; }
        public string ToAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}