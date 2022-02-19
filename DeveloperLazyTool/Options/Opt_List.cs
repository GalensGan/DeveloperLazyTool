using CommandLine;
using DeveloperLazyTool.Core.Modules;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Core.Options
{
    [Verb("list", HelpText = "获取配置名称")]
    class Opt_List : OptionBase
    {
        protected override JArray GetAllCmdConfigs()
        {
            return null;
        }
    }
}
