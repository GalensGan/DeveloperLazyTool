using CommandLine;
using DeveloperLazyTool.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Options
{
    [Verb("es", true, HelpText = "运行脚本文件(Execute Script)")]
    public class Opt_Script : OptionBase
    {
        //[Value(0, HelpText = "上传的配置名称",Required =false)]
        //// [Option('n', "name", HelpText = "上传的配置名称", Required = false)]        
        //public string Name { get; set; }

        [Option('q', "quiet", HelpText = "是否提示确认", Default = false)]
        public bool Quiet { get; set; }

        protected override void ResolveArgumenets(ArgumentFactory factory)
        {
            Argument = factory.GetNamedArguments(Enums.FieldNames.scripts.ToString());
        }
    }
}
