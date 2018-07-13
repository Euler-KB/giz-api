using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace GIZ.API.Helpers
{
    public static class UserTokensHelper
    {
        public static readonly byte[] Secret;

        static UserTokensHelper()
        {
            Secret = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["USER_TOKEN_SECRET"]);
        }

        public static string GenerateToken(int length)
        {
            return string.Concat( Guid.NewGuid().ToString("N").OrderBy(x => new Guid()).Take(Math.Min( length, 32)) ).ToUpper();
        }

        public static string ProtectToken(string rawToken, out string salt)
        {
            byte[] saltBlob = new byte[32];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
                rng.GetBytes(saltBlob);

            byte[] keyHash;
            using (var hmac = new System.Security.Cryptography.HMACSHA256(Secret))
                keyHash = hmac.ComputeHash(saltBlob);

            salt = Convert.ToBase64String(saltBlob);

            using (var aes = new System.Security.Cryptography.AesCryptoServiceProvider())
            {
                aes.GenerateIV();
                aes.Key = keyHash;
                using (var encryptor = aes.CreateEncryptor())
                {
                    var inputBuffer = Encoding.UTF8.GetBytes(rawToken);
                    var cipher = encryptor.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);

                    using (var ms = new MemoryStream())
                    {
                        using (var binaryWriter = new BinaryWriter(ms))
                        {
                            binaryWriter.Write(aes.IV.Length);
                            binaryWriter.Write(aes.IV);
                            binaryWriter.Write(cipher.Length);
                            binaryWriter.Write(cipher);
                            binaryWriter.Flush();
                        }

                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }

        public static string UnProtectToken(string token, string salt)
        {
            byte[] saltBlob = Convert.FromBase64String(salt);

            byte[] keyHash;
            using (var hmac = new System.Security.Cryptography.HMACSHA256(Secret))
                keyHash = hmac.ComputeHash(saltBlob);

            using (var aes = new System.Security.Cryptography.AesCryptoServiceProvider())
            {
                aes.Key = keyHash;

                var inputBuffer = Convert.FromBase64String(token);
                byte[] cipherBlob;
                using (var br = new BinaryReader(new MemoryStream(inputBuffer)))
                {
                    aes.IV = br.ReadBytes(br.ReadInt32());
                    cipherBlob = br.ReadBytes(br.ReadInt32());
                }

                using (var decryptor = aes.CreateDecryptor())
                {
                    return Encoding.UTF8.GetString(decryptor.TransformFinalBlock(cipherBlob, 0, cipherBlob.Length));
                }
            }

        }
    }
}