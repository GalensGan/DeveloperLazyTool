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
        [Option('u',"user",HelpText = "用户配置")]
        public bool User { get; set; }

        [Option('s',"system", HelpText = "系统配置")]
        public bool System { get; set; }

        [Option(HelpText = "打开脚本目录")]
        public bool ScriptDir { get; set; }

        [Option(HelpText = "打开程序安装目录")]
        public bool SetupDir { get; set; }
    }
}
