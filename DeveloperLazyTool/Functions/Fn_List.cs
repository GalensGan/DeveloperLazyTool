using DeveloperLazyTool.Enums;
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
    /// <summary>
    /// 查找某个动词的所有用户配置
    /// </summary>
    class Fn_List : FuncBase
    {
        private ILog _logger = LogManager.GetLogger(typeof(Fn_Ftp));
        private Opt_List _option;

        public Fn_List(StdInOut stdInOut) : base(stdInOut)
        {
            _option = stdInOut.CmdOptions as Opt_List;
        }

        public override StdInOut Run()
        {
            // 判断配置字段名称（动词对应的顶层字段）是否为空
            if (string.IsNullOrEmpty(_option.Name))
            {
                _logger.Error("请输入查看的配置名称");
                return null;
            }

            // 根据名称找到配置并输出
            JArray jArray = InputParams.AllUserConfigs.Value<JArray>(_option.Name);
            if (jArray==null)
            {
                _logger.Error($"当前配置文件中没有名为：{_option.Name}的配置");
                return null;
            }

            // 找到所有的 name            
            foreach(JToken jToken in jArray)
            {
                string name = jToken.Value<string>(FieldNames.name);
                if (string.IsNullOrEmpty(name)) continue;

                Console.WriteLine(name);
            }

            return null;
        }
    }
}
