using CommandLine;
using DeveloperLazyTool.Functions;
using DeveloperLazyTool.Modules;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Options
{
    [Verb("ftp", HelpText = "上传文件或者文件夹到ftp")]
    public class Opt_Ftp : OptionBase
    {
        //[Value(0,HelpText = "上传的配置名称", Required = false)]
        //public string Name { get; set; }

        [Option('q',"quiet", HelpText = "是否提示确认", Default = false)]
        public bool Quiet { get; set; }

        protected override void ResolveArgumenets(ArgumentFactory factory)
        {
            Argument = factory.GetFtpArguments();
        }
    }
}
