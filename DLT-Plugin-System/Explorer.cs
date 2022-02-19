using CommandLine;
using DeveloperLazyTool.Plugin;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowToolAPI.Utils.Core;

namespace DLTPlugins.System
{
    [Verb("exp", HelpText = "打开当前目录或者文件")]
    internal class Explorer : CustomPlugin
    {
        private ILog _logger = LogManager.GetLogger(typeof(Explorer));

        public override IResult<JToken> RunOneCommand(JObject verbSetting)
        {
            // 获取目录
            var fullPath = Environment.CurrentDirectory;
            if (!string.IsNullOrEmpty(Name))
            {
                fullPath = Path.Combine(fullPath, Name);
            }

            if (File.Exists(fullPath))
            {
                // 用默认程序打开文件
                Process.Start(fullPath);

                return new SuccessResult();
            }

            if (Directory.Exists(fullPath))
            {
                Process.Start("explorer.exe",fullPath);
                return new SuccessResult();
            }

            var message = "文件或目录不存在";
            _logger.Error(message);
            return new ErrorResult(message);
        }
    }
}
