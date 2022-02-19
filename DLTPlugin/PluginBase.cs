using CommandLine;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WowToolAPI.Utils.Core;

namespace DeveloperLazyTool.Plugin
{
    public abstract class PluginBase
    {
        #region 所有动词都有的选项
        [Value(0, HelpText = "名称", Required = false)]
        public virtual string Name { get; set; }

        [Option('q', "quiet", HelpText = "是否提示确认", Default = true)]
        public virtual bool Quiet { get; set; }

        /// <summary>
        /// 执行多个时，是否忽略错误
        /// </summary>
        [Option('c', "continue", HelpText = "运行多个命令配置时，发生错误后,是否继续", Default = false)]
        public virtual bool ContinueWhenError { get; set; } = false;
        #endregion

        private ILog _logger = LogManager.GetLogger(typeof(PluginBase));

        #region 保护方法，由外部注入参数
        protected ConfigContainer ConfigContainer { get; private set; }
        protected IPluginFactory PluginFactory { get; private set; }
        #endregion

        /// <summary>
        /// 注入参数
        /// </summary>
        /// <param name="configContainer"></param>
        /// <param name="pluginFactory"></param>
        public void SetParams(ConfigContainer configContainer, IPluginFactory pluginFactory)
        {
            ConfigContainer = configContainer;
            PluginFactory = pluginFactory;
        }

        public IResult<JToken> StartCommand()
        {
            // 检查是否静默运行
            var quietRes = QuietNotify();
            if (quietRes.NotOk) return quietRes;

            // 检查是否需要配置参数
            var checkRes = IsExistVerbSetting();
            if (checkRes.NotOk) return checkRes;

            return RunAllCommands();
        }

        /// <summary>
        /// 是否静默运行
        /// </summary>
        /// <returns></returns>
        protected virtual IResult<JToken> QuietNotify()
        {
            // 是否要提示
            if (!Quiet)
            {
                string message;
                if (string.IsNullOrEmpty(Name))
                {
                    message = "是否运行所有配置(Y/N，Default:Y) ?";
                }
                else
                {
                    message = $"是否运行配置 {Name}(Y/N，Default:Y) ?";
                }
                Console.WriteLine(message);
                string command = Console.ReadLine();
                if (!string.IsNullOrEmpty(command) && command.ToLower() != "y")
                {
                    _logger.Warn("取消上传");
                    return new ErrorResult("取消上传");
                }
            }

            return new SuccessResult();
        }

        /// <summary>
        /// 检查设置
        /// </summary>
        /// <returns></returns>
        protected virtual IResult<JToken> IsExistVerbSetting()
        {
            // 找到所有操作配置
            if (!(ConfigContainer.VerbSetting is JArray jArray) || jArray.Count < 1)
            {
                _logger.Error("当前命令配置为空");
                return new ErrorResult("当前命令配置为空");
            }

            return new SuccessResult(null);
        }

        /// <summary>
        /// 此处调用每一个命令
        /// </summary>
        protected virtual IResult<JToken> RunAllCommands()
        {
            IResult<JToken> lastResult = new ErrorResult("命令行没有返回结果");
            try
            {
                // 获取bat文件路径
                // 没有传入名称，运行所有的项
                var jArray =  ConfigContainer.VerbSetting as JArray;
                List<JToken> jTokens = jArray.ToList();
                if (!string.IsNullOrEmpty(Name))
                {
                    // 找到指定name的配置
                    jTokens = jArray.ToList().FindAll(jt => jt.Value<string>("name").ToLower() == Name.ToLower());
                }

                if (jTokens == null || jTokens.Count < 1)
                {
                    var message = $"未找到{ Name }的配置";
                    _logger.Error(message);
                    return new ErrorResult(message);
                }

                // 运行所有的命令
                foreach (JToken jToken in jTokens)
                {
                    lastResult = RunOneCommand(jToken as JObject);

                    // 错误时，不继续
                    if (lastResult.NotOk && !ContinueWhenError) continue;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // 返回最后一个结果
            return lastResult;
        }

        public abstract IResult<JToken> RunOneCommand(JObject verbSetting);
    }
}
