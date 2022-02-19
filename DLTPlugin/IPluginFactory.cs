using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Plugin
{
    public interface IPluginFactory
    {
        /// <summary>
        /// 获取选项
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns></returns>
        PluginBase GetPlugin(string[] args);
    }
}
