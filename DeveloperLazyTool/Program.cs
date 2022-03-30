using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DeveloperLazyTool.Core.Plugin;

namespace DeveloperLazyTool.Core
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // Console.WriteLine(string.Join("__", args));
            // System.Windows.Forms.MessageBox.Show(string.Join(",", args));

            // args = new string[] { "ftp","dlt" };
            // args = new string[] { "config","u"};
            // args = new string[] { "uninstall"};
            // args = new string[] { "cmd","-n","ahk"};
            // args = new string[] { "cmd","-q","true","-n","cd-backend"};
            // args = new string[] { "cd-backend", "-q", "true", };
            // args = new string[] { "agg", "uploadDlt" };
            // args = new string[] { "list", "ftps" };
            // args = new string[] { "es", "testenv" };
            // args = new string[] { "es","addmac", "-b" };
            // args = new string[] { "plugin","add", "-n","FTP","-p", "DLT-Plugins-FTP.dll" };
            // args = new string[] { "minio", "img", "-p", "ToDesk_Lite.exe" };
            // args = new string[] { "exp" };

            var plugin = PluginFactory.Instance.GetPlugin(args);
            plugin?.StartCommand();
        }
    }
}
