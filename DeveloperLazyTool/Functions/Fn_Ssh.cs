﻿using DeveloperLazyTool.Core.Modules;
using DeveloperLazyTool.Core.Options;
using log4net;
using Newtonsoft.Json.Linq;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Core.Functions
{
    /// <summary>
    /// 未完成
    /// </summary>
    class Fn_Ssh : FuncBase
    {
        private Opt_Ssh _option;
        private ILog _logger = LogManager.GetLogger(typeof(Fn_Script));

        public Fn_Ssh(StdInOut stdInOut) : base(stdInOut) { }

        // 仅例子，未完成
        public override StdInOut Run()
        {
            using (var sshClient = new SshClient("host", 22, "username", "password"))

            {
                sshClient.Connect();
                using (var cmd = sshClient.CreateCommand("dir /w"))
                {
                    var res = cmd.Execute();
                    Console.Write(res);
                }
            }

            return InputParams;
        }
    }
}
