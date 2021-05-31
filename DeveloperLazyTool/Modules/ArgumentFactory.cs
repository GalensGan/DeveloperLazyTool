using DeveloperLazyTool.Enums;
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
    /// <summary>
    /// 参数的工厂，可以从这儿获取生成一些参数
    /// </summary>
    public class ArgumentFactory
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(ArgumentFactory));

        private JObject JObjUser { get; set; }

        public ArgumentFactory(string baseDir, string pathData, string userConfigName)
        {
            // 读取用户配置文件
            // 从本机读取
            string fileFullName = Path.Combine(baseDir, pathData, userConfigName);

            _logger.Debug($"用户配置文件位置：{fileFullName}");

            if (!File.Exists(fileFullName))
            {

                _logger.Error("用户配置文件不存在");
                return;
            }

            // 读取配置文件
            string configStr = new StreamReader(fileFullName, Encoding.Default).ReadToEnd();
            JObject jObjUser = JsonConvert.DeserializeObject<JObject>(configStr);


            JObjUser = jObjUser;
        }

        public Argument GetNamedArguments(string name)
        {
            if (JObjUser.ContainsKey(name))
            {
                JArray array = JObjUser.Value<JArray>(name);
                return new Argument(array);
            }

            return default;
        }

        /// <summary>
        /// 获取Ftp参数
        /// </summary>
        /// <returns></returns>
        public Argument GetFtpArguments()
        {
            return GetNamedArguments(Enums.FieldNames.ftps.ToString());
        }
    }
}
