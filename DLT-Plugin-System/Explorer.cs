using CommandLine;
using DeveloperLazyTool.Plugin;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowToolAPI.Utils.Core;

namespace DLTPlugins.System
{
    /// <summary>
    /// 
    /// </summary>
    [Verb("exp", HelpText = "打开当前目录或者文件")]
    internal class Explorer : CustomPlugin
    {
        private ILog _logger = LogManager.GetLogger(typeof(Explorer));

        [Option('p', "path", HelpText = "打开的目录路径")]
        public IEnumerable<string> Paths { get; set; }

        public override IResult<JToken> RunOneCommand(JObject verbSetting)
        {
            // 如果没有 Name 或者 path 为空，则只打开当前目录
            // 如果没有 path，则打开当前，然后返回
            if ((Paths == null || Paths.Count() < 1) && string.IsNullOrEmpty(Name))
            {
                var baseDir = Environment.CurrentDirectory;
                if (Directory.Exists(baseDir))
                {
                    Process.Start("explorer.exe", baseDir);
                    return new SuccessResult();
                }
            }

            // 如果有 Name, 则去找 Name 项配置
            OpenByName();

            // 如果有 path，则调用打开模块
            OpenByPath();

            return new SuccessResult();
        }

        private IResult<JToken> OpenByName()
        {
            if (string.IsNullOrEmpty(Name)) return new ErrorResult("没有输入 Name");

            // 查找Name配置
            if (!(ConfigContainer.VerbSetting is JArray configs)) return new ErrorResult("命令配置不存在");

            // 获取Name对应的配置
            var nameConfig = configs.Where(jt => jt.Value<string>("name").ToLower() == Name.ToLower()).FirstOrDefault();
            if (nameConfig == null)
            {
                var message = $"未在 ess 中找到 {Name} 对应的配置";
                _logger.Error(message);
                return new ErrorResult(message);
            }

            var paths = nameConfig.SelectToken("paths") as JArray;
            if (paths == null || paths.Count() < 1)
            {
                var message = $"未找到 paths 字段，应为数组";
                _logger.Error(message);
                return new ErrorResult(message);
            }

            foreach (var pathJt in paths)
            {
                // 判断是否存在
                var path = pathJt.ToString();

                if (File.Exists(path))
                {
                    Process.Start(path);
                    continue;
                }

                if (Directory.Exists(path))
                {
                    Process.Start("explorer.exe", path);
                    continue;
                }
            }

            return new SuccessResult();
        }

        private IResult<JToken> OpenByPath()
        {
            if (Paths == null) return new ErrorResult("没有传递 path");

            // 获取目录
            var baseDir = Environment.CurrentDirectory;

            // 有 path 时
            foreach (var path in Paths)
            {
                string fullPath = baseDir;
                if (!string.IsNullOrEmpty(path))
                {
                    fullPath = Path.Combine(baseDir, path);
                }

                if (File.Exists(fullPath))
                {
                    // 用默认程序打开文件
                    Process.Start(fullPath);
                    continue;
                }

                if (Directory.Exists(fullPath))
                {
                    Process.Start("explorer.exe", fullPath);
                    continue;
                }

                var message = $"文件或目录{fullPath} 不存在";
                _logger.Error(message);
            }

            return new SuccessResult();
        }
    }
}
