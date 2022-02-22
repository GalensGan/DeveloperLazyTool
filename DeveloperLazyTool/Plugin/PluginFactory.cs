using CommandLine;
using DeveloperLazyTool.Core.Enums;
using DeveloperLazyTool.Core.Extensions;
using DeveloperLazyTool.Plugin;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Core.Plugin
{
    internal class PluginFactory : IPluginFactory
    {
        public static PluginFactory Instance
        {
            get
            {
                if (_instance == null) _instance = new PluginFactory();

                return _instance;
            }
        }

        private static PluginFactory _instance;


        public string _systemConfigPath => "config/config.json";
        private ILog _logger = LogManager.GetLogger(typeof(PluginFactory));

        // 系统配置文件
        private JToken _systemConfig;
        private JToken _verbsConfig;

        private List<Type> _verbTypes = new List<Type>();

        private PluginFactory()
        {
            // 读取全局设置
            LoadSystemConfig();

            // 根据配置加载插件
            LoadPlugins();

            // 获取当前程序集内的 Plugin Type
            LoadSystemPluginTypes();

            // 根据全局配置读取用户配置
            LoadVerbConfig();
        }

        /// <summary>
        /// 获取插件
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public PluginBase GetPlugin(string[] args)
        {
            if (_verbTypes.Count < 1)
            {
                _logger.Error("未找到插件配置，可能是插件目录中包含与主程序重复的dll");
                return null;
            }
            // 运行命令
            var parserResult = Parser.Default
                .ParseArguments(args, _verbTypes.ToArray())
                .WithNotParsed(HandleParseError);
            if (!(parserResult is Parsed<object> parsed))
            {
                return null;
            }
            var pluginBase = parsed.Value as PluginBase;
            // 获取用户配置
            var verbAttribute = pluginBase.GetType().GetCustomAttribute<VerbAttribute>();
            var verbConfig = _verbsConfig.SelectTokenPlus(verbAttribute.Name + "s", new JArray());

            // 获取插件配置
            JToken pluginConfig = new JObject();
            if (_pluginSettingsDic.TryGetValue(pluginBase.GetType(), out JToken jToken)) pluginConfig = jToken;

            var container = new ConfigContainer(_systemConfig, pluginConfig, verbConfig, _verbsConfig);
            pluginBase.SetParams(container, this);

            return pluginBase;
        }


        // 加载系统配置文件
        private void LoadSystemConfig()
        {
            var baseDir = System.AppDomain.CurrentDomain.BaseDirectory;

            // 从本机读取
            string fileFullName = Path.Combine(baseDir, _systemConfigPath);

            if (!File.Exists(fileFullName))
            {
                _logger.Error("系统配置文件丢失");
                throw new ArgumentNullException("系统配置文件丢失");
            }

            // 读取配置文件
            using var reader = new StreamReader(fileFullName, Encoding.Default);
            var configStr = reader.ReadToEnd();
            var systemConfig = JsonConvert.DeserializeObject<JToken>(configStr);
            _systemConfig = systemConfig;
        }

        // 加载插件，并根据插件，保存其配置
        private Dictionary<Type, JToken> _pluginSettingsDic = new Dictionary<Type, JToken>();
        private void LoadPlugins()
        {
            // 提前加载通用类库
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            List<string> globalAssemblies = new List<string>() { "CommandLine.dll", "ShellProgressBar.dll" }
                .ConvertAll(p => Path.Combine(baseDir, p));
            // 避免被插件加载
            foreach (var globalDll in globalAssemblies) Assembly.LoadFrom(globalDll);

            // 获取插件配置
            JArray plugins = _systemConfig.SelectTokenPlus(FieldNames.plugins, new JArray());

            var enables = plugins.ToList().Where(t => t.SelectTokenPlus(FieldNames.enable, false)).ToList();

            if (enables.Count == 0) return;

            // 获取目录           
            var pluginsDir = Path.Combine(baseDir, FieldNames.plugins);
            var pluginConfigName = _systemConfig.SelectTokenPlus($"{FieldNames.system}.{FieldNames.pluginConfigName}", string.Empty);

            // 添加插件目录到环境变量中
            var pathEnv = Environment.GetEnvironmentVariable("Path");

            foreach (var plugin in enables)
            {
                var pluginName = plugin.SelectTokenPlus(FieldNames.name, string.Empty);
                if (string.IsNullOrEmpty(pluginName)) continue;

                // 组装插件目录
                var pluginDir = Path.Combine(pluginsDir, pluginName);
                // 添加目录到环境字符串中
                pathEnv += $";{pluginDir}";

                var dirInfo = new DirectoryInfo(pluginDir);
                if (!dirInfo.Exists) continue;

                // 获取所有的 dll 文件
                var dllFiles = dirInfo.GetFiles("*.dll", SearchOption.AllDirectories);
                // 过滤掉已经加载了的程序集
                var assNames = AppDomain.CurrentDomain.GetAssemblies().ToList().ConvertAll(s => s.GetName().Name);
                dllFiles = dllFiles.Where(f => !assNames.Contains(f.Name.Replace(f.Extension, ""))).ToArray();

                // 读取配置
                JToken pluginConfig = new JObject();
                if (!string.IsNullOrEmpty(pluginConfigName))
                {
                    var pluginConfigPath = Path.Combine(pluginDir, pluginConfigName);
                    if (File.Exists(pluginConfigPath))
                    {
                        // 读取配置文件
                        using var reader = new StreamReader(pluginConfigPath, Encoding.Default);
                        var configStr = reader.ReadToEnd();
                        pluginConfig = JsonConvert.DeserializeObject<JToken>(configStr);
                    }
                }

                // 加载 dll
                foreach (var dllFile in dllFiles)
                {
                    try
                    {
                        // LoadFrom 会加载依赖
                        // LoadFile 仅加载当前文件
                        Assembly pluginAssembly = Assembly.LoadFrom(dllFile.FullName);

                        // 根据 dll 中的 type，保存配置
                        var types = pluginAssembly.GetTypes().Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToList();

                        // 保存所有的 type
                        _verbTypes.AddRange(types);

                        var test = pluginAssembly.GetTypes();

                        // 保存类型与配置
                        foreach (var type in types)
                        {
                            if (_pluginSettingsDic.ContainsKey(type))
                            {
                                continue;
                            }

                            _pluginSettingsDic.Add(type, pluginConfig);
                        }
                    }
                    catch (Exception ex)
                    {
                        // _logger.Error(ex);
                        continue;
                    }
                }
            }

            Environment.SetEnvironmentVariable("Path", pathEnv);
        }

        // 获取当前系统的 type
        private void LoadSystemPluginTypes()
        {
            var results = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttribute<VerbAttribute>() != null);
            _verbTypes.AddRange(results);
        }

        // 加载命令配置文件
        private void LoadVerbConfig()
        {
            // 读取用户配置文件
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var userConfigName = _systemConfig.SelectTokenPlus($"{FieldNames.system}.{FieldNames.userConfigName}", string.Empty);

            // 从本机读取
            string fileFullName = Path.Combine(baseDir, userConfigName);

            _logger.Debug($"用户配置文件位置：{fileFullName}");

            if (!File.Exists(fileFullName))
            {
                _logger.Error("用户配置文件不存在");
                throw new ArgumentNullException("用户配置文件不存在");
            }

            // 读取配置文件
            string configStr = new StreamReader(fileFullName, Encoding.Default).ReadToEnd();
            _verbsConfig = JsonConvert.DeserializeObject<JToken>(configStr);
        }

        // 错误处理
        private void HandleParseError(IEnumerable<Error> errs)
        {
            //if (errs.IsVersion())
            //{
            //    _logger.Error("Version Request");
            //    return;
            //}

            //if (errs.IsHelp())
            //{
            //    _logger.Error("Help Request");
            //    return;
            //}

            //_logger.Error("Parser Failed");
        }
    }
}
