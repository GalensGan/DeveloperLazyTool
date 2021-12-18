using DeveloperLazyTool.Modules;
using DeveloperLazyTool.Options;
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
        private OptionBase _option;

        public Fn_Install(StdInOut stdInOut) : base(stdInOut)
        {
            _option = stdInOut.CmdOptions;
        }

        public override StdInOut Run()
        {
            // 给用户 path 变量中加上当前目录
            string userPath = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.User);
            if (!userPath.Contains(_option.BaseDir))
            {
                userPath += $";{_option.BaseDir}";
                Environment.SetEnvironmentVariable("path", userPath, EnvironmentVariableTarget.User);

                _logger.Info("安装成功!");
                return null;
            }
            _logger.Info("已安装！");

            return InputParams;
        }
    }
}
