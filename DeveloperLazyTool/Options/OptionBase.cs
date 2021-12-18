using CommandLine;
using DeveloperLazyTool.Enums;
using DeveloperLazyTool.Functions;
using DeveloperLazyTool.Modules;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Options
{
    public abstract class OptionBase
    {
        #region 所有动词都有的选项
        [Value(0, HelpText = "名称", Required = false)]
        public virtual string Name { get; set; }

        [Option('q', "quiet", HelpText = "是否提示确认", Default = false)]
        public bool Quiet { get; set; }
        #endregion

        private ILog _logger = LogManager.GetLogger(typeof(OptionBase));

        #region 系统配置变量
        // 系统配置
        protected JObject SystemConfig { get; private set; }

        // 用户配置（脚本定义,整个文件）
        protected JObject UserConfig { get; private set; }

        public string SystemConfigPath => "config\\config.json";

        public string BaseDir { get; private set; }
        /// <summary>
        /// 本系统 data 路径
        /// </summary>
        public string PathData { get; private set; }
        /// <summary>
        /// 本系统 script 路径
        /// </summary>
        public string PathScript { get; private set; }
        /// <summary>
        /// 本系统 system 路径
        /// </summary>
        public string PathSystem { get; private set; }
        /// <summary>
        /// 用户配置文件的名称
        /// </summary>
        public string UserConfigName { get; set; }
        #endregion

        public OptionBase()
        {
            // 加载系统配置文件
            LoadSystemConfig();

            // 加载脚本定义文件
            LoadScriptDefinitions();
        }

        // 加载配置文件
        private void LoadSystemConfig()
        {
            BaseDir = System.AppDomain.CurrentDomain.BaseDirectory;

            // 从本机读取
            string fileFullName = Path.Combine(BaseDir, SystemConfigPath);

            if (!File.Exists(fileFullName))
            {
                _logger.Error("系统配置文件丢失");
                throw new ArgumentNullException("系统配置文件丢失");
            }

            // 读取配置文件
            string configStr = new StreamReader(fileFullName, Encoding.Default).ReadToEnd();
            SystemConfig = JsonConvert.DeserializeObject<JObject>(configStr);

            JObject jPath = SystemConfig.Value<JObject>("systemPath");
            PathData = jPath["data"].ToString();
            PathScript = jPath["script"].ToString();
            PathSystem = jPath["system"].ToString();
            UserConfigName = jPath["userConfigName"].ToString();
        }

        // 加载脚本定义文件
        private void LoadScriptDefinitions()
        {
            // 读取用户配置文件
            // 从本机读取
            string fileFullName = Path.Combine(BaseDir, PathData, UserConfigName);

            _logger.Debug($"用户配置文件位置：{fileFullName}");

            if (!File.Exists(fileFullName))
            {
                _logger.Error("用户配置文件不存在");
                throw new ArgumentNullException("用户配置文件不存在");
            }

            // 读取配置文件
            string configStr = new StreamReader(fileFullName, Encoding.Default).ReadToEnd();
            UserConfig = JsonConvert.DeserializeObject<JObject>(configStr);
        }

        public void StartFunction()
        {
            // 用反射解耦
            string subClassName = this.GetType().Name;
            string[] names = subClassName.Split('_');
            if (names.Length != 2)
            {
                _logger.Error($"参数类名称格式错误，应为：Opt_xxx");
                return;
            }

            Type type = Type.GetType($"DeveloperLazyTool.Functions.Fn_{names[1]}");
            if (type == null)
            {
                _logger.Error($"未找到 {subClassName} 对应的功能");
                return;
            }

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
                    return;
                }
            }

            RunAllCommands(type);
        }

        protected virtual void RunAllCommands(Type type)
        {
            // 找到所有操作
            JArray jArray = GetAllCmdConfigs();
            if (jArray == null || jArray.Count < 1)
            {
                _logger.Info("当前命令配置为空");
                return;
            }

            try
            {
                // 获取bat文件路径
                // 没有传入名称，运行所有的项
                if (string.IsNullOrEmpty(Name))
                {
                    // 调用模块
                    RunCommands(type, jArray.ToList());
                }
                // 如果传入了 name, 则执行特定的项
                else
                {
                    // 找到指定name的配置
                    var jtokens = jArray.ToList().FindAll(jt => jt.Value<string>("name").ToLower() == Name.ToLower());
                    if (jtokens != null)
                    {
                        // 调用模块
                        RunCommands(type, jtokens);
                    }
                    else
                    {
                        // 提示错误
                        _logger.Error($"未找到{ Name }的配置");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void RunCommands(Type type,List<JToken> jTokens)
        {
            StdInOut lastStdData = null;
            // 调用上传模块
            jTokens.ToList().ForEach(jt =>
            {
                // 生成参数
                var stdIn = new StdInOut(this, jt as JObject);
                if (lastStdData != null)
                {
                    lastStdData.AddNext(stdIn);
                    lastStdData = stdIn;
                }

                FuncBase funcBase = Activator.CreateInstance(type, stdIn) as FuncBase;
                funcBase.Run();
            });
        }


        protected abstract JArray GetAllCmdConfigs();
    }
}
