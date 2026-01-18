using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace UwfManager.Services
{
    public class PasswordService
    {
        // Simple hash storage in local app data
        private readonly string _authFilePath;

        public PasswordService()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(appData, "UwfManager");
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }
            _authFilePath = Path.Combine(appFolder, "auth.dat");
        }

        public bool IsPasswordSet()
        {
            return File.Exists(_authFilePath);
        }

        public void SetPassword(string password)
        {
            var hash = HashPassword(password);
            File.WriteAllText(_authFilePath, hash);
        }

        public bool VerifyPassword(string input)
        {
            if (!IsPasswordSet()) return false;
            var storedHash = File.ReadAllText(_authFilePath);
            var inputHash = HashPassword(input);
            return storedHash == inputHash;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
