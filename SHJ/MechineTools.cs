using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHJ
{
    class MechineTools
    {
        public MechineTools()
        {
            
        }
        public short runCode;//运行代码
        public short faultCode;//错误代码
        public short mainCode;//主控程序
        public byte nowStep;//机器当前进行步骤  0x00:等待  0x01 :放置印面  0x02:取盖子，存盖子  0x03:装配印面  0x04:装配盖子  0x05:出料  0x06:出料及回零
        public bool runToken;//是否运行
        /// <summary>
        /// PLC自动运行程序
        /// </summary>
        /// <param name="action">是否运行</param>
        public void PlcAutoControl(bool action)
        {
            GetFaultCode();
            if (runToken && action)
            {
                GetMainCode();
                switch (mainCode)
                {
                    case 0:
                        nowStep = 0x00;
                        break;
                    case 1:
                        InstallPrintFace();
                        nowStep = 0x01;
                        break;
                    case 2:
                        nowStep = 0x02;
                        break;
                    case 3:
                        PrintTakeBack();
                        nowStep = 0x03;
                        break;
                    case 4:
                        nowStep = 0x04;
                        break;
                    case 5:
                        nowStep = 0x05;
                        break;
                    case 6:
                        IniMachine();
                        nowStep = 0x06;
                        break;
                    case 7:
                        nowStep = 0x07;
                        break;
                }
            }
        }

        /// <summary>
        /// 监控机器是否报错
        /// 报错:True,无报错:false
        /// </summary>
        /// <returns>报错:True,无报错:false</returns>
        public bool GetFaultCode()
        {
            faultCode=new PCHMI.VAR().GET_INT16(0, "D209");
            if (faultCode != 0)
            {
                runToken = false;
                return true;
            }
            else
            {
                runToken = true;
                return false;
            }
        }

        /// <summary>
        /// 获取程序当前步骤
        /// </summary>
        /// <returns></returns>
        public short GetMainCode()
        {
            mainCode = new PCHMI.VAR().GET_INT16(0, "D15");
            return mainCode;
        }

        private bool trayOut = false;//托盘是否弹出
        private bool trayBack = false;//托盘是否回收
        /// <summary>
        /// 安装印面监控程序
        /// </summary>
        public void InstallPrintFace()
        {
            short D11 = new PCHMI.VAR().GET_INT16(0, "D11");
            if (D11 == 6 && !trayOut)
            {
                trayOut = true;
                PEPrinter.needMoveTray = 4;//弹出托盘
            }
            else if (D11 == 10 && trayOut)
            {
                trayBack = true;
                PEPrinter.needMoveTray = 3;//回收托盘进行打印
            }
            else if (((PEPrinter.TrayCondition & 0x01) == 0x01) && trayBack && !String.IsNullOrEmpty(PEPrinter.PicPath))//开始打印
            {
                Form1.isextbusy = 2;
            }
            else { }
        }

        /// <summary>
        /// 收回托盘程序
        /// </summary>
        public void PrintTakeBack()
        {
            short D7 = new PCHMI.VAR().GET_INT16(0, "D7");
            if (D7 >= 5)
            {
                PEPrinter.needMoveTray = 1;//回收托盘
            }
        }

        /// <summary>
        /// 还原机器状态
        /// </summary>
        public void IniMachine()
        {
            trayBack = false;
            trayOut = false;
            nowStep = 0x00;
        }
    }
}
