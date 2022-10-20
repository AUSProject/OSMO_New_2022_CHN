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
        public short runCode;//运行代码
        public static short faultCode;//错误代码
        public short mainCode;//主控程序
        public string curBit;//当前位
        public short curData;//当前位数据
        public static int time = 150;
        public int runTiming = time;
        
        /// <summary>
        /// <para>机器当前运行步骤</para>
        /// 00:空闲 01:取盒子 02:抓取印面 03:等待打印  04:正在打印  05:正在组装 06:正在出货 07:等待取货 98:打印机故障 99:机器故障
        /// </summary>
        public static byte nowStep;

        /// <summary>
        /// PLC自动运行程序
        /// </summary>
        /// <param name="action">是否运行</param>
        public void PlcAutoControl(bool action)
        {
            if (action)
            {
                GetCode();
                switch (mainCode)
                {
                    case 0:
                        break;
                    case 1:
                        curBit = "D11：";
                        InstallPrintFace();//托盘弹出和回收，开始打印
                        break;
                    case 2:
                        curBit = "D6：";
                        curData = new PCHMI.VAR().GET_INT16(0, "D6");
                        break;
                    case 3:
                        curBit = "D7：";
                        PrintTakeBack();//托盘回收归零,开始组装
                        break;
                    case 4:
                        curBit = "D8：";
                        curData = new PCHMI.VAR().GET_INT16(0, "D8");
                        break;
                    case 5:
                        curBit = "D9：";
                        Shipment();//出货
                        break;
                    case 6:
                        curBit = "D10：";
                        WaitShip();//等待取货
                        break;
                    case 7:
                        IniMachine();//复位
                        break;
                    case 98:
                        nowStep = 0x98;
                        break;
                    case 99:
                        nowStep = 0x99;
                        break;
                }
            }
        }

        /// <summary>
        /// 监控机器错误代码和运行代码
        /// <para>获取当前步骤</para>
        /// </summary>
        private void GetCode()
        {
            faultCode=new PCHMI.VAR().GET_INT16(0, "D209");
            runCode = new PCHMI.VAR().GET_INT16(0, "D208");
            mainCode = new PCHMI.VAR().GET_INT16(0, "D15");
            if(faultCode != 0)//报错
            {
                mainCode = 99;
            }
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
            curData = D11;
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
            curData = D7;
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
            curData = D9;
            if (D9 >= 4)
            {
                nowStep = 0x06;
            }
        }

        /// <summary>
        ///等待取货
        /// </summary>
        private void WaitShip()
        {
            short D10 = new PCHMI.VAR().GET_INT16(0, "D10");
            curData = D10;
            if (D10 >= 0)
            {
                nowStep = 0x07;
            }
        }

        /// <summary>
        /// 复位数据
        /// </summary>
        private void IniMachine()
        {
            if (runCode == 0)
            {
                trayReZero = false;
                trayBack = false;
                trayOut = false;
                nowStep = 0x00;
                runTiming = time;
                Form1.HMIstep = 1;
            }
        }

        /// <summary>
        /// 检测是否还有印面
        /// <para>True:有 False:没有</para>
        /// </summary>
        /// <returns></returns>
        public static bool GoodsInspect()
        {
            Int16 num = new PCHMI.VAR().GET_INT16(0, "D202");
            if (num == 0)
                return false;
            else
                return true;
        }

        private string[] errorList =
        {
            "打印机托盘错误",
             "没有打印",
             "出料位置错误",
             "装配位置检测",
             "存盖子位置检测",
             "抓手故障",
             "料槽位置印面取料失败",
             "盖子存放位置放置失败",
             "盒子取料超时" ,
             "盖子存储位置取料失败",
             "装配位置成品取料失败",
             "成品出料槽放置成品失败"
        };
        /// <summary>
        /// 机器故障显示
        /// <para>返回故障信息</para>
        /// </summary>
        public static string FaultShow()
        {
            string eMsg = "";
            if (faultCode != 0)
            {
                string error = Convert.ToString(faultCode, 2);
            }
            else
            {
                eMsg = null;
            }
            return eMsg;
        }

        /// <summary>
        /// 检查设备是否连接
        /// <para>true:连接 false:未连接</para>
        /// </summary>
        public static bool CheckPortConnect()
        {
            string[] gcom = System.IO.Ports.SerialPort.GetPortNames();
            if (gcom.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
