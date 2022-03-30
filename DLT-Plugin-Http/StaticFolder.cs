using CommandLine;
using DeveloperLazyTool.Plugin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowToolAPI.Utils.Core;

namespace DLTPlugin.Http
{
    /// <summary>
    /// 静态目录
    /// </summary>
    [Verb("http-static", HelpText = "静态 http 服务(未实现)")]
    internal class StaticFolder : PluginBase
    {
        public override IResult<JToken> RunOneCommand(JObject verbSetting)
        {
            throw new NotImplementedException("暂未实现");

            //// 判断是否有静态目录，如果有的话，添加
            //if (verbSetting.TryGetValue("static", out var staticDir))
            //{
            //    server.WithStaticFolder("staticFolder", "/public", staticName, false, m =>
            //    {
            //        m.ContentCaching = true;
            //        m.Cache = new Swtools.Service.Http.Files.FileCache()
            //        {
            //            MaxFileSizeKb = 204800,
            //            MaxSizeKb = 1024000,
            //        };
            //    });
            //} // 判断是否有静态目录，如果有的话，添加
            //if (verbSetting.TryGetValue("static", out var staticDir))
            //{
            //    server.WithStaticFolder("staticFolder", "/public", staticName, false, m =>
            //    {
            //        m.ContentCaching = true;
            //        m.Cache = new Swtools.Service.Http.Files.FileCache()
            //        {
            //            MaxFileSizeKb = 204800,
            //            MaxSizeKb = 1024000,
            //        };
            //    });
            //}
        }
    }
}
