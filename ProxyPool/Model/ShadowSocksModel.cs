using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProxyPool
{
    internal class ShadowSocksModel
    {
        public ShadowSocksModel(string name, string type, string server, int port, string cipher, string password, bool udp)
        {
            Name = name;
            Type = type;
            Server = server;
            Port = port;
            Cipher = cipher;
            Password = password;
            Udp = udp;
        }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("server")]
        public string Server { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("cipher")]
        public string Cipher { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("udp")]
        public bool Udp { get; set; }
    }
}
