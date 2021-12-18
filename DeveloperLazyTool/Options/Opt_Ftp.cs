using CommandLine;
using DeveloperLazyTool.Enums;
using DeveloperLazyTool.Extensions;
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
        protected override JArray GetAllCmdConfigs()
        {
            // 获取用户的ftp配置,传入整个 ftp 配置
            return UserConfig.SelectTokenPlus(FieldNames.ftps, new JArray());
        }
    }
}
