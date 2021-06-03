using DeveloperLazyTool.Modules;
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
    /// 只能运行一个
    /// </summary>
    public abstract class SingleFuncBase : ArrayFuncBase
    {
        private ILog _logger = LogManager.GetLogger(typeof(ArrayFuncBase));

        public override Argument Run()
        {
            if (!BeforeRuning())
            {
                return null;
            }

            // 找到所有操作
            JArray jArray = Option.Argument.InputData;
            if (jArray == null || jArray.Count < 1)
            {
                _logger.Info("ftps 配置为空");
                return null;
            }

            // 获取bat文件路径
            // 如果传入了 name,则执行特定的项
            if (string.IsNullOrEmpty(GetRuningName()))
            {
                _logger.Info("未输入 name,将运行第一个配置");
                // 调用第一个模块
                var jt = jArray.ToList().FirstOrDefault();
                var argTemp = RunOne(jt);
                Option.Argument.AddNext(argTemp);
            }
            else
            {
                // 找到指定name的配置
                var jtoken = jArray.ToList().Find(jt => jt.Value<string>("name") == GetRuningName());
                if (jtoken != null)
                {
                    var argTemp = RunOne(jtoken);
                    Option.Argument.AddNext(argTemp);
                }
            }

            return Option.Argument.Last;
        }
    }
}
