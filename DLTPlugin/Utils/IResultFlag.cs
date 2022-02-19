namespace WowToolAPI.Utils.Core
{
    /// <summary>
    /// 结果标记
    /// </summary>
    public interface IResultFlag
    {
        /// <summary>
        /// 是否验证通过
        /// </summary>
        bool Ok { get; set; }

        /// <summary>
        /// 是否不通过
        /// </summary>
        bool NotOk { get; }

        /// <summary>
        /// 附带的消息
        /// </summary>
        string Message { get; set; }
    }
}
