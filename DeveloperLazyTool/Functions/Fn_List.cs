using DeveloperLazyTool.Modules;
using DeveloperLazyTool.Options;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Functions
{
    class Fn_List : FuncBase
    {
        private ILog _logger = LogManager.GetLogger(typeof(Fn_Ftp));

        public override StdInOut Run()
        {
            // 判断名称是否为空
            if (string.IsNullOrEmpty(Option.Name))
            {
                _logger.Error("请输入查看的配置名称");
                return null;
            }

            // 根据名称找到配置并输出
            JArray jArray = Option.Argument.JObjUser.Value<JArray>(Option.Name);
            if (jArray==null)
            {
                _logger.Error($"当前配置文件中没有名为：{Option.Name}的配置");
                return null;
            }

            // 找到所有的 name            
            foreach(JToken jToken in jArray)
            {
                string name = jToken.Value<string>("name");
                if (string.IsNullOrEmpty(name)) continue;

                Console.WriteLine(name);
            }

            return null;
        }
    }
}
