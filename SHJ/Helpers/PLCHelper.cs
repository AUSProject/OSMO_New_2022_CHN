using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHJ
{
    /// <summary>
    /// PLC控制类
    /// <para>设备的自动和手动控制</para>
    /// </summary>
    class PLCHelper
    {
        public PLCHelper()
        {
            print = PrintHelper.GetExample();
            log = LogHelper.GetLogHelperExamlpe();
            curState = 0x90;
        }

        #region Feild

        /// <summary>
        /// <para>机器当前运行步骤</para>
        /// 00:空闲 01:取盒子 02:抓取印面 03:等待打印  04:正在打印  05:正在组装 06:正在出货 07:等待取货 
        /// <para>70:打印机正在复位 71:托盘内有印面 80:机器正在复位 81:机器正在排故 98:打印机故障 99:机器故障</para>
        /// </summary>
        public static byte nowStep;
        
        private PrintHelper print = null;
        private LogHelper log;

        public static bool _RunEnd=false;//机器运行结束
        public static bool isAutoRun = true;
        public short runCode;//运行代码
        public static short faultCode;//错误代码
        public short mainCode;//主控程序
        public int runTiming = 150;//运行总时长
        public static bool isRigPrint;//是否装配印面 false:未安装  true:已安装 
        public static bool rigPrinting;//正在安装印面
        /// <summary>
        /// 运行故障标志
        /// </summary>
        public static bool errorToken;
        public static string errorMsg=null;//故障信息
        
        #endregion

        #region 控制地址

        /// <summary>
        /// 系统复位
        /// <para>1开始,4结束</para>
        /// </summary>
        public short D5 = 0;
        /// <summary>
        /// 装配印面
        /// <para>1开始,10结束</para>
        /// </summary>
        public short D11 = 0;
        /// <summary>
        /// 货道控制
        /// 1开始,5结束
        /// </summary>
        public short D0 = 0;
        /// <summary>
        /// 取盖子
        /// <para>1开始,10结束</para>
        /// </summary>
        public short D6 = 0;
        /// <summary>
        /// 从打印机位置取料装配
        /// <para>1开始,10结束</para>
        /// </summary>
        public short D7 = 0;
        /// <summary>
        /// 装配盖子
        /// <para>1开始,11结束</para>
        /// </summary>
        public short D8 = 0;
        /// <summary>
        /// 装配位置出料
        /// <para>1开始,13结束</para>
        /// </summary>
        public short D9 = 0;
        /// <summary>
        /// 出成品和回零
        /// <para>1开始,2结束</para>
        /// </summary>
        public short D10 = 0;
        /// <summary>
        /// 出料位置检测
        /// 置1故障
        /// </summary>
        private ushort X12 = 0;
        /// <summary>
        /// 装配位置检测
        /// 置1故障
        /// </summary>
        private ushort X10 = 0;
        /// <summary>
        /// 存盖子位置检测
        /// 置1故障
        /// </summary>
        private ushort X7 = 0;
        /// <summary>
        /// 印面数量检测
        /// 置1缺少
        /// </summary>
        private ushort X13 = 0;
        /// <summary>
        /// 印面是否到位
        /// </summary>
        private ushort X6 = 0;
        /// <summary>
        /// 排故程序1
        /// <para>1开始,10结束</para>
        /// </summary>
        private short D12 = 0;
        /// <summary>
        /// 排故程序2
        /// <para>1开始,12结束</para>
        /// </summary>
        private short D13 = 0;
        /// <summary>
        /// 排故程序3
        /// <para>1开始,10结束</para>
        /// </summary> 
        private short D14 = 0;

        private short M35 = 0;//X轴运动判断
        private short M34 = 0;//X轴运动判断


        private void IniData()
        {
            new PCHMI.VAR().SEND_INT16(0, "D0",0);
            new PCHMI.VAR().SEND_INT16(0, "D6",0);
            new PCHMI.VAR().SEND_INT16(0, "D7",0);
            new PCHMI.VAR().SEND_INT16(0, "D8",0);
            new PCHMI.VAR().SEND_INT16(0, "D9",0);
            new PCHMI.VAR().SEND_INT16(0, "D10",0);
            new PCHMI.VAR().SEND_INT16(0, "D11", 0);
        }

        #endregion
        
        /// <summary>
        /// 设备运行
        /// </summary>
        public void MachineRun(int number)
        {
            if (runTiming <= 0)
            {
                errorToken = true;
                errorMsg += "未知故障";
            }
            //if (print.PrintFaultInspect() != null)
            //{
            //    errorToken = true;
            //    errorMsg += "打印机故障";
            //}
            if (isAutoRun)
            {
                if (isRigPrint)
                {
                    errorToken = true;
                    errorMsg += "印章机内有印面";
                }
                else
                {
                    _RunEnd = false;
                    if (!sendOver)
                    {
                        StartRunning(number);
                        sendOver = true;
                        Form1.ReduceStock(number.ToString());//印章库存减一
                    }
                    PrintControlAndShow();
                }
            }
            else
            {
                MachineControlAndMonitoring(number);
            }

            if (errorToken)//故障则复位程序
            {
                ResetProgram();
            }
        }

        #region PLC自动控制
        
        /// <summary>
        /// 打印机控制
        /// </summary>
        private void PrintControlAndShow()
        {
            GetCode();
            if (print.PrintFaultInspect() != null)//打印机报错
            {
                mainCode = 99;
                errorToken = true;
                errorMsg += "打印机报错";
            }
            if (faultCode != 0)//机器报错
            {
                mainCode = 99;
                errorToken = true;
                errorMsg += "设备故障";
            }
            switch (mainCode)
            {
                case 0:
                    break;
                case 1:
                    PrinterReceivePrintFace();//托盘弹出和回收，开始打印
                    break;
                case 2:
                    Printing();//开始打印
                    break;
                case 3:
                    PrintTakeBack();//托盘回收归零,开始组装
                    break;
                case 4:
                    break;
                case 5:
                    ShipmentAndWaitShip();//出货,等待取货
                    break;
                case 6:
                    break;
                case 70:
                    nowStep = 0x70;
                    break;
                case 99://故障
                    IniMachine();
                    break;

            }
            if (nowStep >= 0x05 && nowStep<0x10 && runCode == 0 && faultCode == 0)//复位
            {
                IniMachine();
            }
        }
        
        #endregion

        #region 上端手动控制
        
        /// <summary>
        /// 机器执行方案
        /// <para>01执行顺序:货道出料->装配印面->取盖子->打印机位置装配->装配盖子->装配位置出料->出成品和回零->自检</para>
        /// <para>02执行顺序:装配印面->货道出料->取盖子->打印机位置装配->装配盖子->装配位置出料->出成品和回零->自检->装配印面</para>
        /// </summary>
        public static string _MachineRunPlan;

        private const int _Start=1;//置位标志

        /// <summary>
        /// 机器当前执行任务
        /// <para>00:空闲 01:开始运行 02:复位 03:货道控制 04:装配印面 05:取盖子 06:打印机位置取料装配</para>
        /// <para>07:装配盖子 08:装配位置出料 09:出成品和回零 10:执行方案选择 90:机器检测 91:排故 99:报错</para>
        /// </summary>
        private volatile byte curState;

        /// <summary>
        /// 动作结束标志
        /// <para>false:未结束 true:结束</para>
        /// </summary>
        private bool OverToken=true;

        private int[] trubleNum = new int[]{ 0, 0, 0 };//排故序号 

        HashSet<string> cargoMsg = new HashSet<string>();//货道出货记录
        HashSet<string> printMsg = new HashSet<string>();//打印机状态记录
        HashSet<string> ligMsg = new HashSet<string>();//装取盖子记录
        HashSet<string> rigPrintMsg = new HashSet<string>();//装配印面记录
        HashSet<string> outSellerMsg = new HashSet<string>();//出货记录
        
        /// <summary>
        /// 机器操作和监控
        /// </summary>
        private void MachineControlAndMonitoring(int aisleSellNum)
        {
            switch (curState)
            {
                case 0x00:
                    break;
                case 0x01:
                    //安装印面程序
                    AisleSell(aisleSellNum);//货道出货     
                    TrayQueryAndLabor();
                    break;
                case 0x02:
                    ReSetMachine();//机器复位
                    break;
                case 0x05:
                    Printing();
                    TakeLid();//取盖子
                    break;
                case 0x06:
                    PrinterTakeAndRig();//打印机位置出料
                    PrintTakeBack();//回收打印机托盘
                    break;
                case 0x07:  
                    RigLid();//装配盖子
                    break;
                case 0x08:
                    RigLocaSell();//装配位置出料
                    break;
                case 0x09:
                    OutSellerAndZeroing();//出成品和回零
                    break;
                case 0x10:
                    Execution();//结束运行
                    break;
                case 0x11://结束后自检
                    string res = Post();
                    if (res == "None")
                        curState = 0x10;
                    else
                        ResetProgram();
                    break;
                case 0x90:
                    string result = Post();
                    if (result == "None")
                        curState = 0x10;
                    else if (result == "Error")
                        curState = 0x91;
                    else
                    {
                        errorMsg = result;
                        errorToken = true;
                        ResetProgram();
                    }
                    break;
                case 0x91:
                    Troubleshooting();//排故程序
                    break;
                case 0x99:
                    break;
            }
        }

        #endregion

        #region 打印机操作

        private bool trayOut = false;//托盘是否弹出
        private bool trayBack = false;//托盘是否回收打印
        private bool trayReZero = false;//托盘是否归零
        private bool printing = false;//开始打印
        private bool InPrintPos = false;//托盘是否在打印位置

        /// <summary>
        /// 托盘状态查询和操作
        /// </summary>
        private void TrayQueryAndLabor()
        {
            if (isRigPrint)//托盘已有印面
            {
                if (!InPrintPos)//未在打印位置
                {
                    if (!trayOut)
                    {
                        printMsg.Add("印面已安装");
                        printMsg.Add("正在调整打印机托盘位置");
                        PEPrinter.needMoveTray = 2;//弹出托盘
                        trayOut = true;
                        printMsg.Add("托盘弹出");
                    }
                    else if ((PEPrinter.TrayCondition==0x02 || PEPrinter.TrayCondition==0x06) && !trayBack)
                    {
                        PEPrinter.needMoveTray = 3;//托盘回收(打印位置)
                        trayBack = true;
                        printMsg.Add("托盘回收到打印位置");
                    } 
                    else if ((PEPrinter.TrayCondition == 0x05 || PEPrinter.TrayCondition == 0x01) && trayBack && !printing) 
                    {
                        Printing();
                        printing = true;
                    }
                }
                else//已经在打印位置
                {
                    if (!printing)
                    {
                        printMsg.Add("印面已安装");
                        printMsg.Add("托盘位置正确");
                        trayBack = true;
                        printing = true;
                        Printing();
                    }
                }
                if (D0 == 5 && printing)//取盖子
                {
                    curState = 0x05;
                    printing = false;
                    cargoMsg.Add("货道出货完成");
                }
            }
            else if (!isRigPrint && OverToken)//托盘无印面
            {
                RigPrintFace();
                OverToken = false;
            }
            if (!OverToken)//托盘接收印面
            {
                PrinterReceivePrintFace();
            }
        }

        /// <summary>
        /// 打印接收印面
        /// </summary>
        private void PrinterReceivePrintFace()
        {
            D11 = new PCHMI.VAR().GET_INT16(0, "D11");
            if (D11 > 0 && D11 < 3)
            {
                nowStep = 0x01;
            }
            else if (D11 >= 3 && D11 < 6)
            {
                nowStep = 0x02;
            }
            else if (D11 >= 6 && D11 < 8 && !trayOut)
            {
                nowStep = 0x03;
                trayOut = true;
                PEPrinter.needMoveTray = 4;//弹出托盘
                printMsg.Add("托盘弹出");
            }
            if (D0 == 5 && D11 == 10)//印面接收完成，货道出货完成
            {
                PEPrinter.needMoveTray = 3;//回收托盘进行打印
                trayBack = true;
                PrintFaceRecord(true);
                OverToken = true;
                curState = 0x05;
                printMsg.Add("托盘收回到打印位置");
                cargoMsg.Add("货道出货完成");

                log.WriteStepLog(StepType.货道出货, ref cargoMsg);//写入日志
            }
        }

        /// <summary>
        /// 打印程序
        /// </summary>
        private void Printing()
        {
            if (((PEPrinter.TrayCondition & 0x01) == 0x01) && trayBack && !String.IsNullOrEmpty(PEPrinter.PicPath))//开始打印
            {
                trayOut = false;
                trayBack = false;
                nowStep = 0x04;
                print.PrintAction();
                printMsg.Add("印章开始打印");
            }
        }

        /// <summary>
        /// 收回托盘程序
        /// </summary>
        private void PrintTakeBack()
        {
            if (D7 > 5 && !trayReZero)
            {
                trayReZero = true;
                nowStep = 0x05;
                PEPrinter.needMoveTray = 1;//回收托盘
                PrintFaceRecord(false);

                printMsg.Add("取货成功,托盘回收");
                log.WriteStepLog(StepType.印面打印, ref printMsg);
            }
        }

        /// <summary>
        /// 印面装配记录
        /// </summary>
        /// <param name="Is">已装配:true 未装配:false</param>
        public static void PrintFaceRecord(bool Is)
        {
            try
            {
                isRigPrint = Is;
                Form1.machineNode.WriteNameItemVAlue("isRigPrint", Is.ToString());
                Form1.myxmldoc.Save(Form1.configxmlfile);
                Form1.myxmldoc.Save(Form1.configxmlfilecopy);
            }
            catch
            {
                isRigPrint = Is;
            }
        }

        #endregion

        #region 机器控制

        /// <summary>
        /// 机器自检
        /// </summary>
        /// <returns></returns>
        private string Post()
        {
            string result = null;//是否有故障
            _RunEnd = false;//机器开始运行
            X12 = new PCHMI.VAR().GET_BIT(0, "X12");
            X10 = new PCHMI.VAR().GET_BIT(0, "X10");
            X7 = new PCHMI.VAR().GET_BIT(0, "X7");
            X13 = new PCHMI.VAR().GET_BIT(0, "X13");
            if (X12 == 1)
            {
                trubleNum[0] = 1;
                result = "Error";
            }
            if (X10 == 1)
            {
                trubleNum[1] = 1;
                result = "Error";
            }
            if (X7 == 1)
            {
                trubleNum[2] = 1;
                result = "Error";
            }
            if (X13 == 1)
                result = "NoPrinter";
            else if (X12 == 0 && X10 == 0 && X7 == 0)
            {
                result = "None";
            }
            return result;
        }
        
        bool checkD11 = false;
        /// <summary>
        /// 结束时方案选择
        /// </summary>
        private void Execution()
        {
            if (_MachineRunPlan == "02")
            {
                rigPrinting = true;
                Form1.HMIstep = 1;//退出页面
                X6 = new PCHMI.VAR().GET_BIT(0, "X6");
                X13 = new PCHMI.VAR().GET_BIT(0, "X13");
                M34 = new PCHMI.VAR().GET_INT16(0, "M34");
                M35 = new PCHMI.VAR().GET_INT16(0, "M35");
                if (X13 == 0 && X6 == 1 && M34==0 && M35==0 && checkD11==false)//如果有印面
                {
                    IniData();
                    new PCHMI.VAR().SEND_INT16(0, "D5", 0);
                    RigPrintFace();
                    checkD11 = true;
                }
                if (checkD11)
                {
                    PrinterReceivePrintFace();
                    if (D11 == 10)
                    {
                        PEPrinter.needMoveTray = 3;//托盘回收(打印位置)
                        InPrintPos = true;//托盘在打印位置
                        new PCHMI.VAR().SEND_CTRL(0, "M36", "反转","");//机器复位
                        PrintFaceRecord(true);//印面已经装配完成，不用再装
                        rigPrinting = false;
                        checkD11 = false;
                        ResetProgram();
                    }
                }
            }
            else
            {
                ResetProgram();
            }
        }

        /// <summary>
        /// 复位系统
        /// </summary>
        private void ResetProgram()
        {
            OverToken = true;
            isAisleSell = true;
            curState = 0x90;
            IniMachine();
            IniData();
        }

        /// <summary>
        /// PLC复位控制
        /// </summary>
        private void ReSetMachine()
        {
            if (OverToken)
            {
                new PCHMI.VAR().SEND_INT16(0, "D5", _Start);
                curState = 0x02;
                OverToken = false;
                nowStep = 0x80;
            }
            D5 = new PCHMI.VAR().GET_INT16(0, "D5");
            if (D5 == 4)
            {
                curState = 0x01;
                OverToken = true;
            } 
        }

        private bool isAisleSell = true;//货道是否出货
        /// <summary>
        /// 货道出货控制
        /// </summary>
        /// <param name="aisleNum">货道号 1 or 2 or 3</param>
        private void AisleSell(int aisleNum)
        {
            if (isAisleSell)
            {
                new PCHMI.VAR().SEND_INT16(0, "D0", (short)aisleNum);
                switch (aisleNum)
                {
                    case 1:
                        new PCHMI.VAR().SEND_INT16(0, "D1", _Start);
                        break;
                    case 2:
                        new PCHMI.VAR().SEND_INT16(0, "D2", _Start);
                        break;
                    case 3:
                        new PCHMI.VAR().SEND_INT16(0, "D3", _Start);
                        break;
                    default:
                        throw new Exception("货道错误");
                }

                cargoMsg.Add(aisleNum.ToString() + "号货道开始出货");//日志记录

                new PCHMI.VAR().SEND_INT16(0, "D4", _Start);
                isAisleSell = false;

                Form1.ReduceStock(aisleNum.ToString());//印章库存减一
            }
            D0 = new PCHMI.VAR().GET_INT16(0, "D0");
        }

        /// <summary>
        /// 装配印面
        /// </summary>
        private void RigPrintFace()
        {
            new PCHMI.VAR().SEND_INT16(0, "D11", _Start);
            printMsg.Add("开始取印面");
        }

        /// <summary>
        /// 取盖子
        /// </summary>
        private void TakeLid()
        {
            if (OverToken)
            {
                ligMsg.Add("开始取盖子");
                new PCHMI.VAR().SEND_INT16(0, "D6", _Start);
                OverToken = false;
            }
            D6 = new PCHMI.VAR().GET_INT16(0, "D6");
            if (D6 == 10)
            {
                curState = 0x06;
                OverToken = true;
                ligMsg.Add("取盖子完成");

                log.WriteStepLog(StepType.取盖子, ref ligMsg);//写入日志
            }
        }
        
        /// <summary>
        /// 打印机位置出料和装配
        /// </summary>
        private void PrinterTakeAndRig()
        {
            if (OverToken)
            {
                new PCHMI.VAR().SEND_INT16(0, "D7", _Start);
                OverToken = false;
                rigPrintMsg.Add("等待打印完成");
            }
            D7 = new PCHMI.VAR().GET_INT16(0, "D7");
            if (D7 ==2)
            {
                printMsg.Add("打印完成,托盘出货成功");
                rigPrintMsg.Add("开始装配印面");
            }
            else if (D7 == 10)
            {
                curState = 0x07;
                OverToken = true;

                rigPrintMsg.Add("印面装配完成");
                log.WriteStepLog(StepType.安装印面, ref rigPrintMsg);//写入日志
            }
        }

        /// <summary>
        /// 装配盖子
        /// </summary>
        private void RigLid()
        {
            if (OverToken)
            {
                new PCHMI.VAR().SEND_INT16(0, "D8", _Start);
                OverToken = false;
                ligMsg.Add("开始装配盖子");
            }
            D8 = new PCHMI.VAR().GET_INT16(0, "D8");
            if (D8 == 11)
            {
                OverToken = true;
                curState = 0x08;

                ligMsg.Add("装配盖子完成");
                log.WriteStepLog(StepType.装配盒子, ref ligMsg);
            }
        }

        /// <summary>
        /// 装配位置出料
        /// </summary>
        private void RigLocaSell()
        {
            if (OverToken)
            {
                new PCHMI.VAR().SEND_INT16(0, "D9", _Start);
                OverToken = false;
                nowStep = 0x06;

                outSellerMsg.Add("等待出货");
            }
            D9 = new PCHMI.VAR().GET_INT16(0, "D9");
            if (D9 == 13)
            {
                curState = 0x09;
                OverToken = true;
                outSellerMsg.Add("开始出货");
            }
        }

        /// <summary>
        /// 出成品和回零
        /// </summary>
        private void OutSellerAndZeroing()
        {
            if (OverToken)
            {
                new PCHMI.VAR().SEND_INT16(0, "D10", _Start);
                OverToken = false;
                nowStep = 0x07;
            }
            D10 = new PCHMI.VAR().GET_INT16(0, "D10");
            if (D10 == 2)
            {
                curState = 0x11;
                OverToken = true;

                outSellerMsg.Add("出货完成,机器复位");
                log.WriteStepLog(StepType.货道出货, ref outSellerMsg);
                log.SaveRunningLog();
            }
        }

        /// <summary>
        /// 排故程序
        /// </summary>
        /// <param name="index">排故序号 1 or 2 or 3</param>
        private void Troubleshooting()
        {
            if (OverToken)
            {
                if (trubleNum[0] == 1)//故障1
                {
                    if (OverToken)
                    {
                        new PCHMI.VAR().SEND_INT16(0, "D12", _Start);
                        OverToken = false;
                        nowStep = 0x81;
                    }
                    D12 = new PCHMI.VAR().GET_INT16(0, "D12");
                    OverToken = D12 == 10 ? true : false;
                    trubleNum[0] = D12 == 10 ? 0 : 1;
                }
                else if (trubleNum[1] == 1)//故障2
                {
                    if (OverToken)
                    {
                        new PCHMI.VAR().SEND_INT16(0, "D13", _Start);
                        OverToken = false;
                        nowStep = 0x81;
                    }
                    D13 = new PCHMI.VAR().GET_INT16(0, "D13");
                    OverToken = D13 == 12 ? true : false;
                    trubleNum[1] = D13 == 12 ? 0 : 1;
                }
                else if (trubleNum[2] == 1)//故障3
                {
                    if (OverToken)
                    {
                        new PCHMI.VAR().SEND_INT16(0, "D14", _Start);
                        OverToken = false;
                        nowStep = 0x81;
                    }
                    D14 = new PCHMI.VAR().GET_INT16(0, "D14");
                    OverToken = D14 == 10 ? true : false;
                    trubleNum[2] = D14 == 10 ? 0 : 1;
                }
                else
                {
                    curState = 0x90;//排故完成后再次检查
                }
            }
        }

        /// <summary>
        /// 监控机器错误代码和运行代码
        /// <para>获取当前步骤</para>
        /// </summary>
        private void GetCode()
        {
            faultCode = new PCHMI.VAR().GET_INT16(0, "D209");
            runCode = new PCHMI.VAR().GET_INT16(0, "D208");
            mainCode = new PCHMI.VAR().GET_INT16(0, "D15");

        }
        
        /// <summary>
        /// 出货和等待取货
        /// </summary>
        private void ShipmentAndWaitShip()
        {
            D9 = new PCHMI.VAR().GET_INT16(0, "D9");
            if (D9 >= 4)
            {
                nowStep = 0x06;
            }
            else if (D9 >= 7)
            {
                nowStep = 0x07;
            }
        }

        /// <summary>
        /// 复位数据
        /// </summary>
        private void IniMachine()
        {
            sendOver = false;
            trayReZero = false;
            trayBack = false;
            trayOut = false;
            nowStep = 0x00;
            runTiming = 150;
            Form1.HMIstep = 1;
            _RunEnd = true;
        }

        private bool sendOver = false;
        /// <summary>
        /// 向设备发送货道号和开始运行指令
        /// </summary>
        /// <param name="huodaorecv">货道号</param>
        private void StartRunning(int huodaorecv)
        {
            PCHMI.CONFIG.PLC_OFF[0] = false;
            switch (huodaorecv)
            {
                case 1:
                    new PCHMI.VAR().SEND_CTRL(0, "D208", "字写入", "257");
                    break;
                case 2:
                    new PCHMI.VAR().SEND_CTRL(0, "D208", "字写入", "513");
                    break;
                case 3:
                    new PCHMI.VAR().SEND_CTRL(0, "D208", "字写入", "769");
                    break;
            }
        }

        #endregion

        #region 机器检测

        /// <summary>
        /// 检测是否还有印面
        /// <para>True:有 False:没有</para>
        /// </summary>
        /// <returns></returns>
        public static bool GoodsInspect()
        {
            ushort x13 = new PCHMI.VAR().GET_BIT(0, "X13");
            ushort x6 = new PCHMI.VAR().GET_BIT(0, "X6");
            if (x13 == 1 || x6==0)
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

        #endregion

    }
}
