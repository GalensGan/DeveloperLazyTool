using System.Collections;

namespace WowToolAPI.Utils.Core
{
    public class Result<T> : IResult<T>
    {
        public Result() { }


        /// <summary>
        /// 创建一个实例
        /// </summary>
        /// <param name="ok"></param>
        /// <param name="data"></param>
        /// <param name="error"></param>
        public Result(T data, bool ok, string error)
        {
            Data = data;
            Ok = ok;
            Message = error;
        }

        public bool Ok { get; set; }
        public bool NotOk => !Ok;
        public string Message { get; set; }
        public T Data { get; set; }     

        public IResult<T1> ConvertTo<T1>(T1 data)
        {
            return new Result<T1>()
            {
                Data = data,
                Message = Message,
                Ok = Ok,
            };
        }

        /// <summary>
        /// 转置结果
        /// </summary>
        /// <returns></returns>
        public IResult<T> Reverse()
        {
            Ok = !Ok;
            return this;
        }

    }
}
