using Microsoft.AspNetCore.Mvc;
using FinalProject.Web.Models.ViewModels.Authentication;

namespace FinalProject.Web.Controllers.Base
{
    public abstract class BaseController : Controller
    {
        protected int GetUserId() => HttpContext.Session.GetInt32("UserId") ?? 0;

        protected UserViewModel GetUserSessionData()
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var email = HttpContext.Session.GetString("Email") ?? string.Empty;
            var fullName = HttpContext.Session.GetString("FullName") ?? string.Empty;

            return new UserViewModel()
            {
                Id = userId,
                Email = email,
                FullName = fullName,
            };
        }
    }
}