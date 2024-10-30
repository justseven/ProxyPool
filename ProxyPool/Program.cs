using CommandLine;
using ProxyPool;
using ProxyPool.Model;
using ProxyPool.UpdateProxies;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        await Parser.Default.ParseArguments<Options>(args)
            .WithParsedAsync(async options => await Run(options));
    }

    private static async Task Run(Options options)
    {
        try
        {
            if (options.IsUpdate)
            {
                Console.WriteLine("Updating proxies...");
                await Fofa.Start(); // 执行更新逻辑
                Console.WriteLine("Proxy update completed.");
            }

            Console.WriteLine("Starting Socks5 Proxy Server with Pool...");
            await Socks5ProxyServerWithPoolAsync.Start(); // 启动代理池服务器
            Console.WriteLine("Socks5 Proxy Server started.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"发生错误: {ex.Message}");
        }
    }
}
