using CommandLine;
using DeveloperLazyTool.Core.Enums;
using DeveloperLazyTool.Core.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Core.Options
{
    [Verb("minio", HelpText = "向 MinIO 服务器上传对象")]
    class Opt_MinIO : OptionBase
    {
        [Option('p', "paths", HelpText = "传入的文件路径")]
        public IEnumerable<string> Paths { get; set; }

        protected override JArray GetAllCmdConfigs()
        {
            return UserConfig.SelectTokenPlus(FieldNames.minios, new JArray());
        }
    }
}
