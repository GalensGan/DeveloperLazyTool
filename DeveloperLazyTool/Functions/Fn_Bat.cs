using DeveloperLazyTool.Modules;
using DeveloperLazyTool.Options;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Functions
{
    public class Fn_Bat : ArrayFuncBase
    {
        private Opt_Bat _option;
        private ILog _logger = LogManager.GetLogger(typeof(Fn_Bat));

        public override void SetParams(OptionBase optionBase)
        {
            base.SetParams(optionBase);

            _option = ConvertParams<Opt_Bat>();
        }

        protected override Argument RunOne(JToken jToken)
        {
            // 获取bat文件路径
            string name = jToken.Value<string>("name");
            string fileFullName = jToken.Value<string>("fileFullName");
            string successFlag = jToken.Value<string>("successFlag");

            Process p = new Process();
            p.StartInfo.FileName = fileFullName;           
            p.StartInfo.UseShellExecute = false;//是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
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
                _logger.Info($"运行 bats:[{name}] 成功！");
                return new Argument($"{Enums.FieldNames.bat}_{name}", jToken as JObject);
            }
            else
            {
                _logger.Error($"运行 bats:[{name}] 失败！");
                return null;
            }
        }

        protected override string GetRuningName()
        {
            return _option.Name;
        }
    }
}
