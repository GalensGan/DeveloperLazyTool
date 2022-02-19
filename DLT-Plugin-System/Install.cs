using CommandLine;
using DeveloperLazyTool.Plugin;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using WowToolAPI.Utils.Core;

namespace DLTPlugins.System
{
    [Verb("install", HelpText = "初始化安装")]
    public class Install : CustomPlugin
    {
        private ILog _logger = LogManager.GetLogger(typeof(Install));

        public override IResult<JToken> RunOneCommand(JObject verbSetting)
        {
            // 给用户 path 变量中加上当前目录
            string userPath = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.User);

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

            if (!userPath.Contains(baseDir))
            {
                userPath += $";{baseDir}";
                Environment.SetEnvironmentVariable("path", userPath, EnvironmentVariableTarget.User);

                _logger.Info("安装成功!");
                return null;
            }
            _logger.Info("已安装！");

            return new SuccessResult();
        }
    }
}
