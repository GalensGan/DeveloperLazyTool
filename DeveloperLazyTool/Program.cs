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
            // Console.WriteLine(string.Join("__", args));
            //System.Windows.Forms.MessageBox.Show(string.Join(",", args));

            // args = new string[] { "ftp","dlt" };
            // args = new string[] { "config","u"};
            // args = new string[] { "uninstall"};
            // args = new string[] { "cmd","-n","ahk"};
            // args = new string[] { "cmd","-q","true","-n","cd-backend"};
            // args = new string[] { "cd-backend", "-q", "true", };
            // args = new string[] { "agg", "uploadDlt" };
            // args = new string[] { "list", "ftps" };
            // args = new string[] { "es", "ipecversion" };
            // args = new string[] { "es","addmac", "-b" };

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
                // 运行命令
                options.StartFunction();
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
