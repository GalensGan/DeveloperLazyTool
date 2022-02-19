using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Core.Extensions
{
    public static class Ex_Json
    {
        public static T SelectTokenPlus<T>(this JToken jt,string path,T defaultValue)
        {
            var val = jt.SelectToken(path, false);
            if (val!=null) return val.ToObject<T>();

            return defaultValue;
        }
    }
}
