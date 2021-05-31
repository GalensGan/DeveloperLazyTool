using DeveloperLazyTool.Modules;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Functions
{
    class Fn_Uninstall : FuncBase
    {
        private ILog _logger = LogManager.GetLogger(typeof(Fn_Ftp));
        public override Argument Run()
        {
            // 删除用户 path 中添加的变量
            string userPath = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.User);
            if (userPath.Contains(Option.BaseDir))
            {
                _logger.Info("清除环境变量...");

                userPath = userPath.Replace(";" + Option.BaseDir, "");
                Environment.SetEnvironmentVariable("path", userPath, EnvironmentVariableTarget.User);

                _logger.Info("环境变量清除成功！");
            }

            // 判断当前命令行位置
            _logger.Info($"卸载成功！运行下列语句彻底删除：\n cd .. & rmdir /s/q \"{Option.BaseDir.Trim('\\')}\"");

            return null;
        }
    }
}
