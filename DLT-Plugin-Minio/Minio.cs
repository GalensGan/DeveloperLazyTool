using CommandLine;
using DeveloperLazyTool.Plugin;
using log4net;
using Minio;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WowToolAPI.Utils.Core;
using WowToolAPI.Utils.Extensions;

namespace DLTPlugin.Minio
{
    [Verb("minio", HelpText = "向minio上传文件")]
    internal class Minio : PluginBase
    {
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

                Console.WriteLine("上传结果:");
                var protocol = "http";
                if (useSSL) protocol = "https";

                foreach (var filePath in FilePaths)
                {
                    // 生成文件名字
                    var fileName = Path.Combine(Environment.CurrentDirectory, filePath);
                    // 判断文件是否存在
                    if (!File.Exists(fileName))
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

                    var objectName = Path.Combine(objectDir, Path.GetFileName(fileName)).Replace("\\","/");

                    // 获取预上传的url
                    //  var uploadUrl = client.PresignedPutObjectAsync(bucketName, objectName, 24 * 60 * 60);

                    // 利用 httpclient 上传

                    // 异步当成同步调用
                    client.PutObjectAsync(bucketName, objectName, fileName).Wait();

                    // 向命令行输出路径
                    var url = $"{protocol}://{endpoint}/{bucketName}/{objectName}";
                    Console.WriteLine(url);
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
    }
}
