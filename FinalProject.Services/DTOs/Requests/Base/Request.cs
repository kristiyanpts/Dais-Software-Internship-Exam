using FinalProject.Services.DTOs.Responses.User;

namespace FinalProject.Services.DTOs.Requests.Base
{
    public abstract class Request<T>
    {
        public UserResponseDto? User { get; set; }
        public T Data { get; set; }
    }
}