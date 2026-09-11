using System.Security.Cryptography;
using System.Text;

namespace DonaAhora.Services
{
    public static class PasswordHelper
    {
        private const string Salt = "DonaAhora_Salt_2026_UASD";

        public static string Hash(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password + Salt);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static bool Verify(string password, string hash)
        {
            return Hash(password) == hash;
        }
    }
}
