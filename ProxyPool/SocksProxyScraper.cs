using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool
{
    internal class SocksProxyScraper
    {
        public static async Task Start()
        {
            string url = "https://list.proxylistplus.com/Socks-List-1";
            var proxies = await ScrapeProxiesAsync(url);

            _ = WriteProxiesToFileAsync(proxies, "ProxyPool.txt");

            Console.WriteLine("可用 SOCKS 代理已写入 ProxyPool.txt");
            /*foreach (var proxy in proxies)
            {
                Console.WriteLine(proxy);
            }*/
        }



        private static async Task<string[]> ScrapeProxiesAsync(string url)
        {
            // 创建一个 HttpClientHandler，并启用自动使用系统代理
           /* var handler = new HttpClientHandler
            {
                UseProxy = true,
                Proxy = WebRequest.GetSystemWebProxy(),
                PreAuthenticate = true
            };*/

            using (var httpClient = new HttpClient())
            {
                // 设置请求头
               //httpClient.DefaultRequestHeaders.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
               //httpClient.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate, br, zstd");
               //httpClient.DefaultRequestHeaders.Add("accept-language", "zh-CN,zh;q=0.9");
                //httpClient.DefaultRequestHeaders.Add("cache-control", "max-age=0");
                httpClient.DefaultRequestHeaders.Add("cookie", "_ga=GA1.2.284227443.1729834644; _gid=GA1.2.2075566057.1729834644; cf_clearance=sLq0PIMIW_uNiJzgG5SJIo6D.1.5uM_4ukLouE3fnu8-1729834643-1.2.1.1-2kOswFOWi0R6RRHuGi9stl.w3KYoKcMzamHh7DloqOPe28D94RVMfUwcbXnUoRl0QCZakZWVg8z_Sum9DzRMnv6RgqC3f2tUYhByZXkuFDZ2xzu6mbMvqVQRxcrMJDESgytgvkphVcY8A9Yx4MUtHDMwlw877cOEXbzbuJYgszBjmymMG0w0lEeaq7qhGCtqjcrvrw8_MuHmZSZMz2KEHmouXaDO6fxVMjn9yQuysyOcVRLcCzt5xOgrhEVYAaVHFgVj270y6VP2Piz.j2PHLY7_wonfJFaFGaVMnMKL22PxvoGYuLaaOSCKvtxf8x2_KKmEwpyagLAS8YzZcy9vNyEl62E0i6Ilrfuo8YSuGYfWJPLqKVFkfSm_Nyh3wpC7; _no_tracky_100814458=1; _ga_Z3MSCTK1RG=GS1.2.1729834644.1.0.1729834644.0.0.0");
                httpClient.DefaultRequestHeaders.Add("priority", "u=0, i");
                httpClient.DefaultRequestHeaders.Add("sec-ch-ua", "\"Chromium\";v=\"130\", \"Google Chrome\";v=\"130\", \"Not?A_Brand\";v=\"99\"");
                //httpClient.DefaultRequestHeaders.Add("sec-ch-ua-arch", "\"x86\"");
                //httpClient.DefaultRequestHeaders.Add("sec-ch-ua-bitness", "\"64\"");
                httpClient.DefaultRequestHeaders.Add("sec-ch-ua-full-version", "\"130.0.6723.59\"");
                httpClient.DefaultRequestHeaders.Add("sec-ch-ua-full-version-list", "\"Chromium\";v=\"130.0.6723.59\", \"Google Chrome\";v=\"130.0.6723.59\", \"Not?A_Brand\";v=\"99.0.0.0\"");
                //httpClient.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                //httpClient.DefaultRequestHeaders.Add("sec-ch-ua-model", "\"\"");
                httpClient.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
                httpClient.DefaultRequestHeaders.Add("sec-ch-ua-platform-version", "\"10.0.0\"");
                //httpClient.DefaultRequestHeaders.Add("sec-fetch-dest", "document");
                //httpClient.DefaultRequestHeaders.Add("sec-fetch-mode", "navigate");
                //httpClient.DefaultRequestHeaders.Add("sec-fetch-site", "none");
                //httpClient.DefaultRequestHeaders.Add("sec-fetch-user", "?1");
                httpClient.DefaultRequestHeaders.Add("upgrade-insecure-requests", "1");
                httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36");

                try
                {
                    var response = await httpClient.GetStringAsync(url);

                    var ipsAndPorts = ExtractIPsAndPorts(response);

                    var proxies = new List<string>();
                    foreach (var item in ipsAndPorts)
                    {
                        proxies.Add($"{item.Item1}:{item.Item2}");
                    }
                    return proxies.ToArray();
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"请求失败: {e.Message}");
                    return Array.Empty<string>();
                }
            }
        }

        static List<(string, string)> ExtractIPsAndPorts(string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);

            var rows = document.DocumentNode.SelectNodes("//tr[@class='cells']");
            var result = new List<(string, string)>();

            foreach (var row in rows)
            {
                var columns = row.SelectNodes("td");
                if (columns != null && columns.Count >= 3)
                {
                    string ip = columns[1].InnerText.Trim(); // IP 地址
                    string port = columns[2].InnerText.Trim(); // 端口
                    result.Add((ip, port));
                }
            }

            return result;
        }

        private static async Task WriteProxiesToFileAsync(IEnumerable<string> proxies, string filePath)
        {
            
            using (FileStream stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                foreach (var proxy in proxies)
                {
                    byte[] encodedText = Encoding.UTF8.GetBytes(proxy + Environment.NewLine); // 添加换行符
                    await stream.WriteAsync(encodedText, 0, encodedText.Length);
                }
            }
        }
    }
}
