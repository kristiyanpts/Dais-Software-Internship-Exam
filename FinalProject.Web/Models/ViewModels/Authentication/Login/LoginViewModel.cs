namespace FinalProject.Web.Models.ViewModels.Authentication.Login
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? ReturnUrl { get; set; }
    }
}