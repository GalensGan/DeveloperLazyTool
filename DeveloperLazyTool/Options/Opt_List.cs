using CommandLine;
using DeveloperLazyTool.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Options
{
    [Verb("list", HelpText = "获取配置名称")]
    class Opt_List:OptionBase
    {
        protected override void ResolveArgumenets(ArgumentFactory factory)
        {
            Argument = factory.GetListArguments();
        }
    }
}
