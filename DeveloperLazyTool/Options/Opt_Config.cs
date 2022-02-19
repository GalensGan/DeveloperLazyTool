using CommandLine;
using DeveloperLazyTool.Core.Enums;
using DeveloperLazyTool.Core.Functions;
using DeveloperLazyTool.Core.Modules;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Core.Options
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
       
        protected override JArray GetAllCmdConfigs()
        {
            // 获取config配置
            // 系统内部默认，所以返回空配置
            return null;
        }

        protected override void RunAllCommands(Type type)
        {
            var stdIn = new StdInOut(this, null);
            FuncBase funcBase = Activator.CreateInstance(type, stdIn) as FuncBase;
            funcBase.Run();
        }
    }
}
