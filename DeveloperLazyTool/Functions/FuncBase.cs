using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLazyTool.Functions
{
    public abstract class FuncBase
    {
        protected Options.OptionBase Option { get; private set; }

        public abstract void Run();

        /// <summary>
        /// 重写时，必须先调用父类方法
        /// </summary>
        /// <param name="optionBase"></param>
        public virtual void SetParams(Options.OptionBase optionBase)
        {
            Option = optionBase;
        }

        protected T ConvertParams<T>(Options.OptionBase optionBase) where T : Options.OptionBase
        {
            return (T)optionBase;
        }

        /// <summary>
        /// 转换数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T ConvertParams<T>() where T:Options.OptionBase
        {
            if (Option == null) return null;

            return (T)Option;
        }
    }
}
