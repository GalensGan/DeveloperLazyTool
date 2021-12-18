using CommandLine;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Options
{
    [Verb("install", HelpText = "初始化安装")]
    public class Opt_Install : OptionBase
    {
        protected override JArray GetAllCmdConfigs()
        {
            return null;
        }
    }
}
