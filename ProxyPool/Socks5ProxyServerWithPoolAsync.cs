using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ProxyPool
{
    public class Socks5ProxyServerWithPoolAsync
    {
        private const int listenPort = 8215;
        private static List<(string IP, int Port)> proxyPool = new List<(string, int)>();
        private static (string IP, int Port) currentProxy;
        public static async Task Start()
        {
            // 从文件加载代理池并验证代理可用性
            await LoadAndValidateProxyPoolAsync("ProxyPool.txt");

            TcpListener listener = new TcpListener(IPAddress.Any, listenPort);
            listener.Start();
            Console.WriteLine($"Listening on port {listenPort}...");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                SwitchProxy();
                _ = Task.Run(() => HandleClient(client, currentProxy.IP, currentProxy.Port));
            }
        }
        /// <summary>
        /// 读取代理文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>

        private static async Task LoadAndValidateProxyPoolAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("代理池文件未找到，使用空的代理池。");
                return;
            }

            var validProxies = new List<(string, int)>();
            string[] lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                var parts = line.Split(':');
                if (parts.Length == 2 &&
                    IPAddress.TryParse(parts[0], out var ip) &&
                    int.TryParse(parts[1], out var port))
                {
                    // 验证代理可用性
                    if (await IsProxyAvailable(ip.ToString(), port))
                    {
                        validProxies.Add((ip.ToString(), port));
                        Console.WriteLine($"可用代理：{ip}:{port}");
                    }
                    else
                    {
                        Console.WriteLine($"不可用代理：{ip}:{port}");
                    }
                }
            }

            // 更新代理池为可用的代理
            proxyPool = validProxies;
            await UpdateProxyPoolFileAsync(filePath, validProxies);
        }

        private static async Task<bool> IsProxyAvailable(string ip, int port)
        {
            try
            {
                var handler = new HttpClientHandler
                {
                    Proxy = new WebProxy(ip, port),
                    UseProxy = true
                };

                using (var client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromSeconds(3); // 设置超时

                    // 发送请求
                    var response = await client.GetAsync("http://ifconfig.me");

                    return response.IsSuccessStatusCode; // 返回请求是否成功
                }
            }
            catch
            {
                return false; // 如果出现异常，返回不可用
            }
        }

        private static async Task UpdateProxyPoolFileAsync(string filePath, List<(string, int)> validProxies)
        {
            var validProxyLines = new List<string>();
            foreach (var proxy in validProxies)
            {
                validProxyLines.Add($"{proxy.Item1}:{proxy.Item2}");
            }

            // 写回文件，仅包含可用的代理
            await AppendTextAsync(filePath, validProxyLines);
        }

        private static async Task AppendTextAsync(string filePath, List<string> contentList)
        {
            // 使用FileStream异步写入文件，并确保是追加模式
            using (FileStream stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                foreach (var content in contentList)
                {
                    byte[] encodedText = Encoding.UTF8.GetBytes(content + Environment.NewLine); // 添加换行符
                    await stream.WriteAsync(encodedText, 0, encodedText.Length);
                }
            }
        }

        private static void SwitchProxy()
        {
            if (proxyPool.Count > 0)
            {
                currentProxy = SelectRandomProxy();
                Console.WriteLine($"代理服务器已更换为：{currentProxy.IP}:{currentProxy.Port}");
            }
        }

        private static (string IP, int Port) SelectRandomProxy()
        {
            if (proxyPool.Count == 0)
            {
                throw new InvalidOperationException("代理池为空，无法选择代理。");
            }
            Random rand = new Random();
            int index = rand.Next(proxyPool.Count);
            return proxyPool[index];
        }

        static async Task HandleClient(TcpClient client, string targetIp, int targetPort)
        {
            using (client)
            {
                try
                {
                    Console.WriteLine("Client connected.");

                    // 连接到目标服务器
                    using TcpClient targetServer = new TcpClient();
                    await targetServer.ConnectAsync(targetIp, targetPort);
                    Console.WriteLine($"Connected to target server at {targetIp}:{targetPort}");

                    // 获取客户端和目标服务器的网络流
                    NetworkStream clientStream = client.GetStream();
                    NetworkStream targetStream = targetServer.GetStream();

                    // 双向数据转发
                    Task clientToTarget = TransferData(clientStream, targetStream);
                    Task targetToClient = TransferData(targetStream, clientStream);

                    // 等待双向转发完成
                    await Task.WhenAny(clientToTarget, targetToClient);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    Console.WriteLine("Client disconnected.");
                }
            }
        }

        static async Task TransferData(NetworkStream input, NetworkStream output)
        {
            byte[] buffer = new byte[8192];
            int bytesRead;

            while ((bytesRead = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await output.WriteAsync(buffer, 0, bytesRead);
                await output.FlushAsync();
            }
        }
    }
}
