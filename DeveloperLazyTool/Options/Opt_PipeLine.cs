using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Options
{
    /// <summary>
    /// 可以将多个操作放在一个管道内，这样实现连续多个操作
    /// </summary>
    [Verb("pipeline", HelpText = "运行自定义管理")]
    class Opt_PipeLine:OptionBase
    {

        [Option(HelpText = "定义的管理名称", Required = true)]
        public string Name { get; set; }
    }
}
