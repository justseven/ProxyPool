using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Model
{
    public class ProxyInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }

        public ProxyInfo(string username, string password, string ip, int port)
        {
            Username = username;
            Password = password;
            IP = ip;
            Port = port;
        }
    }

}
