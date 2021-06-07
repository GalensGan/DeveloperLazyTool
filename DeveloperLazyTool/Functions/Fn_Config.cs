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
    /// <summary>
    /// 打开配置文件位置
    /// </summary>
    class Fn_Config : FuncBase
    {
        private Opt_Config _option;
        public override void SetParams(OptionBase optionBase)
        {
            base.SetParams(optionBase);

            _option = ConvertParams<Opt_Config>();
        }
        public override Argument Run()
        {
            if (_option.User)
            {
                string fileFullName = Path.Combine(_option.BaseDir, _option.PathData, _option.UserConfigName);
                System.Diagnostics.Process.Start(fileFullName);
            }

            if (_option.System)
            {
                string fileFullName = Path.Combine(_option.BaseDir, _option.SystemConfigPath);
                System.Diagnostics.Process.Start(fileFullName);
            }
            return null;
        }
    }
}
