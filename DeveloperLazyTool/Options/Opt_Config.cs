using CommandLine;
using DeveloperLazyTool.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Options
{
    [Verb("config", HelpText = "用默认软件打开配置文件")]
    public class Opt_Config : OptionBase
    {
        [Option('u',HelpText = "用户配置")]
        public bool User { get; set; }

        [Option('s', HelpText = "系统配置")]
        public bool System { get; set; }
    }
}
