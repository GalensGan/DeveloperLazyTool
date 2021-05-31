using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DeveloperLazyTool.Options;
using DeveloperLazyTool.Modules;

namespace DeveloperLazyTool
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            //System.Windows.Forms.MessageBox.Show(string.Join(",", args));

            // args = new string[] { "ftp","-n","dist" };
            // args = new string[] { "config","u"};
            // args = new string[] { "uninstall"};
            // args = new string[] { "bat"};

            var types = LoadVerbs();

            Parser.Default
                .ParseArguments(args, types)
                .WithNotParsed(HandleParseError)
                .WithParsed(Run);
        }

        //load all types using Reflection
        private static Type[] LoadVerbs()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray();
        }

        private static void Run(object obj)
        {
            if (obj is OptionBase options)
            {
                // 注入配置文件
                ArgumentFactory argumentFactory = new ArgumentFactory(options.BaseDir,options.PathData,options.UserConfigName);
                options.SetArgumentFactory(argumentFactory);
                // 运行命令
                options.RunFunction();
            }
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            if (errs.IsVersion())
            {
                Console.WriteLine("Version Request");
                return;
            }

            if (errs.IsHelp())
            {
                Console.WriteLine("Help Request");
                return;
            }
            Console.WriteLine("Parser Fail");
        }
    }
}
