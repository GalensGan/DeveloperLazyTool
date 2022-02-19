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

namespace DLTPlugins.System
{ 
    [Verb("list", HelpText = "获取某个命令下配置项的名称")]
    public class List : CustomPlugin
    {
        private ILog _logger = LogManager.GetLogger(typeof(List));

        public override IResult<JToken> RunOneCommand(JObject verbSetting)
        {
            // 判断配置字段名称（动词对应的顶层字段）是否为空
            if (string.IsNullOrEmpty(Name))
            {
                var message = "请输入要查看的配置名称";
                _logger.Error(message);
                return new ErrorResult(message);
            }

            // 根据名称找到配置并输出
            JArray jArray = ConfigContainer.AllVerbsSetting.Value<JArray>(Name.ToLower());
            if (jArray == null || jArray.Count<1)
            {
                _logger.Error($"当前配置文件中没有名为：{Name}的配置");
                return null;
            }

            // 找到所有的 name     
            Console.WriteLine($"{Name} 包含{jArray.Count}项配置，分别是：");
            foreach (JToken jToken in jArray)
            {
                string name = jToken.Value<string>("name");
                if (string.IsNullOrEmpty(name)) continue;

                Console.WriteLine("  -{0}",name);
            }

            return new SuccessResult();
        }
    }
}
