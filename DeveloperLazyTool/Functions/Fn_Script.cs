using DeveloperLazyTool.Enums;
using DeveloperLazyTool.Modules;
using DeveloperLazyTool.Options;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Functions
{
    public class Fn_Script : FuncBase
    {
        private Opt_Script _option;
        private ILog _logger = LogManager.GetLogger(typeof(Fn_Script));

        public Fn_Script(StdInOut stdInOut) : base(stdInOut)
        {
            _option = stdInOut.CmdOptions as Opt_Script;
        }

        public override StdInOut Run()
        {
            var jToken = InputParams.CmdConfig;
            // 获取bat文件路径
            string name = jToken.Value<string>("name");
            string fileFullName = jToken.Value<string>("fileName");
            string successFlag = jToken.Value<string>("successFlag") ?? string.Empty;
            string arguments = jToken.Value<string>("arguments") ?? string.Empty;


            Process p = new Process();
            // 如果有附加参数，则要添加
            if (_option.Params != null && _option.Params.Count() > 0)
            {
                // 添加附加的
                string attachParams = string.Join(" ", _option.Params);

                arguments += " " + attachParams;
            }
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                WorkingDirectory = Path.Combine(_option.BaseDir, _option.PathScript),
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

            if (!_option.Background)
            {
                //获取cmd窗口的输出信息
                // string output = p.StandardOutput.ReadToEnd();
                //等待程序执行完退出进程
                p.WaitForExit();
                p.Close();
            }


            //if (output.Contains(successFlag))
            //{
            //    // 说明成功了
            if (!_option.Quiet) _logger.Info($"运行 {Enums.FieldNames.scripts}:[{name}] 成功！");
            return InputParams;
            //}
            //else
            //{
            //    if (!_option.Quiet) _logger.Error($"运行 {Enums.FieldNames.scripts}:[{name}] 失败！");
            //    return null;
            //}
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
