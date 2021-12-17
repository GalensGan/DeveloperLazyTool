using DeveloperLazyTool.Modules;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Functions
{
    public class Fn_Install : FuncBase
    {
        private ILog _logger = LogManager.GetLogger(typeof(Fn_Ftp));
        public override StdInOut Run()
        {
            // 给用户 path 变量中加上当前目录
            string userPath = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.User);
            if (!userPath.Contains(Option.BaseDir))
            {
                userPath += $";{Option.BaseDir}";
                Environment.SetEnvironmentVariable("path", userPath, EnvironmentVariableTarget.User);

                _logger.Info("安装成功!");
                return null;
            }
            _logger.Info("已安装！");

            return null;
        }
    }
}
