using CommandLine;
using DeveloperLazyTool.Plugin;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowToolAPI.Utils.Core;

namespace DLTPlugins.System
{
    [Verb("uninstall", HelpText = "卸载程序")]
    class Uninstall : CustomPlugin
    {
        private ILog _logger = LogManager.GetLogger(typeof(Uninstall));

        public override IResult<JToken> RunOneCommand(JObject verbSetting)
        {
            // 删除用户 path 中添加的变量
            string userPath = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.User);
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

            if (userPath.Contains(baseDir))
            {
                _logger.Info("清除环境变量...");

                userPath = userPath.Replace(";" + baseDir, "");
                Environment.SetEnvironmentVariable("path", userPath, EnvironmentVariableTarget.User);

                _logger.Info("环境变量清除成功！");
            }

            // 判断当前命令行位置
            _logger.Info($"卸载成功！运行下列语句彻底删除：\n cd .. & rmdir /s/q \"{baseDir.Trim('\\')}\"");

            return new SuccessResult();
        }
    }
}
