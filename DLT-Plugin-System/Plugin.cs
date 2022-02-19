using CommandLine;
using DeveloperLazyTool.Plugin;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowToolAPI.Utils.Core;
using WowToolAPI.Utils.Extensions;

namespace DLTPlugins.System
{
    /// <summary>
    /// 安装插件
    /// 此处用插件的名称作为文件名称
    /// </summary>
    [Verb("plugin", HelpText = "加载插件并启用")]
    internal class Plugin : CustomPlugin
    {
        [Option('a', "add", HelpText = "添加插件", SetName = "verb")]
        public bool Add { get; set; }

        [Option('d', "delete", HelpText = "删除插件", SetName = "verb")]
        public bool Delete { get; set; }

        [Option('t', "toggle", HelpText = "开/关插件", SetName = "verb")]
        public bool Toggle { get; set; }

        [Option('l', "list", HelpText = "查看已经安装的所有插件", SetName = "verb")]
        public bool List { get; set; }

        [Option('n', "nmae", HelpText = "插件名称")]
        public override string Name { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        [Option('p', "path", HelpText = "插件库文件路径")]
        public string DllPath { get; set; }

        [Option('e', "enable", HelpText = "是否启用插件", Default = true)]
        public bool IsEnable { get; set; }

        private ILog _logger = LogManager.GetLogger(typeof(Plugin));


        public override IResult<JToken> RunOneCommand(JObject verbSetting)
        {
            if (Add)
            {
                return AddPlugin(verbSetting);
            }

            if (Delete)
            {
                return DeletePlugin(verbSetting);
            }

            if (Toggle)
            {
                return TogglePlugin(verbSetting);
            }

            if (List)
            {
                return ListPluginState(verbSetting);
            }

            return new ErrorResult("输入查询方法");
        }

        // 添加配置
        private IResult<JToken> AddPlugin(JObject verbSetting)
        {
            // 判断文件名是否存在
            if (string.IsNullOrEmpty(Name))
            {
                var message = "必须输入插件名";
                _logger.Error(message);

                return new ErrorResult(message);
            }

            var pluginSettingsArr = ConfigContainer.SystemSetting.SelectTokenPlus("plugins", new JArray());
            // 判断插件名是否重复
            var existConfig = pluginSettingsArr.Where(jt => jt.Value<string>("name").ToString() == Name.ToUpper()).FirstOrDefault();
            if (existConfig != null)
            {
                Console.WriteLine($"{Name} 插件已经存在，是否覆盖？(Y/N，default:Y)");
                var confirm = Console.ReadLine();
                if (!string.IsNullOrEmpty(confirm) && confirm.ToLower() != "yes" && confirm.ToLower() != "y")
                {
                    return new ErrorResult("退出");
                }
            }

            // 判断是否有path
            if (string.IsNullOrEmpty(DllPath))
            {
                var message = "路径为空";
                _logger.Error(message);
                return new ErrorResult(message);
            }

            // 将文件复制到指定位置
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var workDir = Environment.CurrentDirectory;
            var dllFileFullName = Path.Combine(workDir, DllPath);
            // 判断文件是否存在
            if (!File.Exists(dllFileFullName))
            {
                var message = "文件不存在";
                _logger.Error(message);
                return new ErrorResult(message);
            }

            // 将文件复制到指定位置
            //var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            //var testDir = Environment.CurrentDirectory;

            // 生成目录
            var pluginDir = Path.Combine(baseDir, "plugins", Name.ToUpper());
            Directory.CreateDirectory(pluginDir);
            File.Copy(dllFileFullName, Path.Combine(pluginDir, Path.GetFileName(dllFileFullName)), true);

            // 向配置文件中添加信息
            // 如果存在，就更新设置
            // 如果不存在，就添加
            if (existConfig != null)
            {
                existConfig["enable"] = IsEnable;
            }
            else
            {
                pluginSettingsArr.Add(new JObject()
                {
                    { "name", Name.ToUpper() },
                    {
                        "enable",IsEnable
                    }
                });
            }

            // 保存
            ConfigContainer.SystemSetting["plugins"] = pluginSettingsArr;

            // 保存到文件
            var content = JsonConvert.SerializeObject(ConfigContainer.SystemSetting, Formatting.Indented);
            var configFilePath = Path.Combine(baseDir, "config/config.json");
            File.WriteAllText(configFilePath, content);

            Console.WriteLine("添加成功!");

            return new SuccessResult();
        }

        // 删除配置
        private IResult<JToken> DeletePlugin(JObject verbSetting)
        {
            if (string.IsNullOrEmpty(Name))
            {
                var message = "必须输入插件名";
                _logger.Error(message);

                return new ErrorResult(message);
            }

            var pluginSettingsArr = ConfigContainer.SystemSetting.SelectTokenPlus("plugins", new JArray());
            // 判断插件名是否重复
            var existConfig = pluginSettingsArr.Where(jt => jt.Value<string>("name").ToString() == Name.ToUpper()).FirstOrDefault();
            if (existConfig == null)
            {
                // 插件不存在
                var message = $"插件{Name}不存在";
                _logger.Error(message);
                return new ErrorResult(message);
            }

            // 开始删除插件
            pluginSettingsArr.Remove(existConfig);
            // 保存
            ConfigContainer.SystemSetting["plugins"] = pluginSettingsArr;

            // 保存到文件
            var content = JsonConvert.SerializeObject(ConfigContainer.SystemSetting, Formatting.Indented);
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var configFilePath = Path.Combine(baseDir, "config/config.json");
            File.WriteAllText(configFilePath, content);
            Console.WriteLine("删除成功!");

            return new SuccessResult();
        }

        // 开关配置
        private IResult<JToken> TogglePlugin(JObject verbSetting)
        {
            if (string.IsNullOrEmpty(Name))
            {
                var message = "必须输入插件名";
                _logger.Error(message);

                return new ErrorResult(message);
            }

            var pluginSettingsArr = ConfigContainer.SystemSetting.SelectTokenPlus("plugins", new JArray());
            // 判断插件名是否重复
            var existConfig = pluginSettingsArr.Where(jt => jt.Value<string>("name").ToString() == Name.ToUpper()).FirstOrDefault();
            if (existConfig == null)
            {
                // 插件不存在
                var message = $"插件{Name}不存在";
                _logger.Error(message);
                return new ErrorResult(message);
            }

            // 修改插件状态
            existConfig["enable"] = !bool.Parse(existConfig["enable"].ToString());
            // 保存
            ConfigContainer.SystemSetting["plugins"] = pluginSettingsArr;

            // 保存到文件
            var content = JsonConvert.SerializeObject(ConfigContainer.SystemSetting, Formatting.Indented);
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var configFilePath = Path.Combine(baseDir, "config/config.json");
            File.WriteAllText(configFilePath, content);
            Console.WriteLine("{0}--{1}", existConfig["name"], existConfig["enable"]);

            return new SuccessResult();
        }

        // 展示插件和其状态
        private IResult<JToken> ListPluginState(JObject verbSetting)
        {
            var pluginSettingsArr = ConfigContainer.SystemSetting.SelectTokenPlus("plugins", new JArray());
            Console.WriteLine("插件列表：");
            foreach (var pluginSetting in pluginSettingsArr)
            {
                Console.WriteLine("{0}--{1}", pluginSetting["name"], pluginSetting["enable"]);
            }

            return new SuccessResult();
        }
    }
}
