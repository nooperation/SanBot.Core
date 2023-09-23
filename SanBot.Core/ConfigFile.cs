using System.Net;
using System.Security;
using System.Text.Json;

namespace SanBot.Core
{
    public class ConfigFile
    {
        private class ConfigFileInsecure
        {
            public string Username { get; set; } = default!;
            public string Password { get; set; } = default!;
        }

        public SecureString Username { get; private set; } = default!;
        public SecureString Password { get; private set; } = default!;

        public static ConfigFile FromJsonFile(string filePath)
        {
            var jsonString = File.ReadAllText(filePath);
            var insecureCredentials = JsonSerializer.Deserialize<ConfigFileInsecure>(jsonString, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            });

            if (insecureCredentials == null)
            {
                throw new Exception("Invalid config: " + filePath);
            }

            var credentials = new ConfigFile
            {
                Username = new NetworkCredential("", insecureCredentials.Username).SecurePassword,
                Password = new NetworkCredential("", insecureCredentials.Password).SecurePassword
            };

            return credentials;
        }
    }
}
