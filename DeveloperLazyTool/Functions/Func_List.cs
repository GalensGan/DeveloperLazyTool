using DeveloperLazyTool.Modules;
using DeveloperLazyTool.Options;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Functions
{
    class Func_List : SingleFuncBase
    {
        private ILog _logger = LogManager.GetLogger(typeof(Fn_Ftp));

        protected override string GetRuningName()
        {
            return Option.Name;
        }

        protected override Argument RunOne(JToken jToken)
        {
            _logger.Info("未实现");

            return null;
        }
    }
}
