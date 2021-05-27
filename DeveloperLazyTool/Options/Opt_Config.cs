using CommandLine;
using DeveloperLazyTool.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Options
{
    [Verb("config", HelpText = "上传文件或者文件夹到ftp")]
    public class Opt_Config : OptionBase
    {
    }
}
