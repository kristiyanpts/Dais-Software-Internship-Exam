namespace FinalProject.Services.DTOs.Responses.Base
{
    public class Response<T>
    {
        public bool Status { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}