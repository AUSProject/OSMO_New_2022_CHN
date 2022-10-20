using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHJ
{
    public class Print
    {
        /// <summary>
        /// 打印机错误检测
        /// <para>无错误返回null</para>
        /// </summary>
        /// <returns></returns>
        public static string PrintFaultInspect()
        {
            if (PEPrinter.PEPrinterState == 65535)
            {
                return "打印机未连接";
            }
            else if(PEPrinter.PEPrinterState > 0x8000)
            {
                return PEPrinter.PEPrinterStatedetail;
            }
            else
            {
                return null;
            }
        }
        
    }
}
