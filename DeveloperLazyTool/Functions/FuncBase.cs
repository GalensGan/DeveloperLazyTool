using DeveloperLazyTool.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Functions
{
    public abstract class FuncBase
    {
        protected StdInOut InputParams { get; private set; }

        public abstract StdInOut Run();

        /// <summary>
        /// 重写时，必须先调用父类方法
        /// </summary>
        /// <param name="optionBase"></param>
        public void SetParams(StdInOut inputParams)
        {
            InputParams = inputParams;
        }
    }
}
