using FinalProject.Web.Models.ViewModels.Home;

namespace FinalProject.Web.Models.ViewModels.Payment.Create
{
    public class CreatePaymentViewModel
    {
        public List<AccountViewModel>? Accounts { get; set; }

        public string FromAccountNumber { get; set; }
        public string ToAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}