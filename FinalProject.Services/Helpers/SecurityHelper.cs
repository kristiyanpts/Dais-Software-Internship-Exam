using System.Security.Cryptography;
using System.Text;

namespace FinalProject.Services.Helpers
{
    public static class SecurityHelper
    {
        public static string HashPassword(string password)
        {
            var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(hashedBytes).ToLower();
        }
    }
}