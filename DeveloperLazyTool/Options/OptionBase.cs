using CommandLine;
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
        #region 选项
        [Value(0, HelpText = "名称", Required = false)]
        public virtual string Name { get; set; }
        #endregion
        private ILog _logger = LogManager.GetLogger(typeof(OptionBase));

        #region 配置变量
        private JObject _jObjSystemConfig = null;

        public string SystemConfigPath => "config\\config.json";

        public string BaseDir { get; private set; }
        /// <summary>
        /// 系统 data 路径
        /// </summary>
        public string PathData { get; private set; }
        /// <summary>
        /// 系统 script 路径
        /// </summary>
        public string PathScript { get; private set; }
        /// <summary>
        /// 系统 system 路径
        /// </summary>
        public string PathSystem { get; private set; }

        /// <summary>
        /// 用户配置文件名称
        /// </summary>
        public string UserConfigName { get; set; }
        #endregion

        public OptionBase()
        {
            BaseDir = System.AppDomain.CurrentDomain.BaseDirectory;

            // 读取系统配置文件
            if (_jObjSystemConfig == null)
            {
                // 从本机读取
                string fileFullName = Path.Combine(BaseDir, SystemConfigPath);

                if (!File.Exists(fileFullName))
                {
                    _logger.Error("系统配置文件丢失");
                    return;
                }

                // 读取配置文件
                string configStr = new StreamReader(fileFullName, Encoding.Default).ReadToEnd();
                _jObjSystemConfig = JsonConvert.DeserializeObject<JObject>(configStr);

                // 加载配置文件
                LoadPath(_jObjSystemConfig);
            }
        }

        // 加载配置文件
        private void LoadPath(JObject jObject)
        {
            JObject jPath = jObject.Value<JObject>("systemPath");
            PathData = jPath["data"].ToString();
            PathScript = jPath["script"].ToString();
            PathSystem = jPath["system"].ToString();
            UserConfigName = jPath["userConfigName"].ToString();
        }

        public Argument Argument { get; protected set; }

        protected virtual bool BeforeFunction() {
            // 检查配置文件
            return true;
        }

        protected virtual void StartFunction()
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
            if (type==null)
            {
                _logger.Error($"未找到 {subClassName} 对应的功能");
                return;
            }

            FuncBase  funcBase = Activator.CreateInstance(type) as FuncBase;
            funcBase.SetParams(this);
            try
            {
                funcBase.Run();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        protected virtual void AfterFunction() { }
        public void RunFunction() {
            ResolveArgumenets(_argumentFactory);

            if (!BeforeFunction()) return;

            StartFunction();

            AfterFunction();
        }

        private ArgumentFactory _argumentFactory = null;

        public void SetArgumentFactory(ArgumentFactory factory)
        {
            _argumentFactory = factory;
        }

        protected virtual void ResolveArgumenets(ArgumentFactory factory) { }
    }
}
