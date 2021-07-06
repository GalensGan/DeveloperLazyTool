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

        [Option('b', "background", HelpText = "是否在后台运行", Default = false)]
        public bool Background { get; set; }

        [Option('p', "params", HelpText = "动态参数")]
        public IEnumerable<string> Params { get; set; }

        protected override void ResolveArgumenets(ArgumentFactory factory)
        {
            Argument = factory.GetNamedArguments(Enums.FieldNames.scripts.ToString());
        }
    }
}
