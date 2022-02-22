using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Plugin
{
    /// <summary>
    /// 所有配置的容器
    /// </summary>
    public class ConfigContainer
    {
        public ConfigContainer(JToken systemSetting,JToken pluginSetting,JToken verbSetting,JToken allVerbsSetting)
        {
            SystemSetting= systemSetting;
            PluginSetting = pluginSetting;
            VerbSetting= verbSetting; 
            AllVerbsSetting= allVerbsSetting;
        }

        /// <summary>
        /// 系统设置
        /// </summary>
        public JToken SystemSetting { get; private set; }

        /// <summary>
        /// 插件设置
        /// </summary>
        public JToken PluginSetting { get; private set; }
        /// <summary>
        /// 当前命令设置
        /// 该值一般是一个数组
        /// </summary>
        public JToken VerbSetting { get; private set; }

        /// <summary>
        /// 所有命令设置
        /// </summary>
        public JToken AllVerbsSetting { get; private set; }
    }
}
