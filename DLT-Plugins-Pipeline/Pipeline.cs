using CommandLine;
using DeveloperLazyTool.Plugin;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowToolAPI.Utils.Core;

namespace DLTPlugins.Pipeline
{
    [Verb("pipe", HelpText = "运行管道数据")]
    public class Pipeline : PluginBase
    {
        private ILog _logger = LogManager.GetLogger(typeof(Pipeline));
        public override IResult<JToken> RunOneCommand(JObject verbSetting)
        {
            _logger.Error("暂未实现");
            return new SuccessResult();
        }
    }
}
