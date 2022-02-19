using DeveloperLazyTool.Core.Modules;
using DeveloperLazyTool.Core.Options;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Core.Functions
{
    /// <summary>
    /// 执行管道
    /// </summary>
    class Fn_Aggregate : FuncBase
    {
        private Opt_Aggregate _option;
        private ILog _logger = LogManager.GetLogger(typeof(Fn_Aggregate));

        public Fn_Aggregate(StdInOut stdInOut) : base(stdInOut) { }
        public override StdInOut Run()
        {
            _logger.Info("聚合模块");
            return InputParams;
        }
    }
}
