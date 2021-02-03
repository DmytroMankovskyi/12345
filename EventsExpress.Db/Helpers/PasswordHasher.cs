﻿using System.Security.Cryptography;
using System.Text;

namespace EventsExpress.Db.Helpers
{
    public static class PasswordHasher
    {
        public static string GenerateHash(string password, string salt)
        {
            byte[] byteSourceText = Encoding.ASCII.GetBytes(salt + password);
            using var hashProvider = new SHA256Managed();
            byte[] byteHash = hashProvider.ComputeHash(byteSourceText);

            return Encoding.ASCII.GetString(byteHash);
        }

        public static string GenerateSalt()
        {
            using var provider = new RNGCryptoServiceProvider();
            byte[] salt = new byte[16];
            provider.GetBytes(salt);

            return Encoding.ASCII.GetString(salt);
        }
    }
}
