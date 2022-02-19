using Newtonsoft.Json.Linq;

namespace WowToolAPI.Utils.Core
{
    public class ErrorResult : Result<JToken>
    {
        public ErrorResult(JToken data, string error) : base(data, false, error)
        {
        }

        public ErrorResult(string error) : base(default, false, error)
        {
        }
    }
}
