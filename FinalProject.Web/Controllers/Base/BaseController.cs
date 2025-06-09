using Microsoft.AspNetCore.Mvc;
using FinalProject.Services.DTOs.Responses.User;

namespace FinalProject.Web.Controllers.Base
{
    public abstract class BaseController : Controller
    {
        protected int GetUserId() => HttpContext.Session.GetInt32("UserId") ?? 0;

        protected UserResponseDto GetUserSessionData()
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var username = HttpContext.Session.GetString("Username") ?? string.Empty;
            var fullName = HttpContext.Session.GetString("FullName") ?? string.Empty;

            return new UserResponseDto()
            {
                Id = userId,
                Username = username,
                FullName = fullName,
            };
        }
    }
}