// See https://aka.ms/new-console-template for more information
using ProxyPool;
using ProxyPool.UpdateProxies;

//Console.WriteLine("Hello, World!");

try
{

    //await SocksProxyScraper.Start();
    //await Fofa.Start();
    await Socks5ProxyServerWithPoolAsync.Start();
    
}
catch (Exception ex)
{
    Console.WriteLine($"发生错误: {ex.Message}");
}

