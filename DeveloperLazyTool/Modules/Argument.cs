using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Modules
{
    public class Argument
    {
        private ILog _logger = LogManager.GetLogger(typeof(Argument));

        public JObject ReadUserConfig(string baseDir,string pathData,string userConfigName)
        {
            // 读取用户配置文件
            // 从本机读取
            string fileFullName = Path.Combine(baseDir,pathData,userConfigName);

            _logger.Debug($"用户配置文件位置：{fileFullName}");

            if (!File.Exists(fileFullName))
            {

                _logger.Error("用户配置文件不存在");
                return null;
            }

            // 读取配置文件
            string configStr = new StreamReader(fileFullName, Encoding.Default).ReadToEnd();
            JObject jObjUser = JsonConvert.DeserializeObject<JObject>(configStr);


            return jObjUser;
        }
    }
}
