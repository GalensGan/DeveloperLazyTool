using CommandLine;
using DeveloperLazyTool.Functions;
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
        private static JObject _jObject;
        private ILog _logger = LogManager.GetLogger(typeof(OptionBase));

        public OptionBase()
        {
            // 读取配置文件
            if (_jObject == null) {
                // 从本机读取
                string fileFullName = $"{System.AppDomain.CurrentDomain.BaseDirectory}Config\\config.json";

                _logger.Debug($"配置文件位置：{fileFullName}");

                if (!File.Exists(fileFullName)) {

                    _logger.Error("配置文件不存在");
                    return;
                }

                // 读取配置文件
                string configStr = new StreamReader(fileFullName,Encoding.Default).ReadToEnd();
                _jObject = JsonConvert.DeserializeObject<JObject>(configStr);
            }
        }
        public JObject JObject => _jObject;
        protected virtual bool BeforeCommand() {
            // 检查配置文件
            if (_jObject == null) {
                // 提示有问题
                _logger.Error("未读取到配置文件");
                return false;
            }

            return true;
        }

        protected virtual void RunningCommand()
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
            funcBase.Run();
        }
        protected virtual void AfterCommand() { }
        public void RunCommand() {
            if (!BeforeCommand()) return;

            RunningCommand();

            AfterCommand();
        }
    }
}
