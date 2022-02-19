using Newtonsoft.Json.Linq;

namespace WowToolAPI.Utils.Core
{
    public class SuccessResult : Result<JToken>
    {
        public SuccessResult(JToken data =null) : base(data, true, "success")
        {
        }
    }
}
