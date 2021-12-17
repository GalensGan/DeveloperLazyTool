using DeveloperLazyTool.Enums;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Modules
{
    /// <summary>
    /// 标准的输入输出
    /// </summary>
    public class StdInOut
    {
        public StdInOut(JObject currentConfig,JObject allUserConfigs)
        {
            CurrentConfig = currentConfig;
            AllUserConfigs = allUserConfigs;

            // 获取name作为stage免
            _stageName = currentConfig.SelectToken(FieldNames.name.ToString()).ToString();

            // 新建数据储存
            Data = new JObject();
        }
        private ILog _logger = LogManager.GetLogger(typeof(StdInOut));

        private string _stageName = string.Empty;

        public JObject AllUserConfigs { get; private set; }

        /// <summary>
        /// 保存定义命令的原始数据
        /// </summary>
        public JObject CurrentConfig { get;private set; }

        /// <summary>
        /// 保存当前命令产生的结果数据
        /// </summary>
        public JObject Data { get; set; }

        /// <summary>
        /// 前一个操作的结果
        /// </summary>
        public StdInOut Previous { get;private set; }

        /// <summary>
        /// 下一个操作的结果
        /// </summary>
        public StdInOut Next { get;private set; }

        public bool AddPrevious(StdInOut pre)
        {
            if (pre == null) return false;

            Previous = pre;

            return true;
        }
        public bool AddNext(StdInOut next)
        {
            if (next == null) return false;

            Next = next;

            return true;
        }
        #region 获取参数值
        public Tuple<bool, T> GetValue<T>(string fieldName)
        {
            // 如果不是以 $ 开头，说明语法不正确
            if (!fieldName.StartsWith("$"))
            {
                throw new ArgumentException("参数应以$开头");
            }

            // 判断是否有限定名称(阶段名.字段名),正则判断
            Regex regex = new Regex(@"^\&[a-zA-Z_]+.");
            if (regex.IsMatch(fieldName))
            {
                // 匹配之后，说明找特定的字段
                string[] names = fieldName.Split('.');
                string argName = names[0].Replace("$", "");
                string fieldNameTemp = names[1];

                return GetValue<T>(argName, fieldNameTemp);
            }
            else
            {
                string fieldNameTemp = fieldName.Replace("$", "");
                return GetValue<T>(fieldNameTemp);
            }
        }

        public Tuple<bool, T> GetValue<T>(string stageName, string fieldName)
        {
            if (string.IsNullOrEmpty(stageName) || string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentException("参数为空");
            }

            if (stageName == _stageName)
            {
                return QueryField<T>(fieldName);
            }

            // 向上查找
            if (Previous != null)
            {
                return Previous.GetValue<T>(stageName, fieldName);
            }

            return new Tuple<bool, T>(false, default);
        }

        /// <summary>
        /// 查找字段，该字段不包含 $
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private Tuple<bool, T> QueryField<T>(string fieldName)
        {
            if (Data == null) return new Tuple<bool, T>(false, default);

            if (!Data.ContainsKey(fieldName)) return new Tuple<bool, T>(false, default);

            var value = Data.Value<T>(fieldName);
            return new Tuple<bool, T>(true, value);
        }

        #endregion

        #region 设置参数
        /// <summary>
        /// 添加字段。只能在当前的 argument 中添加
        /// 添加字段之前，要调用 InitResult
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <returns>true 代表新增，false 代表存在，但是更新了值</returns>
        public bool AddField<T>(string fieldName, T fieldValue)
        {           
            // 添加的字段前面都前缀两个下划线 __，避免与用户定义的字段冲突
            string fieldNameTemp = "__" + fieldName;

            // 判断是否重复，如果重复了，就更新，但是返回false
            if (Data.ContainsKey(fieldNameTemp))
            {
                // 代表重复了，直接更新值
                Data.Remove(fieldNameTemp);
                Data.Add(new JProperty(fieldName, fieldValue));

                return false;
            }

            Data.Add(new JProperty(fieldNameTemp, fieldValue));
            return true;
        }
        #endregion
    }
}
