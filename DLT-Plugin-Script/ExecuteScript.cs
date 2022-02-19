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

namespace DLTPlugins.Script
{
    [Verb("es", true, HelpText = "运行脚本文件(Execute Script)")]
    public class ExecuteScript : PluginBase
    {
        //[Value(0, HelpText = "上传的配置名称",Required =false)]
        //// [Option('n', "name", HelpText = "上传的配置名称", Required = false)]        
        //public string Name { get; set; }

        [Option('b', "background", HelpText = "是否在后台运行", Default = false)]
        public bool Background { get; set; }

        [Option('p', "params", HelpText = "动态参数")]
        public IEnumerable<string> Params { get; set; }


        private ILog _logger = LogManager.GetLogger(typeof(ExecuteScript));

        public override IResult<JToken> RunOneCommand(JObject verbSetting)
        {
            // 运行脚本
            var jToken = verbSetting;
            // 获取bat文件路径
            string name = jToken.Value<string>("name");
            string fileFullName = jToken.Value<string>("fileName");
            string arguments = jToken.Value<string>("arguments") ?? string.Empty;


            Process p = new Process();
            // 如果有附加参数，则要添加
            if (Params != null && Params.Count() > 0)
            {
                // 添加附加的
                string attachParams = string.Join(" ", Params);

                arguments += " " + attachParams;
            }
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                WorkingDirectory = Path.Combine(baseDir, "scripts"),
                FileName = fileFullName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardInput = true,// 接受来自调用程序的输入信息
                RedirectStandardOutput = true,// 由调用程序获取输出信息
                RedirectStandardError = true,// 重定向标准错误输出
                CreateNoWindow = true,// 不显示程序窗口
                StandardOutputEncoding = Encoding.Default,
                StandardErrorEncoding = Encoding.Default
            };
            p.StartInfo = startInfo;
            p.Start();//启动程序

            p.StandardInput.AutoFlush = true;

            p.OutputDataReceived += P_OutputDataReceived;
            p.ErrorDataReceived += P_ErrorDataReceived;
            p.BeginOutputReadLine();

            if (!Background)
            {
                //获取cmd窗口的输出信息
                // string output = p.StandardOutput.ReadToEnd();
                //等待程序执行完退出进程
                p.WaitForExit();
                p.Close();
            }


             // 说明成功了
            if (!Quiet) _logger.Info($"运行 scripts:[{name}] 成功！");

            return new SuccessResult();
        }

        private void P_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data)) return;
            // 转码
            byte[] bytes = Encoding.Default.GetBytes(e.Data);
            bytes = Encoding.Convert(Encoding.Default, Encoding.UTF8, bytes);
            string formatString = Encoding.UTF8.GetString(bytes);
            Console.WriteLine(formatString);
        }

        private void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data)) return;
            // string test = "成功";
            // 转码
            byte[] bytes = Encoding.Default.GetBytes(e.Data);
            bytes = Encoding.Convert(Encoding.Default, Encoding.UTF8, bytes);
            string formatString = Encoding.UTF8.GetString(bytes);
            Console.WriteLine(formatString);
        }
    }
}
