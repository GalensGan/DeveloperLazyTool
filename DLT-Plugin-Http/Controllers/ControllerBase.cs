using EmbedIO.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DLTPlugin.Http.Controllers
{
    public abstract class ControllerBase : WebApiController
    {
        protected HttpClient HttpClient { get; private set; }

        public ControllerBase()
        {
            // 初始化 http
            HttpClient = new HttpClient();
        }
       
    }
}
