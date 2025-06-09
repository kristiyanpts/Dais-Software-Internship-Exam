namespace FinalProject.Services.DTOs.Requests.Payment.CreatePayment
{
    public class CreatePaymentDto
    {
        public string FromAccountNumber { get; set; }
        public string ToAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}