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
    /// 可以运行多个
    /// </summary>
    public abstract class ArrayFuncBase:FuncBase
    {
        private ILog _logger = LogManager.GetLogger(typeof(ArrayFuncBase));

        protected virtual bool BeforeRuning()
        {
            return true;
        }

        protected abstract string GetRuningName();

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
                // 调用上传模块
                jArray.ToList().ForEach(jt =>
                {
                    var argTemp = RunOne(jt);
                    Option.Argument.AddNext(argTemp);
                });
            }
            else
            {
                // 找到指定name的配置
                var jtoken = jArray.ToList().Find(jt => jt.Value<string>("name").ToLower() == GetRuningName().ToLower());
                if (jtoken != null)
                {
                    var argTemp = RunOne(jtoken);
                    Option.Argument.AddNext(argTemp);
                }
                else
                {
                    // 提示错误
                    _logger.Error($"未找到{ GetRuningName()}的配置");
                }
            }

            return Option.Argument.Last;
        }

        protected abstract Argument RunOne(JToken jToken);
    }
}
