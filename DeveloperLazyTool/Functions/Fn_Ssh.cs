using DeveloperLazyTool.Modules;
using DeveloperLazyTool.Options;
using log4net;
using Newtonsoft.Json.Linq;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Functions
{
    class Fn_Ssh : SingleFuncBase
    {
        private Opt_Ssh _option;
        private ILog _logger = LogManager.GetLogger(typeof(Fn_Script));

        public override void SetParams(OptionBase optionBase)
        {
            base.SetParams(optionBase);

            _option = ConvertParams<Opt_Ssh>();
        }

        protected override string GetRuningName()
        {
            return _option.Name;
        }

        // 仅例子，未完成
        protected override StdInOut RunOne(JToken jToken)
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

            return null;
        }
    }
}
