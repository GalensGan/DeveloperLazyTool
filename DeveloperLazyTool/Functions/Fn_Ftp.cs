using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DeveloperLazyTool.Options;
using FluentFTP;
using log4net;
using Newtonsoft.Json.Linq;
using ShellProgressBar;

namespace DeveloperLazyTool.Functions
{
    /// <summary>
    /// 上传文件到 ftp
    /// </summary>
    public class Fn_Ftp : FuncBase
    {
        private Opt_Ftp _option;
        private ILog _logger = LogManager.GetLogger(typeof(Fn_Ftp));

        public override void SetParams(OptionBase optionBase)
        {
            base.SetParams(optionBase);

            _option = ConvertParams<Opt_Ftp>();
        }

        public override void Run()
        {
            // 是否要提示
            if (!_option.Quiet)
            {
                string message = null;
                if (string.IsNullOrEmpty(_option.Name)) {
                    message = "是否运行所有 Ftp 配置(Y/N)?";
                }
                else
                {
                    message = $"是否运行 Ftp 配置 {_option.Name}(Y/N)?";
                }
                Console.WriteLine(message);
                string command = Console.ReadLine();
                if(string.IsNullOrEmpty(command) || command.ToLower() != "y")
                {
                    _logger.Warn("取消上传");
                    return;
                }
            }

            // 调用 ftp 上传
            JArray jArray = _option.JObject.Value<JArray>("ftps");

            if (jArray == null || jArray.Count < 1)
            {
                _logger.Info("ftps 配置为空");
                return;
            }

            // 如果传入了 name,则执行特定的项
            if (string.IsNullOrEmpty(_option.Name))
            {
                // 调用上传模块
                jArray.ToList().ForEach(jt =>
                {
                    FtpUpload(jt);

                });

            }
            else
            {
                // 找到指定name的配置
                var jtoken = jArray.ToList().Find(jt => jt.Value<string>("name") == _option.Name);
                if (jtoken != null)
                {
                    FtpUpload(jtoken);
                }
            }
        }

        private ProgressBarOptions _options = new ProgressBarOptions
        {
            //ForegroundColor = ConsoleColor.Yellow,
            //ForegroundColorDone = ConsoleColor.DarkGreen,
            //BackgroundColor = ConsoleColor.DarkGray,
            // BackgroundCharacter = '\u2593',
            DisplayTimeInRealTime = false,

            ProgressCharacter = '─',
            ProgressBarOnBottom = true
        };

        private ProgressBar _progressBar = null;

        private void FtpUpload(JToken jt)
        {

            // 获取数据
            string name = jt.Value<string>("name");
            string host = jt.Value<string>("host");
            int port = jt.Value<int>("port");
            string username = jt.Value<string>("username");
            string password = jt.Value<string>("password");
            string localPath = jt.Value<string>("localPath");
            string remotePath = jt.Value<string>("remotePath") ?? "/";

            _logger.Info($"开始上传 {name} 任务");

            using (ProgressBar progressBar = new ProgressBar(1000, $"开始上传 ftp {name}", _options))
            {
                _progressBar = progressBar;
                _lastPercent = 0;

                // 判断数据
                if (string.IsNullOrEmpty(host))
                {
                    _logger.Info($"ftps{name}配置中 host 为空");
                    return;
                }

                if (string.IsNullOrEmpty(localPath))
                {
                    _logger.Info($"ftps{localPath}配置中 host 为空");
                    return;
                }

                if (!File.Exists(localPath))
                {
                    _logger.Info($"路径 {localPath} 不存在");
                    return;
                }

                if (port == 0) port = 21;


                // create an FTP client
                using FtpClient client = new FtpClient(host)
                {
                    RetryAttempts = 3,
                    // specify the login credentials, unless you want to use the "anonymous" user account
                    Credentials = new NetworkCredential(username, password),
                    Port = port,
                    Encoding = Encoding.UTF8,
                };

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

            _logger.Info("上传完成");
        }

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
