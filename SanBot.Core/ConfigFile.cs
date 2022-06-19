using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SanBot.Core
{
    public class ConfigFile
    {
        public string Username
        {
            get
            {
                return credentials.UserName;
            }
            set
            {
                credentials = new NetworkCredential(value, Password);
            }
        }

        public string Password
        {
            get
            {
                return credentials.Password;
            }
            set
            {
                credentials = new NetworkCredential(Username, value);
            }
        }

        private NetworkCredential credentials = new NetworkCredential();
    }
}
