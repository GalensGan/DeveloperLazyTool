using CommandLine;
using DeveloperLazyTool.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Options
{
    [Verb("bat", HelpText = "打包文件")]
    public class Opt_Bat:OptionBase
    {
        [Option('n', "name", HelpText = "上传的配置名称", Required = false)]
        public string Name { get; set; }

        protected override void ResolveArgumenets(ArgumentFactory factory)
        {
            Argument = factory.GetNamedArguments(Enums.FieldNames.bats.ToString());
        }
    }
}
