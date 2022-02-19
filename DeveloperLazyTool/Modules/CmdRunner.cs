using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Core.Modules
{
    class CmdRunner: ScriptRunner
    {
        private ProcessStartInfo _startInfo;
        private bool _waitForExit = false;

        public CmdRunner(string command,bool waitForExit = false)
        {
            _waitForExit = waitForExit;
   
            _startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",//设定需要执行的命令    
                Arguments = "/C " + command,//“/C”表示执行完命令后马上退出    
                UseShellExecute = false,//不使用系统外壳程序启动
                RedirectStandardInput = false,//不重定向输入    
                RedirectStandardOutput = true, //重定向输出    
                CreateNoWindow = true, //不创建窗口                
            };          
        }

        public override void Start()
        {
            var  process = new Process();//创建进程对象
            process.StartInfo = _startInfo;
            process.Start();            
            if (_waitForExit)
            {
                process.WaitForExit();
            }
        }
    }
}
