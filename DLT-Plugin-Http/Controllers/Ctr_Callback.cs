using EmbedIO;
using EmbedIO.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLTPlugin.Http.Controllers
{
    /// <summary>
    /// 网络回调专用
    /// </summary>
    internal class Ctr_Callback:ControllerBase
    {
        [Route(HttpVerbs.Get, "/tasks/{id}")]
        public Task RunCallback()
        {

        }
    }
}
