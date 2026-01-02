using System;
using System.Security.Cryptography;
using System.Text;

namespace Unity.UOS.Security
{
    public class EncryptManager
    {
        public static byte[] Sha256(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            using (SHA256 mySHA256 = SHA256.Create())
            {
                byte[] hash = mySHA256.ComputeHash(bytes);
                return hash;
            }
        }

        public static string HexString(byte[] data)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("X2").ToLower());
            }

            return builder.ToString();
        }

        public static long GetUnixTimeStampSeconds(DateTime dt)
        {
            DateTime dateStart = new DateTime(1970, 1, 1, 0, 0, 0);
            return Convert.ToInt64((dt - dateStart).TotalSeconds);
        }
    }
}