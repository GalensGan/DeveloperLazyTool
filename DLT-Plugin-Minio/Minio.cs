using CommandLine;
using DeveloperLazyTool.Plugin;
using log4net;
using Minio;
using Newtonsoft.Json.Linq;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using WowToolAPI.Utils.Core;
using WowToolAPI.Utils.Extensions;

namespace DLTPlugin.Minio
{
    [Verb("minio", HelpText = "向minio上传文件")]
    internal class Minio : PluginBase
    {
        // 名称必须要有
        [Value(0, HelpText = "名称", Required = true)]
        public override string Name { get; set; }

        [Option('p', "path", HelpText = "上传的文件路径")]
        public IEnumerable<string> FilePaths { get; set; }

        /// <summary>
        /// 从剪切板中上传数据
        /// 未实现，因为需要在 main 入口处标记 STAThread, 代码不纯粹了
        /// </summary>
        [Option('c', "clipboard", HelpText = "从剪切板上传文件(未实现)")]
        public bool FromClipboard { get; set; }

        private ILog _logger = LogManager.GetLogger(typeof(Minio));
        public override IResult<JToken> RunOneCommand(JObject verbSetting)
        {
            // 判断是否有路径或者剪切板配置
            if (FilePaths.Count() < 1 && !FromClipboard)
            {
                var message = "请传递上传的文件参数 -p 或 -c";
                _logger.Error(message);
                return new ErrorResult(message);
            }

            // 生成客户端
            var endpoint = verbSetting.SelectTokenPlus("endpoint", string.Empty);
            if (string.IsNullOrEmpty(endpoint))
            {
                var message = "endpoint 为空";
                _logger.Error(message);
                return new ErrorResult(message);
            }

            var accessKey = verbSetting.SelectTokenPlus("accessKey", string.Empty);
            var secretKey = verbSetting.SelectTokenPlus("secretKey", string.Empty);
            var region = verbSetting.SelectTokenPlus("region", string.Empty);
            var sessionToken = verbSetting.SelectTokenPlus("sessionToken", string.Empty);
            var useSSL = verbSetting.SelectTokenPlus("useSSL", true);

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
                var client = new MinioClient(endpoint, accessKey, secretKey, region, sessionToken);

                if (useSSL) client = client.WithSSL();

                var bucketName = verbSetting.SelectTokenPlus("bucketName", string.Empty);
                if (string.IsNullOrEmpty(bucketName))
                {
                    var message = "bucketName 为空";
                    _logger.Error(message);
                    return new ErrorResult(message);
                }

                var objectDir = verbSetting.SelectTokenPlus("objectDir", string.Empty);
                if (string.IsNullOrEmpty(objectDir))
                {
                    var message = "objectDir 为空";
                    _logger.Error(message);
                    return new ErrorResult(message);
                }

                // Console.WriteLine("开始上传文件:");
                var protocol = "http";
                if (useSSL) protocol = "https";

                // 利用 httpclient 上传
                HttpClientHandler handler = new HttpClientHandler();
                ProgressMessageHandler progressMessageHandler = new ProgressMessageHandler(handler);
                // progressMessageHandler.HttpReceiveProgress += ProgressMessageHandler_HttpReceiveProgress;
                progressMessageHandler.HttpSendProgress += ProgressMessageHandler_HttpSendProgress;
                HttpClient httpClient = new HttpClient(progressMessageHandler)
                {
                    Timeout = TimeSpan.FromMinutes(60)
                };

                List<string> resultUrls = new List<string>();

                using (ShellProgressBar.ProgressBar progressBar = new ShellProgressBar.ProgressBar(1000, $"开始上传文件至 minio", _options))
                {
                    _filesCount = FilePaths.Count();
                    _progressBar = progressBar;

                    // 判断桶是否存在
                    if (!client.BucketExistsAsync(bucketName).GetAwaiter().GetResult())
                    {
                        var message = $"桶: {bucketName} 不存在";
                        _logger.Error(message);
                        return new ErrorResult(message);
                    }

                    // 从本地上传文件
                    UploadFromFilePaths(httpClient, client, bucketName, objectDir, ref resultUrls);

                    // 从剪切板上传文件
                    UploadFromClipboard(httpClient, client, bucketName, objectDir, ref resultUrls);

                    _progressBar.AsProgress<double>().Report(1);
                }

                // 写入命令行
                // 要是进度条外面写才能显示
                Console.WriteLine("上传成功:");
                foreach (var resultUrl in resultUrls)
                {
                    if (resultUrl.StartsWith("/")) Console.WriteLine($"{protocol}://{endpoint}{resultUrl}");
                }

                // 返回结果
                return new SuccessResult();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return new ErrorResult(ex.Message);
            }
        }

