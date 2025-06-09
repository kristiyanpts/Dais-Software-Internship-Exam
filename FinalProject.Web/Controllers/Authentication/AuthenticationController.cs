using Microsoft.AspNetCore.Mvc;
using FinalProject.Services.DTOs.Requests.Authentication.Login;
using FinalProject.Services.DTOs.Requests.Authentication.Register;
using FinalProject.Services.Interfaces.Authentication;
using FinalProject.Web.Controllers.Base;
using FinalProject.Web.Models.ViewModels.Authentication.Login;
using FinalProject.Web.Models.ViewModels.Authentication.Register;
using FinalProject.Web.Models.ViewModels.Authentication;

namespace FinalProject.Web.Controllers.Authentication
{
    [Route("[controller]")]
    public class AuthenticationController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login(string? returnUrl)
        {
            if (GetUserId() != 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (GetUserId() != 0)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var loginRequest = new LoginRequest()
                {
                    Data = new LoginRequestDto()
                    {
                        Username = model.Username,
                        Password = model.Password
                    }
                };

                var result = await _authenticationService.Login(loginRequest);

                if (result.Status == true && result.Data != null)
                {
                    var user = new UserViewModel()
                    {
                        Id = result.Data.Id,
                        Username = result.Data.Username,
                        FullName = result.Data.FullName
                    };

                    SetUserSession(user);

                    if (model.ReturnUrl != null)
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, result.Message ?? "Неуспешно влизане в системата");
            }

            return View(model);
        }

        [HttpGet]
        [Route("register")]
        public IActionResult Register(string? returnUrl)
        {
            if (GetUserId() != 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new RegisterViewModel { ReturnUrl = returnUrl };

            return View(model);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (GetUserId() != 0)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError(string.Empty, "Паролите не съвпадат");
                    return View(model);
                }

                var registerRequest = new RegisterRequest()
                {
                    Data = new RegisterRequestDto()
                    {
                        Username = model.Username,
                        Password = model.Password,
                        FullName = model.FullName,
                        ConfirmPassword = model.ConfirmPassword
                    }
                };

                var result = await _authenticationService.Register(registerRequest);

                if (result.Status == true && result.Data != null)
                {
                    var user = new UserViewModel()
                    {
                        Id = result.Data.Id,
                        Username = result.Data.Username,
                        FullName = result.Data.FullName
                    };

                    SetUserSession(user);

                    if (model.ReturnUrl != null)
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, result.Message ?? "Неуспешна регистрация");
            }

            return View(model);
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            if (GetUserId() == 0)
            {
                return RedirectToAction("Login");
            }

            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        protected void SetUserSession(UserViewModel user)
        {
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("FullName", user.FullName);
        }
    }
}