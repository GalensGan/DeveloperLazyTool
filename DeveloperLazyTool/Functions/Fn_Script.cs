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
    public class Fn_Script : ArrayFuncBase
    {
        private Opt_Script _option;
        private ILog _logger = LogManager.GetLogger(typeof(Fn_Script));

        public override void SetParams(OptionBase optionBase)
        {
            base.SetParams(optionBase);

            _option = ConvertParams<Opt_Script>();
        }

        protected override Argument RunOne(JToken jToken)
        {
            // 获取bat文件路径
            string name = jToken.Value<string>("name");
            string fileFullName = jToken.Value<string>("fileName");
            string successFlag = jToken.Value<string>("successFlag")??string.Empty;
            string arguments = jToken.Value<string>("arguments") ?? string.Empty;

            Process p = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                WorkingDirectory = Path.Combine(Option.BaseDir,Option.PathScript),
                FileName = fileFullName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardInput = true,//接受来自调用程序的输入信息
                RedirectStandardOutput = true,//由调用程序获取输出信息
                RedirectStandardError = true,//重定向标准错误输出
                CreateNoWindow = true,//不显示程序窗口
            };
            p.StartInfo = startInfo;
            p.Start();//启动程序

            p.StandardInput.AutoFlush = true;

            //获取cmd窗口的输出信息
            string output = p.StandardOutput.ReadToEnd();
            //等待程序执行完退出进程
            p.WaitForExit();
            p.Close();

            if (output.Contains(successFlag))
            {
                // 说明成功了
                if (!_option.Quiet) _logger.Info($"运行 {Enums.FieldNames.scripts}:[{name}] 成功！");
                return new Argument($"{Enums.FieldNames.script}_{name}", jToken as JObject);
            }
            else
            {
                if (!_option.Quiet) _logger.Error($"运行 {Enums.FieldNames.scripts}:[{name}] 失败！");
                return null;
            }

        }

        protected override string GetRuningName()
        {
            return _option.Name;
        }
    }
}
