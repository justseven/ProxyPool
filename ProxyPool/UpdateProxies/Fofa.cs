using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProxyPool.UpdateProxies
{
    public class Fofa
    {
        private const string FofaKey = "Your Fofa Api"; // 替换为你的 Fofa API 密钥
        private const string Query = "protocol==\"socks5\" && country=\"CN\" && banner=\"Method:No Authentication\""; // 搜索 SOCKS5 代理的查询条件

        public static async Task Start()
        {
            try
            {
                var proxies = await SearchSocks5ProxiesAsync();
                await SaveProxiesToFileAsync(proxies, "ProxyPool.txt");
                Console.WriteLine("代理地址已保存到 ProxyPool.txt");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
            }
        }

        private static async Task<string[]> SearchSocks5ProxiesAsync()
        {
            using var httpClient = new HttpClient();
            // 将查询条件进行 Base64 编码
            var encodedQuery = Convert.ToBase64String(Encoding.UTF8.GetBytes(Query));
            string url = $"https://fofa.info/api/v1/search/all?key={FofaKey}&qbase64={encodedQuery}&fields=ip,port&size=300";
            
            var response = await httpClient.GetStringAsync(url);
            var json = JObject.Parse(response);
            if (json.ContainsKey("error") && json["error"].Value<bool>())
            {
                throw new Exception($"Error: {json["errmsg"].ToString()}");
            }
            // 从 JSON 中提取 IP 和端口
            var results = json["results"];
            var proxies = new string[results.Count()];

            for (int i = 0; i < results.Count(); i++)
            {
                string ip = results[i][0].ToString();
                string port = results[i][1].ToString();
                proxies[i] = $"{ip}:{port}";
            }

            return proxies;
        }

        private static async Task SaveProxiesToFileAsync(string[] proxies, string filePath)
        {
            // 使用 FileStream 和 StreamWriter 以追加模式写入文件
            using (FileStream stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            {
                foreach (var proxy in proxies)
                {
                    await writer.WriteLineAsync(proxy);
                }
            }
        }

    }
}
