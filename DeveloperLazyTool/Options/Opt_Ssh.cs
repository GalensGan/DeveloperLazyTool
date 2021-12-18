using CommandLine;
using DeveloperLazyTool.Modules;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Options
{
    [Verb("ssh", HelpText = "ssh 连接")]
    class Opt_Ssh : OptionBase
    {
        //[Option('n', "name", HelpText = "ssh 连接的配置名称", Required = false)]
        //public string Name { get; set; }
        protected override JArray GetAllCmdConfigs()
        {
            return null;
        }
    }
}
