using CommandLine;
using DeveloperLazyTool.Modules;
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
    [Verb("agg", HelpText = "聚合多步操作")]
    class Opt_Aggregate: OptionBase
    {
        [Value(0, HelpText = "名称", Required = false)]
        public override string Name { get; set; }

        protected override void ResolveArgumenets(ArgumentFactory factory)
        {
            Argument = factory.GetNamedArguments(Enums.FieldNames.aggregate.ToString());
        }
    }
}
