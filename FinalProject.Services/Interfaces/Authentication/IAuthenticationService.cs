using FinalProject.Services.DTOs.Requests.Authentication.Login;
using FinalProject.Services.DTOs.Requests.Authentication.Register;
using FinalProject.Services.DTOs.Responses.Authentication.Login;
using FinalProject.Services.DTOs.Responses.Authentication.Register;

namespace FinalProject.Services.Interfaces.Authentication
{
    public interface IAuthenticationService
    {
        Task<LoginResponse> Login(LoginRequest request);
        Task<RegisterResponse> Register(RegisterRequest request);
    }
}