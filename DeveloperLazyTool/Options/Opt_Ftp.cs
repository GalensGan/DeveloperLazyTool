using CommandLine;
using DeveloperLazyTool.Functions;
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
        [Option('n',HelpText = "上传的配置名称", Required = false)]
        public string Name { get; set; }

        [Option('q', HelpText = "是否提示确认，默认为 true", Default = false)]
        public bool Quiet { get; set; }

        public JObject JObject { get; private set; }

        public override bool InitRuningArgs(JObject jObject)
        {
            base.InitRuningArgs(jObject);

            JObject = jObject;

            return true;
        }
    }
}
