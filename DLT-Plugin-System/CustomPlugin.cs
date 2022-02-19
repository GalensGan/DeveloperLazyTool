using DeveloperLazyTool.Plugin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowToolAPI.Utils.Core;

namespace DLTPlugins.System
{
    public abstract class CustomPlugin:PluginBase
    {
        // 不检查是否存在设置
        protected override IResult<JToken> IsExistVerbSetting()
        {
            return new SuccessResult();
        }

        // 重写基础逻辑
        protected override IResult<JToken> RunAllCommands()
        {
            return RunOneCommand(ConfigContainer.VerbSetting as JObject);
        }
    }
}
