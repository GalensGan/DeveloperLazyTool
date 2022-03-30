using CommandLine;
using DeveloperLazyTool.Plugin;
using DLTPlugin.Http.Controllers;
using EmbedIO;
using EmbedIO.WebApi;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WowToolAPI.Utils.Core;
using WowToolAPI.Utils.Extensions;

namespace DLTPlugin.Http
{
    [Verb("http", HelpText = "搭建http服务")]
    public class Setup : PluginBase
    {
        private WebServer _server;
        private ILog _logger = LogManager.GetLogger(typeof(Setup));

        public override IResult<JToken> RunOneCommand(JObject verbSetting)
        {
            // Our web server is disposable
            _server = CreateWebServer(verbSetting);

            // Once we've registered our modules and configured them, we call the RunAsync() method.
            _server.RunAsync();

            return new SuccessResult();
        }


        // Create and configure our web server.
        private WebServer CreateWebServer(JObject verbSetting)
        {
            // 默认配置为
            var host = verbSetting.SelectTokenPlus("host", "*");
            var port = verbSetting.SelectTokenPlus("port", 13148);
            var baseRout = verbSetting.SelectTokenPlus("baseRoute", "api");

            var url = $"http://{host}:{port}";


            // 获取当前工作目录           
            _logger.Info("开始加载 http 路由");

            WebServer server = new WebServer(o => o
                    .WithUrlPrefix(url)
                    .WithMode(HttpListenerMode.EmbedIO))
                // 允许跨域
                .WithCors()
                // First, we will configure our web server by adding Modules.
                .WithLocalSessionManager();

            // 添加路由
            // 获取所有继承于 baseController 的类
            Type[] types = Assembly.GetCallingAssembly().GetTypes();
            Type baseType = typeof(ControllerBase);
            List<Type> controllers = types.Where(t =>
            {
                return baseType == t.BaseType;
            }).ToList();

            server.WithWebApi(baseRout, m =>
            {
                controllers.ForEach(ctor => m.WithController(ctor));
            });

            _logger.Info("http 路由加载完成");

            // Listen for state changes.
            server.StateChanged += Server_StateChanged;

            return server;
        }

        private void Server_StateChanged(object sender, WebServerStateChangedEventArgs e)
        {
            _logger.Info($"WebServer New State - {e.NewState}");
        }
    }
}
