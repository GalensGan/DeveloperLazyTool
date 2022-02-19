using CommandLine;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Core.Options
{
    [Verb("uninstall", HelpText = "卸载程序")]
    class Opt_Uninstall : OptionBase
    {
        protected override JArray GetAllCmdConfigs()
        {
            return null;
        }
    }
}
