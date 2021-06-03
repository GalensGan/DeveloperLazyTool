using CommandLine;
using DeveloperLazyTool.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Options
{
    [Verb("ssh", HelpText = "ssh 连接")]
    class Opt_Ssh:OptionBase
    {
        //[Option('n', "name", HelpText = "ssh 连接的配置名称", Required = false)]
        //public string Name { get; set; }

        protected override void ResolveArgumenets(ArgumentFactory factory)
        {
            Argument = factory.GetNamedArguments(Enums.FieldNames.scripts.ToString());
        }
    }
}
