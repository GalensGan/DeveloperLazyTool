using DeveloperLazyTool.Core.Modules;
using DeveloperLazyTool.Core.Options;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Core.Functions
{
    class Fn_Uninstall : FuncBase
    {
        private ILog _logger = LogManager.GetLogger(typeof(Fn_Uninstall));

        private OptionBase _option;
        public Fn_Uninstall(StdInOut stdInOut) : base(stdInOut)
        {
            _option = stdInOut.CmdOptions;
        }

        public override StdInOut Run()
        {
            // 删除用户 path 中添加的变量
            string userPath = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.User);
            if (userPath.Contains(_option.BaseDir))
            {
                _logger.Info("清除环境变量...");

                userPath = userPath.Replace(";" + _option.BaseDir, "");
                Environment.SetEnvironmentVariable("path", userPath, EnvironmentVariableTarget.User);

                _logger.Info("环境变量清除成功！");
            }

            // 判断当前命令行位置
            _logger.Info($"卸载成功！运行下列语句彻底删除：\n cd .. & rmdir /s/q \"{_option.BaseDir.Trim('\\')}\"");

            return null;
        }
    }
}
