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
        public FuncBase(StdInOut inputParams)
        {
            InputParams = inputParams;
        }

        protected StdInOut InputParams { get; private set; }

        public abstract StdInOut Run();
    }
}