        private void ProgressMessageHandler_HttpSendProgress(object sender, HttpProgressEventArgs e)
        {
            _progressBar.AsProgress<double>().Report(1.0 * e.ProgressPercentage * _currentIndex / _filesCount / 100);
        }

        private ProgressBarOptions _options = new ProgressBarOptions
        {
            ProgressCharacter = '=',
            ProgressBarOnBottom = true,
            DisplayTimeInRealTime = false,
            MessageEncodingName = "utf-8"
        };

        private ShellProgressBar.ProgressBar _progressBar = null;
        private int _filesCount = 1;
        private int _currentIndex = 0;

        private IResult<JToken> UploadFromFilePaths(HttpClient httpClient, MinioClient client, string bucketName, string objectDir, ref List<string> resultUrls)
        {
            if (FilePaths.Count() > 0)
                _logger.Info("正在从路径上传文件");

            foreach (var filePath in FilePaths)
            {
                _currentIndex++;

                _progressBar.Tick($"正在上传（{_currentIndex}/{_filesCount}）：{Path.GetFileName(filePath)}");

                // 生成文件名字
                var fileName = Path.Combine(Environment.CurrentDirectory, filePath);
                var fileInfo = new FileInfo(fileName);
                // 判断文件是否存在
                if (!fileInfo.Exists)
                {
                    var message = $"文件 {fileName} 不存在";
                    _logger.Error(message);
                    return new ErrorResult(message);
                }

                var objectName = Path.Combine(objectDir, Path.GetFileName(fileName)).Replace("\\", "/");

                // 获取预上传的url
                var uploadUrl = client.PresignedPutObjectAsync(bucketName, objectName, 24 * 60 * 60).GetAwaiter().GetResult();

                var fs = fileInfo.OpenRead();
                var sc = new StreamContent(fs);
                HttpResponseMessage resMessage = httpClient.PutAsync(uploadUrl, sc).GetAwaiter().GetResult();
                fs.Close();
                if (!resMessage.IsSuccessStatusCode)
                {
                    _logger.Error(resMessage.ReasonPhrase);
                    continue;
                }


                // 异步当成同步调用
                // client.PutObjectAsync(bucketName, objectName, fileName).Wait();

                // 添加到成果中
                resultUrls.Add($"/{bucketName}/{objectName}");
            }

            return new SuccessResult();
        }

        // 从剪切板上传
        private IResult<JToken> UploadFromClipboard(HttpClient httpClient, MinioClient client, string bucketName, string objectDir, ref List<string> resultUrls)
        {
            if (FromClipboard)
                _logger.Info("正在从剪切板上传");

            if (Clipboard.ContainsImage())
            {
                // 上传图片
                var img = Clipboard.GetImage();
                var mStream = new MemoryStream();
                img.Save(mStream, ImageFormat.Png);

                var sc = new StreamContent(mStream);

                string fileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";

                var objectName = Path.Combine(objectDir, fileName).Replace("\\", "/");
                // 获取预上传的url
                var uploadUrl = client.PresignedPutObjectAsync(bucketName, objectName, 24 * 60 * 60).GetAwaiter().GetResult();

                HttpResponseMessage resMessage = httpClient.PutAsync(uploadUrl, sc).GetAwaiter().GetResult();
                if (resMessage.IsSuccessStatusCode)
                {
                    resultUrls.Add($"/{bucketName}/{objectName}");
                }
            }

            if (Clipboard.ContainsFileDropList())
            {
                // 从复制的路径中上传文件
                var filePaths = Clipboard.GetFileDropList();
            }

            return new SuccessResult();
        }
    }
}
