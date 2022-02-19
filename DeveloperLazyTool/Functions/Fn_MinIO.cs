using DeveloperLazyTool.Core.Enums;
using DeveloperLazyTool.Core.Extensions;
using DeveloperLazyTool.Core.Modules;
using DeveloperLazyTool.Core.Options;
using log4net;
using Minio;
using Minio.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Core.Functions
{
    class Fn_MinIO : FuncBase
    {
        private Opt_MinIO _option;
        private ILog _logger = LogManager.GetLogger(typeof(Fn_Ftp));


        public Fn_MinIO(StdInOut stdInOut) : base(stdInOut)
        {
            _option = InputParams.CmdOptions as Opt_MinIO;
        }

        // 运行命令
        public override StdInOut Run()
        {
            var endpoint = InputParams.CmdConfig.SelectTokenPlus(FieldNames.endpoint, string.Empty);
            var accessKey = InputParams.CmdConfig.SelectTokenPlus(FieldNames.accessKey, string.Empty);
            string secretKey = InputParams.CmdConfig.SelectTokenPlus(FieldNames.secretKey, string.Empty);
            string region = InputParams.CmdConfig.SelectTokenPlus(FieldNames.region, string.Empty);
            string sessionToken = InputParams.CmdConfig.SelectTokenPlus(FieldNames.sessionToken, string.Empty);

            try
            {
                var minioClient = new MinioClient(endpoint, accessKey, secretKey, region, sessionToken).WithSSL();
                AsyncRun(minioClient).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            

            return InputParams;
        }

        private async Task AsyncRun(MinioClient minio)
        {
            string bucketName = InputParams.CmdConfig.SelectTokenPlus(FieldNames.bucket, string.Empty);
            try
            {
                // Make a bucket on the server, if not already present.
                bool found = await minio.BucketExistsAsync(bucketName);
                if (!found)
                {
                    //await minio.MakeBucketAsync(bucketName, location);
                    return;
                }

                // 验证文件是否存在
                foreach (var path in _option.Paths)
                {
                    // 如果是网络图片，则直接返回
                    if (path.StartsWith("http"))
                    {
                        Console.Write(path);
                        continue;
                    }

                    // 开始进行上传
                    await UploadObject(minio,bucketName,path);
                }                
            }
            catch (MinioException e)
            {
                Console.WriteLine("File Upload Error: {0}", e.Message);
            }
        }

        private async Task UploadObject(MinioClient minio,string bucketName, string filePath)
        {
            FileInfo finfo = new FileInfo(filePath);
            if (!finfo.Exists) return;


            // 计算hash值
            var sha256 = SHA256.Create();
            FileStream fs = finfo.OpenRead();
            var bytes = sha256.ComputeHash(fs);
            fs.Close();
            var hash = BitConverter.ToString(bytes).Replace("-", "");

            string objectName = $"{hash}_{finfo.Name}"

            // Upload a file to bucket.
            await minio.PutObjectAsync(bucketName, objectName, filePath, contentType);

            Console.WriteLine("Successfully uploaded " + objectName);

            Console.Write(_option.Params);
        }
    }
}
