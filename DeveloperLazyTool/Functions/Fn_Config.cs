using DeveloperLazyTool.Modules;
using DeveloperLazyTool.Options;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Functions
{
    /// <summary>
    /// 打开配置文件位置
    /// 需要传递的参数如下
    /// </summary>
    class Fn_Config : FuncBase
    {
        private Opt_Config _option;
        public Fn_Config(StdInOut inputConfig) : base(inputConfig)
        {
            // 初始化需要的参数
            _option = InputParams.CmdOptions as Opt_Config;
        }

        public override StdInOut Run()
        {
            if (_option.User)
            {
                string fileFullName = Path.Combine(_option.BaseDir, _option.PathData, _option.UserConfigName);
               Process.Start(fileFullName);
            }

            if (_option.System)
            {
                string fileFullName = Path.Combine(_option.BaseDir, _option.SystemConfigPath);
                Process.Start(fileFullName);
            }

            if (_option.ScriptDir)
            {
                // 打开脚本目录
                string fileFullName = Path.Combine(_option.BaseDir, _option.PathScript);
                Process.Start("explorer.exe", fileFullName);
            }

            if (_option.SetupDir)
            {
                Process.Start("explorer.exe", _option.BaseDir);
            }

            // 返回成功值
            return InputParams;
        }
    }
}
