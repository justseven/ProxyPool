using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Model
{
    public class Options
    {
        [Option('u', "update", Required = false, HelpText = "是否自动更新代理。")]
        public bool IsUpdate { get; set; }

        [Option('k', "fofaKey", Required = false, HelpText = "FofaKey")]
        public string FofaKey { get; set; }
    }
}
