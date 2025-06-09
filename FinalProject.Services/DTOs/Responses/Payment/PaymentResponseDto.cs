namespace FinalProject.Services.DTOs.Responses.Payment
{
    public class PaymentResponseDto
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
        public Models.PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}