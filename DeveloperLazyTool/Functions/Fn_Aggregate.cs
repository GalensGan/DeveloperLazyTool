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
    /// <summary>
    /// 执行管道
    /// </summary>
    class Fn_Aggregate : ArrayFuncBase
    {
        private Opt_Aggregate _option;
        private ILog _logger = LogManager.GetLogger(typeof(Fn_Aggregate));

        public override void SetParams(OptionBase optionBase)
        {
            base.SetParams(optionBase);

            _option = ConvertParams<Opt_Aggregate>();
        }

        protected override string GetRuningName()
        {
            return _option.Name;
        }

        protected override Argument RunOne(JToken jToken)
        {
            return null;
        }
    }
}
