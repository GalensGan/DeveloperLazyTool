using CommandLine;
using DeveloperLazyTool.Plugin;
using FluentFTP;
using log4net;
using Newtonsoft.Json.Linq;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WowToolAPI.Utils.Core;

namespace DLTPlugins.FTP
{
    /// <summary>
    /// FTP插件
    /// </summary>
    [Verb("ftp", HelpText = "上传文件或者文件夹到ftp")]
    public class FTP : PluginBase
    {
        private ILog _logger = LogManager.GetLogger(typeof(FTP));
        public override IResult<JToken> RunOneCommand(JObject verbSettings)
        {
            JObject jobj = verbSettings;

            // 获取数据
            string name = jobj.Value<string>("name");
            string host = jobj.Value<string>("host");
            int port = jobj.Value<int>("port");
            string username = jobj.Value<string>("username");
            string password = jobj.Value<string>("password");
            string localPath = jobj.Value<string>("localPath");
            string remotePath = jobj.Value<string>("remotePath") ?? "/";

            _logger.Info($"开始上传 {name} 任务");

            using (ProgressBar progressBar = new ProgressBar(1000, $"开始上传 ftp {Name}", _options))
            {
                _progressBar = progressBar;
                _lastPercent = 0;

                // 判断数据
                if (string.IsNullOrEmpty(host))
                {
                    string message = $"ftps {name} 配置中 host 为空";
                    _logger.Info(message);
                    return new ErrorResult(message);
                }

                if (string.IsNullOrEmpty(host))
                {
                    string message = $"ftps {name} 配置中 host 为空";
                    _logger.Info(message);
                    return new ErrorResult(message); ;
                }

                if (!File.Exists(localPath) && !Directory.Exists(localPath))
                {
                    string message = $"ftps {name} 配置中 localPath 不存在";
                    _logger.Info(message);
                    return new ErrorResult(message); ;
                }

                if (port == 0) port = 21;


                // create an FTP client
                using (FtpClient client = new FtpClient(host)
                {
                    RetryAttempts = 3,
                    // specify the login credentials, unless you want to use the "anonymous" user account
                    Credentials = new NetworkCredential(username, password),
                    Port = port,
                    Encoding = Encoding.UTF8,
                })
                {
                    // begin connecting to the server
                    client.Connect();

                    // 开启 utf8 编码
                    FtpReply ftpReply = client.Execute("OPTS UTF8 ON");
                    if (!ftpReply.Code.Equals("200") && !ftpReply.Code.Equals("202"))
                        client.Encoding = Encoding.GetEncoding("ISO-8859-1");

                    // 判断是否是文件
                    if (File.Exists(localPath))
                    {
                        // 上传文件
                        client.UploadFile(localPath, remotePath, FtpRemoteExists.Overwrite, progress: UploadProgress);
                    }
                    else
                    {
                        client.UploadDirectory(localPath, remotePath, FtpFolderSyncMode.Update, FtpRemoteExists.Overwrite, progress: UploadProgress);
                    }

                    _progressBar.AsProgress<double>().Report(1);
                    _progressBar = null;
                }


            }
            _logger.Info("上传完成");

            // 修改参数
            return new SuccessResult();
        }

        private ProgressBarOptions _options = new ProgressBarOptions
        {
            ProgressCharacter = '=',
            ProgressBarOnBottom = true,
            DisplayTimeInRealTime = false,
            MessageEncodingName = "utf-8"
        };

        private ProgressBar _progressBar = null;

        private double _lastPercent = 0;
        private void UploadProgress(FtpProgress progress)
        {
            double accumulatePercent = progress.FileIndex * 1.0 / (progress.FileCount);
            double currentPercent = accumulatePercent + progress.Progress / progress.FileCount / 100;

            // 控制刷新            
            if (currentPercent > _lastPercent)
            {
                _lastPercent += 0.01;

                // 更改信息
                //  _progressBar.Tick($"正在上传: {Path.GetFileName(progress.LocalPath)} {Math.Round(progress.TransferSpeed / 1024 / 1024, 3)}M/s");
                _progressBar.AsProgress<double>().Report(currentPercent);
            }
        }
    }
}
