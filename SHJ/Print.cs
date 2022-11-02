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
        private Print()
        {
        }
        private static Print _Pirnt = null;
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static Print GetExample()
        {
            if (_Pirnt == null)
            {
                _Pirnt = new Print();
            }
            return _Pirnt;
        }

        /// <summary>
        /// 打印机错误监控
        /// </summary>
        /// <returns></returns>
        public string PrintFaultInspect()
        {
            if (PEPrinter.PEPrinterState == 65535)
            {
                return "打印机未连接";
            }
            else if(PEPrinter.PEPrinterState > 0x8000)
            {
                PrintReset();
                return PEPrinter.PEPrinterStatedetail;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 打印机复位
        /// </summary>
        private void PrintReset()
        {
            PEPrinter.needReset = true;
        }
        
        /// <summary>
        /// 启动打印程序
        /// </summary>
        public void PrintAction()
        {
            if ((PEPrinter.TrayCondition & 0x01) == 0x01)//托盘已经归位
            {
                PEPrinter.TYPE_STAMP mytype;
                int osmotype = 3;
                try
                {
                    osmotype = int.Parse(Form1.mynodelisthuodao[Form1.wulihuodao].Attributes.GetNamedItem("position").Value);
                }
                catch
                {
                }
                switch (osmotype)
                {
                    case 1:
                        mytype = PEPrinter.TYPE_STAMP.TYPE_1010;
                        break;
                    case 2:
                        mytype = PEPrinter.TYPE_STAMP.TYPE_2020;
                        break;
                    case 3:
                        mytype = PEPrinter.TYPE_STAMP.TYPE_2530;
                        break;
                    default:
                        mytype = PEPrinter.TYPE_STAMP.TYPE_2530;
                        break;
                }
                try
                {
                    PEPrinter.CreateProcessingData(PEPrinter.PicPath, mytype);
                }
                catch (Exception ex)
                {
                }
                PEPrinter.needPutImage = true;//加载图片并打印
            }
        }
    }
}
