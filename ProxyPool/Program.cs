// See https://aka.ms/new-console-template for more information
using ProxyPool;

//Console.WriteLine("Hello, World!");

try
{
    
    //await SocksProxyScraper.Start();
    await Socks5ProxyServerWithPoolAsync.Start();
    
}
catch (Exception ex)
{
    Console.WriteLine($"发生错误: {ex.Message}");
}

