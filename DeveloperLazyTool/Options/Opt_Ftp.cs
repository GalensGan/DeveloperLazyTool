﻿using CommandLine;
using DeveloperLazyTool.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Options
{
    [Verb("ftp", HelpText = "上传文件或者文件夹到ftp")]
    public class Opt_Ftp : OptionBase
    {
        [Option(HelpText = "上传的配置名称", Required = false)]
        public string Name { get; set; }
    }
}