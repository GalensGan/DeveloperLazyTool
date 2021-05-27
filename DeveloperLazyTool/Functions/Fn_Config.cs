using DeveloperLazyTool.Options;
using log4net;
using System;
using System.Collections.Generic;
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
        public override void Run()
        {
            string fileFullName = $"{System.AppDomain.CurrentDomain.BaseDirectory}Config\\config.json";
            System.Diagnostics.Process.Start(fileFullName);
        }
    }
}
