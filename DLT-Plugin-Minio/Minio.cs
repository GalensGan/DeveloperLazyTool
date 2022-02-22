using CommandLine;
using DeveloperLazyTool.Plugin;
using log4net;
using Minio;
using Newtonsoft.Json.Linq;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Text;
using System.Threading.Tasks;
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

        private ILog _logger = LogManager.GetLogger(typeof(Minio));
        public override IResult<JToken> RunOneCommand(JObject verbSetting)
        {
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

                using (ProgressBar progressBar = new ProgressBar(1000, $"开始上传文件至 minio", _options))
                {
                    _filesCount = FilePaths.Count();
                    _progressBar = progressBar;

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

                        // 判断桶是否存在
                        if (!client.BucketExistsAsync(bucketName).GetAwaiter().GetResult())
                        {
                            var message = $"桶: {bucketName} 不存在";
                            _logger.Error(message);
                            return new ErrorResult(message);
                        }

                        var objectName = Path.Combine(objectDir, Path.GetFileName(fileName)).Replace("\\", "/");

                        // 获取预上传的url
                        var uploadUrl = client.PresignedPutObjectAsync(bucketName, objectName, 24 * 60 * 60).GetAwaiter().GetResult();

                        using (MultipartFormDataContent multipartFormData = new MultipartFormDataContent())
                        {
                            multipartFormData.Add(new StreamContent(fileInfo.OpenRead()), "file", fileInfo.Name);
                            HttpResponseMessage resMessage = httpClient.PutAsync(uploadUrl, multipartFormData).GetAwaiter().GetResult();
                            if (!resMessage.IsSuccessStatusCode)
                            {
                                resultUrls.Add(resMessage.ReasonPhrase);
                                continue;
                            }
                        }

                        // 异步当成同步调用
                        // client.PutObjectAsync(bucketName, objectName, fileName).Wait();

                        // 向命令行输出路径
                        var url = $"{protocol}://{endpoint}/{bucketName}/{objectName}";
                        resultUrls.Add(url);
                    }

                    _progressBar.AsProgress<double>().Report(1);
                }

                // 写入命令行
                // 要是进度条外面写才能显示
                Console.WriteLine("上传成功:");
                foreach (var resultUrl in resultUrls) Console.WriteLine(resultUrl);

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

        private ProgressBar _progressBar = null;
        private int _filesCount = 1;
        private int _currentIndex = 0;
    }
}
