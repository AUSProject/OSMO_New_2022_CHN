using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHJ
{
    class Machine
    {
        public Machine()
        {
            
        }
        public static short runCode;//运行代码
        public static short faultCode;//错误代码
        public static short mainCode;//主控程序
        /// <summary>
        /// <para>机器当前运行步骤</para>
        /// 00:空闲 01:取盒子 02:抓取印面 03:等待打印  04:正在打印  05:正在组装 06:正在出货 07:等待取货 98:打印机故障 99:机器故障 
        /// </summary>
        public static byte nowStep;
        public static bool runToken;//是否运行
        /// <summary>
        /// PLC自动运行程序
        /// </summary>
        /// <param name="action">是否运行</param>
        public void PlcAutoControl()
        {
            GetFaultCode();
            if (runToken)
            {
                GetMainCode();
                switch (mainCode)
                {
                    case 0:
                        break;
                    case 1:
                        InstallPrintFace();//托盘弹出和回收，开始打印
                        break;
                    case 2:
                        break;
                    case 3:
                        PrintTakeBack();//托盘回收归零,开始组装
                        break;
                    case 4:
                        break;
                    case 5:
                        Shipment();
                        break;
                    case 6:
                        IniMachine();
                        break;
                    case 7:
                        break;
                }
            }
        }

        /// <summary>
        /// 监控机器错误代码
        /// </summary>
        private void GetFaultCode()
        {
            faultCode=new PCHMI.VAR().GET_INT16(0, "D209");
            if (faultCode != 0)//报错则停止运行
            {
                runToken = false;
            }
            else
            {
                runToken = true;
            }
        }

        /// <summary>
        /// 获取程序当前步骤
        /// </summary>
        /// <returns></returns>
        private short GetMainCode()
        {
            mainCode = new PCHMI.VAR().GET_INT16(0, "D15");
            return mainCode;
        }

        private  bool trayOut = false;//托盘是否弹出
        private  bool trayBack = false;//托盘是否回收打印
        private  bool trayReZero = false;//托盘是否归零
        /// <summary>
        /// 安装印面监控程序
        /// </summary>
        private void InstallPrintFace()
        {
            short D11 = new PCHMI.VAR().GET_INT16(0, "D11");
            if (D11 >= 0 && D11 < 3)
            {
                nowStep = 0x01;
            }
            else if(D11 >= 3 && D11 < 6)
            {
                nowStep = 0x02;
            }
            else if (D11 >= 6 && D11 < 10 && !trayOut)
            {
                nowStep = 0x03;
                trayOut = true;
                PEPrinter.needMoveTray = 4;//弹出托盘
            }
            else if (D11 == 10 && trayOut)
            {
                trayBack = true;
                trayOut = false;
                PEPrinter.needMoveTray = 3;//回收托盘进行打印
            }
            else if (((PEPrinter.TrayCondition & 0x01) == 0x01) && trayBack && !String.IsNullOrEmpty(PEPrinter.PicPath))//开始打印
            {
                trayBack = false;
                nowStep = 0x04;
                Form1.isextbusy = 2;
            }
            else { }
        }

        /// <summary>
        /// 收回托盘程序
        /// </summary>
        private void PrintTakeBack()
        {
            short D7 = new PCHMI.VAR().GET_INT16(0, "D7");
            if (D7 > 5 && !trayReZero)
            {
                trayReZero = true;
                nowStep = 0x05;
                PEPrinter.needMoveTray = 1;//回收托盘
            }
        }

        /// <summary>
        /// 出货过程
        /// </summary>
        private void Shipment()
        {
            short D9 = new PCHMI.VAR().GET_INT16(0, "D9");
            if (D9 < 5 && D9>0)
            {
                nowStep = 0x06;
            }
            else if (D9 >= 8)
            {
                nowStep = 0x07;
            }
        }

        /// <summary>
        /// 还原机器状态
        /// </summary>
        private void IniMachine()
        {
            short D10 = new PCHMI.VAR().GET_INT16(0, "D10");
            if (D10 >= 0)
            {
                trayReZero = false;
                trayBack = false;
                trayOut = false;
                nowStep = 0x00;
            }
        }
    }
}
