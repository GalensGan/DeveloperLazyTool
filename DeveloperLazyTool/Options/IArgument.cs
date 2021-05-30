using DeveloperLazyTool.Modules;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Options
{
    public interface IArgument
    {
       bool InitRuningArgs(Argument argument);
    }
}
