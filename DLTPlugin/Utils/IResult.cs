namespace WowToolAPI.Utils.Core
{

    /// <summary>
    ///返回结果接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IResult<T>:IResultFlag
    {       

        /// <summary>
        /// 数据
        /// </summary>
        T Data { get; set; }

        /// <summary>
        /// 数据转换成另一个类型
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        IResult<T1> ConvertTo<T1>(T1 data);

        /// <summary>
        /// 将结果转置
        /// </summary>
        /// <returns></returns>
        IResult<T> Reverse();
    }
}
