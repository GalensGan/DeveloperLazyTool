using CommandLine;
using DeveloperLazyTool.Plugin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowToolAPI.Utils.Core;
using WowToolAPI.Utils.Extensions;

namespace DLTPlugins.System
{
    [Verb("config", HelpText = "用默认软件打开配置文件")]
    public class Config : PluginBase
    {
        [Option('u',"user",HelpText = "用户配置")]
        public bool User { get; set; }

        [Option('s',"system", HelpText = "系统配置")]
        public bool System { get; set; }

        [Option(HelpText = "打开程序安装目录")]
        public bool SetupDir { get; set; }

        public override IResult<JToken> RunOneCommand(JObject verbSetting)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            if (User)
            {
                // 获取userConfigName
                var userConfigName = ConfigContainer.SystemSetting.SelectTokenPlus("userConfigName", string.Empty);
                string fileFullName = Path.Combine(baseDir, userConfigName);
                Process.Start(fileFullName);
            }

            if (System)
            {
                string fileFullName = Path.Combine(baseDir,"config/config.json");
                Process.Start(fileFullName);
            }

            if (SetupDir)
            {
                Process.Start("explorer.exe",baseDir);
            }

            // 返回成功值
            return new SuccessResult();
        }
    }
}
