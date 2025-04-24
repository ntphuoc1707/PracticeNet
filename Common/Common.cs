using System.Security.Cryptography;
using System.Text;

namespace Utility
{
    public class Common
    {
        public static String HashData(String input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public class ServerInfoSetting
        {
            public int Port { get; set; }
            public string? ServerName { get; set; }
            public string ? HttpCertPath { get; set; }
            public string? HttpCertPass { get; set; }
        }
    }
}
