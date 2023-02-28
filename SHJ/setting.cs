﻿using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Runtime.InteropServices;
using AForge.Video.DirectShow;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;

namespace SHJ
{
    public partial class setting : Form
    {
        private const int SW_RESTORE = 9;//显示任务栏
        private VideoCaptureDevice tempDevice;//临时摄像
        private string photoTestPath=AppDomain.CurrentDomain.BaseDirectory+"//TestImages//Photos";//拍照测试路径

        [DllImport("user32.dll")]
        public static extern int ShowWindow(int hwnd, int nCmdShow);
        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        public setting()
        {
            InitializeComponent();
        }
         
        #region Feild
        
        public static string debugPass = null;//机器调试密码
        public static string setupPass = null;//系统设置密码
        public static string CPFRPass = null;//补货密码
        
        private bool needsave = false;//是否需要保存配置
        private int stateOK;//货道状态是否正常

        public static string helpimgaddress;

        private Keyboard keyboard = null;
        private CameraHelper camera;
        private CameraPara nowCamera;

        private Point defualtPoint = new Point(508, 78);

        #endregion

        #region Load

        private void setting_Load(object sender, EventArgs e)
        {
            updatecaidan();
            if (Form1.functionnode.Attributes.GetNamedItem("fenbianlv").Value == "0")
            {
                this.Width = 1920;
                this.Height = 1080;
                this.Location = new Point(0, 0);
            }
            txt_Pass.Text = "";
            keyboard = Keyboard.GetKeyboard();//获取实例

            label31.Text = "设定输出灰度值:" + hScrollBar2.Value.ToString();
            label30.Text = "设定橡胶打印行速:" + hScrollBar1.Value.ToString();
            
            if (!Directory.Exists(photoTestPath))
            {
                Directory.CreateDirectory(photoTestPath);
            }
            else//删除临时拍照图片
            {
                DirectoryInfo dir = new DirectoryInfo(photoTestPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
                foreach (var item in fileinfo)
                {
                    File.Delete(item.FullName);
                }
            }
        }

        #endregion

        #region Method
        
        /// <summary>
        /// 键盘
        /// </summary>
        /// <param name="maxNum">最大值</param>
        /// <param name="inputNum">初始值</param>
        /// <param name="point">位置</param>
        /// <returns></returns>
        private string ShowKeyboard(int maxNum, string inputNum, Point point, string valueType = "Int")
        {
            keyboard.maxNum = maxNum;
            keyboard.inputValue = inputNum;
            keyboard.Location = point;
            keyboard.valueType = valueType;
            if (keyboard.ShowDialog() == DialogResult.OK)
            {
                if (inputNum != keyboard.inputValue)
                    needsave = true;
                return keyboard.inputValue;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 程序退出
        /// </summary>
        private void CloseProgram()
        {
            PEPrinter.PE_Close(PEPrinter.PEhandle);
            try
            {
                System.Diagnostics.Process[] MyProcesses = System.Diagnostics.Process.GetProcesses();
                foreach (System.Diagnostics.Process MyProcess in MyProcesses)
                {
                    if (MyProcess.ProcessName.CompareTo("ADHStart") == 0)
                    {
                        MyProcess.Kill();
                    }
                }
            }
            catch { }
            Form1.needcloseform = true;
            ShowWindow(FindWindow("Shell_TrayWnd", null), SW_RESTORE);
            ShowWindow(FindWindow("Button", null), SW_RESTORE);
        }

        /// <summary>
        /// 显示出货记录
        /// </summary>
        private void ShowShipRecord()
        {
            listBox1.Items.Clear();
            XmlNodeList list = Form1.shipmentDoc.SelectSingleNode("Sale").SelectSingleNode("ShipRecord").ChildNodes;
            listBox1.Items.Add("  货道号      价格            出货时间");
            for (int i = 0; i < list.Count; i++)
            {
                string str;
                str = "    "+list[i].GetNameItemValue("itemNo") + "       " + list[i].GetNameItemValue("price")+"\t"+list[i].GetNameItemValue("time");
                listBox1.Items.Add(str);
            }
        }
        /// <summary>
        /// 切换设置页面
        /// </summary>
        /// <param name="panel"></param>
        private void SwitchPage(Panel panel)
        {
            txt_Pass.Text = "";
            panel_Back.Visible = true;
            panel.Visible = false;
        }

        /// <summary>
        /// 将修改保存到XML
        /// </summary>
        private void updatexml()
        {
            Form1.netcofignode.Attributes.GetNamedItem("ipconfig").Value = 
                textBox6.Text + "." + textBox7.Text + "." + textBox8.Text + "." + textBox9.Text;

            Form1.netcofignode.Attributes.GetNamedItem("port").Value = textBox3.Text;
            Form1.netcofignode.Attributes.GetNamedItem("netdelay").Value = textBox4.Text;
            if (rb_PC.Checked)
                Form1.machineNode.Attributes.GetNamedItem("isAutoRun").Value = "False";
            else
                Form1.machineNode.Attributes.GetNamedItem("isAutoRun").Value = "True";
            if (rb_RunType1.Checked)
                Form1.machineNode.Attributes.GetNamedItem("runType").Value = "01";
            else
                Form1.machineNode.Attributes.GetNamedItem("runType").Value = "02";
            if (checkBox8.Checked)
                Form1.machineNode.Attributes.GetNamedItem("photoTest").Value = "True";
            else
                Form1.machineNode.Attributes.GetNamedItem("photoTest").Value = "False";
            if (checkBox5.Checked)
                Form1.functionnode.Attributes.GetNamedItem("adupdate").Value = "0";
            else
                Form1.functionnode.Attributes.GetNamedItem("adupdate").Value = "1";
            if (checkBox1.Checked)
                Form1.appconfig.WriteNameItemVAlue("fualtCheck", "True");
            else
                Form1.appconfig.WriteNameItemVAlue("fualtCheck", "False");

            int i;
            for (i = 0; i < dataGridView1.Rows.Count; i++)
            {
                Form1.nodelistshangpin[i].Attributes.GetNamedItem("jiage").Value = dataGridView1.Rows[i].Cells[1].Value.ToString();
                Form1.nodelistshangpin[i].Attributes.GetNamedItem("huodao").Value = dataGridView1.Rows[i].Cells[2].Value.ToString();
            }
            for (i = 0; i < dataGridView2.Rows.Count; i++)
            {
                Form1.nodelisthuodao[i].Attributes.GetNamedItem("kucun").Value = dataGridView2.Rows[i].Cells[1].Value.ToString();
                Form1.nodelisthuodao[i].Attributes.GetNamedItem("volume").Value = dataGridView2.Rows[i].Cells[3].Value.ToString();
                Form1.nodelisthuodao[i].Attributes.GetNamedItem("position").Value = dataGridView2.Rows[i].Cells[4].Value.ToString();
            }
            Form1.functionnode.Attributes.GetNamedItem("temperature1").Value = hScrollBar1.Value.ToString();
            Form1.functionnode.Attributes.GetNamedItem("temperature2").Value = hScrollBar2.Value.ToString();
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        private void updatecaidan()
        {
            label28.Text = "软件版本:"+ Form1.systemNode.GetNameItemValue("Version");
            label15.Text = "设备编号:" + Encoding.ASCII.GetString(Form1.IMEI);

            ShowShipRecord();

            comboBox9.SelectedIndex = 1;
            comboBox1.SelectedIndex = 2;
            if (Form1.appconfig.GetNameItemValue("fualtCheck") == "True")
                checkBox1.Checked = true;
            else
                checkBox1.Checked = false;
            
            if(Form1.machineNode.GetNameItemValue("isAutoRun")=="True")
                rb_PLC.Checked = true;
            else
                rb_PC.Checked = true;
            if(Form1.machineNode.GetNameItemValue("runType")=="01")
            {
                rb_RunType1.Checked = true;
                button1.Visible = true;
            }
            else if(Form1.machineNode.GetNameItemValue("runType")=="02")
            {
                rb_RunType2.Checked = true;
                button1.Visible = false;
            }
            
            string[] ipstring = Form1.netcofignode.Attributes.GetNamedItem("ipconfig").Value.Split('.');
            if (ipstring.Length == 4)
            {
                textBox6.Text = ipstring[0];
                textBox7.Text = ipstring[1];
                textBox8.Text = ipstring[2];
                textBox9.Text = ipstring[3];
            }
            textBox3.Text = Form1.netcofignode.Attributes.GetNamedItem("port").Value;
            textBox4.Text = Form1.netcofignode.Attributes.GetNamedItem("netdelay").Value;
            if (Form1.machineNode.Attributes.GetNamedItem("photoTest").Value == "True")
                checkBox8.Checked = true;
            else
                checkBox8.Checked = false;
           
            if (Form1.functionnode.Attributes.GetNamedItem("adupdate").Value == "1")
                checkBox5.Checked = false;
            else
                checkBox5.Checked = true;

            if (Form1.appconfig.GetNameItemValue("fualtCheck") == "True")
                checkBox1.Checked = true;
            else
                checkBox1.Checked = false;

            textBox11.Text = Form1.nodelistshangpin.Count.ToString();//商品数量

            hScrollBar1.Value = int.Parse(Form1.functionnode.Attributes.GetNamedItem("temperature1").Value);
            hScrollBar2.Value = int.Parse(Form1.functionnode.Attributes.GetNamedItem("temperature2").Value);
            string tempvalue = Form1.functionnode.Attributes.GetNamedItem("touch").Value;

            dataGridView1.Columns.Add("c0", "商品");
            dataGridView1.Columns.Add("c1", "价格");
            dataGridView1.Columns.Add("c2", "货道");
            dataGridView1.Columns.Add("c3", "状态");
            dataGridView1.Columns.Add("c4", "销售");
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[3].ReadOnly = true;
            dataGridView1.Columns[4].ReadOnly = true;
            dataGridView1.Columns[2].ReadOnly = true;

            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            foreach (XmlNode _node in Form1.nodelistshangpin)
            {               
                dataGridView1.Rows.Add();
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[0].Value = _node.Attributes.GetNamedItem("shangpinnum").Value;
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[1].Value = _node.Attributes.GetNamedItem("jiage").Value;
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[2].Value = _node.Attributes.GetNamedItem("huodao").Value;
                if (_node.Attributes.GetNamedItem("state").Value == "0")
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[3].Value = "正常";
                else
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[3].Value = "暂停";
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[4].Value = _node.Attributes.GetNamedItem("salesum").Value;
            }
            dataGridView1.ClearSelection();

            textBox12.Text = Form1.nodelisthuodao.Count.ToString();//货道数量
            dataGridView2.Columns.Add("c0", "货道");
            dataGridView2.Columns.Add("c1", "库存");
            dataGridView2.Columns.Add("c2", "状态");
            dataGridView2.Columns.Add("c3", "容量");
            dataGridView2.Columns.Add("c4", "类型");
            dataGridView2.Columns[0].ReadOnly = true;
            dataGridView2.Columns[1].ReadOnly = false;
            dataGridView2.Columns[2].ReadOnly = true;
            dataGridView2.Columns[3].ReadOnly = true;
            dataGridView2.Columns[4].ReadOnly = true;

            dataGridView2.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView2.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView2.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView2.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView2.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            stateOK = 0;
            foreach (XmlNode _node in Form1.nodelisthuodao)
            {
                dataGridView2.Rows.Add();
                dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[0].Value = _node.Attributes.GetNamedItem("huodaonum").Value;
                dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[1].Value = _node.Attributes.GetNamedItem("kucun").Value;
                if (_node.Attributes.GetNamedItem("state").Value == "0")
                {
                    dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[2].Value = "正常";
                    stateOK++;
                }
                else if (_node.Attributes.GetNamedItem("state").Value == "1")
                    dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[2].Value = "过流";
				else if (_node.Attributes.GetNamedItem("state").Value == "2")
                    dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[2].Value = "断线";
				else
                    dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[2].Value = "故障";
                dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[3].Value = _node.Attributes.GetNamedItem("volume").Value;
                dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[4].Value = _node.Attributes.GetNamedItem("position").Value;
            }
            dataGridView2.ClearSelection();
            
        }
        
        private int count1000;//1000ms计数
        /// <summary>
        /// 更新菜单状态
        /// </summary>
        private void updatemenu()
        {
            
            if (count1000 < 9)
            {
                count1000++;
            }
            else
            {
                count1000 = 0;
                Form1.guanggaoreturntime = 0;//返回广告页面计时清零
            }
            if(PEPrinterupdatedelay>0)//需要更新打印机橡胶打印线速度
            {
                if(PEPrinterupdatedelay<30)//3秒内不重复更新
                {
                    PEPrinterupdatedelay++;
                }
                else
                {
                    PEPrinterupdatedelay = 0;
                    if (hScrollBar1.Value != PEPrinter.LineSpeedValue1)
                    {
                        PEPrinter.LineSpeedValue1= (UInt16)hScrollBar1.Value;
                        PEPrinter.needPutPrintCondition = true;
                    }
                    if (hScrollBar2.Value != PEPrinter.OutputNumber)
                    {
                        PEPrinter.OutputNumber = (UInt16)hScrollBar2.Value;
                        PEPrinter.needPutPrintCondition = true;
                    }
                }
            }

            if (Form1.istestmode)
            {
                stateOK = 0;
                for (int i = 0; i < Form1.nodelisthuodao.Count; i++)
                {
                    if (Form1.nodelisthuodao[i].Attributes.GetNamedItem("state").Value == "0")
                    {
                        dataGridView2.Rows[i].Cells[2].Value = "正常";
                        stateOK++;
                    }
                    else if (Form1.nodelisthuodao[i].Attributes.GetNamedItem("state").Value == "1")
                    {
                        dataGridView2.Rows[i].Cells[2].Value = "过流";
                    }
                    else if (Form1.nodelisthuodao[i].Attributes.GetNamedItem("state").Value == "2")
                    {
                        dataGridView2.Rows[i].Cells[2].Value = "断线";
                    }
                    else
                    {
                        dataGridView2.Rows[i].Cells[2].Value = "故障";
                    }
                }
            }
            
            //打印机状态
            if (PEPrinter.isconnected)
            {
                label70.Text = "连接状态:已连接";
            }
            else
            {
                label70.Text = "连接状态:未连接";
            }

            if (PEPrinter.isPEPrinterReady)
            {
                label2.Text = "机器状态:准备就绪";
            }
            else
            {
                label2.Text = "机器状态:未准备好";
            }
            label152.Text = label70.Text;
            label153.Text = label2.Text;
            label16.Text = "任务步骤:" + PEPrinter.PEloopstate;
            label157.Text = label16.Text;
            label69.Text = "状态字:" + PEPrinter.PEPrinterState.ToString("X4");
            label160.Text = label69.Text;
            label61.Text = "状态描述:" + PEPrinter.PEPrinterStatedetail;
            label156.Text = label61.Text;
            label68.Text = "媒介类型:" + PEPrinter.MediaType.ToString();
            label66.Text = "打印头温度:" + (PEPrinter.HeadTemperature / 2.0).ToString("f1") + "℃";
            switch (PEPrinter.TrayCondition)
            {
                case 0x00:
                    label67.Text = "托盘状态:托盘运动中";
                    label65.Text = "媒介状态:媒介不存在";
                    break;
                case 0x01:
                    label67.Text = "托盘状态:托盘归位";
                    label65.Text = "媒介状态:媒介不存在";
                    break;
                case 0x02:
                    label67.Text = "托盘状态:托盘弹出";
                    label65.Text = "媒介状态:未知";
                    break;
                case 0x04:
                    label67.Text = "托盘状态:托盘运动中";
                    label65.Text = "媒介状态:媒介存在";
                    break;
                case 0x05:
                    label67.Text = "托盘状态:托盘归位";
                    label65.Text = "媒介状态:媒介存在";
                    break;
                default:
                    label67.Text = "托盘状态:未知" + PEPrinter.TrayCondition.ToString("X2");
                    label65.Text = "媒介状态:未知";
                    break;
            }
            label67.Text += PEPrinter.TrayCondition.ToString();

            label162.Text = label67.Text;
            label155.Text = label65.Text;
            label75.Text = "打印机编号:" + PEPrinter.ProductID.ToString("X4");
            label154.Text = label75.Text;
            label72.Text = "F/W版本号:V" + (PEPrinter.Version >> 8).ToString("X") + "." + (PEPrinter.Version & 0xff).ToString("X2");
            label74.Text = "可打印最宽像素:" + PEPrinter.PrintableWidth.ToString();
            label73.Text = "可打印最高像素:" + PEPrinter.PrintableHeight.ToString();
            label64.Text = "打印头分辨率:" + PEPrinter.HeadResolution.ToString("f3") + "dpi";
            label71.Text = "所有打印头点数:" + PEPrinter.HeadDots.ToString();
            label63.Text = "有效打印头点数:" + PEPrinter.ValidHeadDots.ToString();
            label62.Text = "每块打印头点数:" + PEPrinter.HeadBlockDots.ToString();
            label77.Text = "橡胶行速:" + PEPrinter.LineSpeed1.ToString("f1") + "ms/行";
            label170.Text = label77.Text;
            label76.Text = "标签行速:" + PEPrinter.LineSpeed2.ToString("f1") + "ms/行";
            label168.Text = label76.Text;
            label54.Text = "橡胶打印行速设定值:" + PEPrinter.LineSpeedValue1.ToString();
            label56.Text = "标签打印行速设定值:" + PEPrinter.LineSpeedValue2.ToString();
            label60.Text = "休眠模式进入时间:" + PEPrinter.SeepTransitionTime.ToString() + "分";
            label59.Text = "输出灰度值:" + PEPrinter.OutputNumber.ToString();
            label176.Text = label59.Text;
            label78.Text = "预热操作输出值:" + PEPrinter.PreheatData.ToString();
            label166.Text = label78.Text;
            label58.Text = "预热操作执行行数:" + PEPrinter.PreheatLines.ToString();
            label178.Text = label58.Text;
            label57.Text = "预热操作STROBE信号低电平:" + PEPrinter.PreheatLineStrobeLow.ToString();

            System.Windows.Forms.Application.DoEvents();
        }

        /// <summary>
        ///  更新广告图片和商品图片
        /// </summary>
        private void updatepic()
        {
            try
            {
                DriveInfo[] s = DriveInfo.GetDrives();
                foreach (DriveInfo drive in s)
                {
                    if (drive.DriveType == DriveType.Removable)
                    {
                        if (System.IO.Directory.Exists(drive.Name + "EPTON") == false)//更新文件夹不存在
                        {
                            System.IO.Directory.CreateDirectory(drive.Name + "EPTON");
                        }
                        string mymima = null;
                        string[] upanfiles;
                        switch (mymima)
                        {
                            case "":
                                //更新广告
                                if (System.IO.Directory.Exists(drive.Name + "EPTON\\adimages") == false)//更新文件夹不存在
                                {
                                    System.IO.Directory.CreateDirectory(drive.Name + "EPTON\\adimages");
                                }
                                upanfiles = System.IO.Directory.GetFiles(drive.Name + "EPTON\\adimages");
                                for (int i = 0; i < upanfiles.Length; i++)//复制文件到系统
                                {
                                    int mystartindex = upanfiles[i].LastIndexOf('\\');
                                    int myendindex = upanfiles[i].LastIndexOf('.');
                                    string tempupanfile = upanfiles[i].ToLower();
                                    bool mycontainpic = tempupanfile.EndsWith(".bmp") || tempupanfile.EndsWith(".jpg")
                                        || tempupanfile.EndsWith(".png") || tempupanfile.EndsWith(".gif")
                                        || tempupanfile.EndsWith(".tif") || tempupanfile.EndsWith(".jpeg");
                                    if ((mystartindex >= 0) && (myendindex >= 0) && (mycontainpic == true))//文件名正确
                                    {
                                        string mycmname = upanfiles[i].Substring(mystartindex + 1, myendindex - mystartindex - 1);
                                        try
                                        {
                                            int adnum = int.Parse(mycmname);
                                            if ((adnum > 0) && (adnum <= 5))//支持5个广告图片
                                            {
                                                File.Copy(upanfiles[i], Form1.adimagesaddress + "\\" + mycmname + ".jpg", true);
                                            }
                                        }
                                        catch
                                        { }
                                    }
                                    mycontainpic = tempupanfile.EndsWith(".wav") || tempupanfile.EndsWith(".mid")
                                        || tempupanfile.EndsWith(".mp4") || tempupanfile.EndsWith(".mp3")
                                        || tempupanfile.EndsWith(".mpg") || tempupanfile.EndsWith(".avi")
                                        || tempupanfile.EndsWith(".asf") || tempupanfile.EndsWith(".wmv")
                                        || tempupanfile.EndsWith(".rm") || tempupanfile.EndsWith(".rmvb");
                                    if ((mystartindex >= 0) && (myendindex >= 0) && (mycontainpic == true))//文件名正确
                                    {
                                        string mycmname = upanfiles[i].Substring(mystartindex + 1, myendindex - mystartindex - 1);
                                        try
                                        {
                                            int adnum = int.Parse(mycmname);
                                            if ((adnum > 0) && (adnum <= 5))//支持5个广告视频
                                            {
                                                File.Copy(upanfiles[i], Form1.adimagesaddress + "\\" + mycmname + ".mp4", true);
                                            }
                                        }
                                        catch
                                        { }
                                    }
                                }
                                Form1.adimagefiles = System.IO.Directory.GetFiles(Form1.adimagesaddress);

                                for (int i = 0; i < Form1.adimagefiles.Length; i++)//复制文件到U盘
                                {
                                    int mystartindex = Form1.adimagefiles[i].LastIndexOf('\\');
                                    string mycmname = Form1.adimagefiles[i].Substring(mystartindex);
                                    File.Copy(Form1.adimagefiles[i], drive.Name + "EPTON\\adimages" + mycmname, true);
                                }
                                //更新商品图片
                                if (System.IO.Directory.Exists(drive.Name + "EPTON\\cmimages") == false)//更新文件夹不存在
                                {
                                    System.IO.Directory.CreateDirectory(drive.Name + "EPTON\\cmimages");
                                }
                                upanfiles = System.IO.Directory.GetFiles(drive.Name + "EPTON\\cmimages");
                                for (int i = 0; i < upanfiles.Length; i++)//复制文件到系统
                                {
                                    int mystartindex = upanfiles[i].LastIndexOf('\\');
                                    int myendindex = upanfiles[i].LastIndexOf('.');
                                    string tempupanfile = upanfiles[i].ToLower();
                                    bool mycontainpic = tempupanfile.EndsWith(".bmp") || tempupanfile.EndsWith(".jpg")
                                        || tempupanfile.EndsWith(".png") || tempupanfile.EndsWith(".gif")
                                        || tempupanfile.EndsWith(".tif") || tempupanfile.EndsWith(".jpeg");
                                    if ((mystartindex >= 0) && (myendindex >= 0) && (mycontainpic == true))//文件名正确
                                    {
                                        string mycmname = upanfiles[i].Substring(mystartindex + 1, myendindex - mystartindex - 1);
                                        try
                                        {
                                            int adnum = int.Parse(mycmname);
                                            if ((adnum > 0) && (adnum < 1000) && (mycmname.Length == 3))//支持1-999个商品图片,编码从001-999
                                            {
                                                File.Copy(upanfiles[i], Form1.cmimagesaddress + "\\" + mycmname + ".jpg", true);
                                            }
                                        }
                                        catch
                                        { }
                                    }
                                }
                                Form1.cmimagefiles = System.IO.Directory.GetFiles(Form1.cmimagesaddress);
                                for (int i = 0; i < Form1.cmimagefiles.Length; i++)//复制文件到U盘
                                {
                                    int mystartindex = Form1.cmimagefiles[i].LastIndexOf('\\');
                                    string mycmname = Form1.cmimagefiles[i].Substring(mystartindex);
                                    File.Copy(Form1.cmimagefiles[i], drive.Name + "EPTON\\cmimages" + mycmname, true);
                                }

                                break;
                            case "999"://更新系统
                                if (System.IO.Directory.Exists(drive.Name + "EPTON\\bkimages") == false)//更新文件夹不存在
                                {
                                    System.IO.Directory.CreateDirectory(drive.Name + "EPTON\\bkimages");
                                }
                                string tempfilestr = drive.Name + "EPTON\\bkimages\\select.jpg";
                                try
                                {
                                    File.Copy(tempfilestr, Form1.bkimagesaddress + tempfilestr.Substring(tempfilestr.LastIndexOf('\\')), true);
                                }
                                catch { }
                                tempfilestr = drive.Name + "EPTON\\reg.dll";
                                try
                                {
                                    //判断注册是否正确
                                    XmlDocument mxmldoc = new XmlDocument();
                                    mxmldoc.Load(tempfilestr);
                                    UInt64 mregdata = UInt64.Parse(mxmldoc.SelectSingleNode("reg").Attributes.GetNamedItem("regid").Value);
                                    UInt64 mimeidata = 0;
                                    for (int i = 0; i < 15; i++)
                                    {
                                        mimeidata = (mimeidata << 8) + (byte)(Form1.IMEI[i] & 0x77);
                                    }
                                    if (mimeidata == mregdata)//正确复制文件
                                    {
                                        File.Copy(tempfilestr, Form1.bkimagesaddress.Remove(Form1.bkimagesaddress.LastIndexOf('\\')) + "\\reg.dll", true);
                                    }
                                }
                                catch { }
                                tempfilestr = drive.Name + "EPTON\\SHJ.exe";
                                try
                                {
                                    File.Copy(tempfilestr, Form1.bkimagesaddress.Remove(Form1.bkimagesaddress.LastIndexOf('\\')) + "\\new.exe", true);
                                }
                                catch { }
                                tempfilestr = drive.Name + "EPTON\\ADHStart.exe";
                                try
                                {
                                    File.Copy(tempfilestr, Form1.bkimagesaddress.Remove(Form1.bkimagesaddress.LastIndexOf('\\')) + "\\ADHStart.exe", true);
                                }
                                catch { }
                                break;
                        }
                    }
                }
            }
            catch (Exception myexp)
            {
                MessageBox.Show(myexp.Message);
            }
        }
        
        #endregion

        #region Timer
        
        private void timer1_Tick(object sender, EventArgs e)//500ms
        {
            updatemenu();
            if (PLCHelper.errorToken)
            {
                lbl_ErrorMsg.Text = PLCHelper.errorMsg;
                label171.Visible = true;
                lbl_ErrorMsg.Visible = true;
            }
            else
            {
                label171.Visible = false;
                lbl_ErrorMsg.Visible = false;
            }
        }

        #endregion

        #region FormClosing

        private void setting_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult myresult;
            if (needsave)
            {
                myresult = MessageBox.Show("配置已经修改，是否需要保存并退出后台管理？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                if (myresult == DialogResult.Yes)
                {
                    needsave = false;
                    updatexml();
                    Form1.myxmldoc.Save(Form1.configxmlfile);
                    Form1.myxmldoc.Save(Form1.configxmlfilecopy);
                }
                else if (myresult == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                myresult = MessageBox.Show("是否退出后台管理？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                if (myresult == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        #endregion

        #region TextBox

        private void txt_Pass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txt_Pass.Text == CPFRPass)
                {
                    panel_Back.Visible = false;
                    panel_CPFR.Visible = true;
                }
                else if (txt_Pass.Text == setupPass)
                {
                    panel_Back.Visible = false;
                    panel_setup.Visible = true;
                }
                else if (txt_Pass.Text == debugPass)
                {
                    panel_Back.Visible = false;
                    panel_debug.Visible = true;
                }
                else
                {
                    MessageBox.Show("密码错误！！");
                }
            }
        }

        private void textBox5_Click(object sender, EventArgs e)
        {
            textBox5.Text=ShowKeyboard(99, textBox5.Text, defualtPoint);
        }

        private void textBox6_Click(object sender, EventArgs e)
        {
            textBox6.Text = ShowKeyboard(255, textBox6.Text, defualtPoint);
        }

        private void textBox7_Click(object sender, EventArgs e)
        {
            textBox7.Text = ShowKeyboard(255, textBox7.Text, defualtPoint);
        }

        private void textBox8_Click(object sender, EventArgs e)
        {
            textBox8.Text = ShowKeyboard(255, textBox8.Text, defualtPoint);
        }

        private void textBox9_Click(object sender, EventArgs e)
        {
            textBox9.Text = ShowKeyboard(255, textBox9.Text, defualtPoint);
        }

        private void textBox3_Click(object sender, EventArgs e)
        {
            textBox3.Text = ShowKeyboard(65535, textBox3.Text, defualtPoint);
        }

        private void textBox4_Click(object sender, EventArgs e)
        {
            textBox4.Text = ShowKeyboard(180, textBox4.Text, defualtPoint);
        }

        private void textBox11_Click(object sender, EventArgs e)
        {
            textBox11.Text = ShowKeyboard(1000, textBox11.Text, defualtPoint);
        }

        private void textBox12_Click(object sender, EventArgs e)
        {
            textBox12.Text = ShowKeyboard(1000, textBox12.Text, defualtPoint);
        }

        private void textBox13_Click(object sender, EventArgs e)
        {
            textBox13.Text = ShowKeyboard(150, textBox13.Text, defualtPoint);
        }

        private void textBox14_Click(object sender, EventArgs e)
        {
            textBox14.Text = ShowKeyboard(10000, textBox14.Text, defualtPoint);
        }

        private void textBox16_Click(object sender, EventArgs e)
        {
            textBox16.Text = ShowKeyboard(150, textBox16.Text, defualtPoint);
        }

        #endregion

        #region CheckBox

        private void checkBox5_Click(object sender, EventArgs e)
        {
            needsave = true;
        }

        private void checkBox8_Click(object sender, EventArgs e)
        {
            needsave = true;
        }
        private void checkBox1_Click(object sender, EventArgs e)
        {
            needsave = true;
        }
        #endregion

        #region DataGridView


        private void dataGridView2_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > 0)
            {
                if (dataGridView2.Rows[e.RowIndex - 1].Cells[e.ColumnIndex].Value.ToString() == "")
                {
                    dataGridView2.Rows[e.RowIndex - 1].Cells[e.ColumnIndex].Value = "0";
                }
            }
        }
        
        private void dataGridView2_Click(object sender, EventArgs e)
        {
            if (this.dataGridView2.CurrentCell.ColumnIndex==1)
            {
                this.dataGridView2.CurrentCell.Value = ShowKeyboard(255, this.dataGridView2.CurrentCell.Value.ToString(), defualtPoint);
            }
            else if (this.dataGridView2.CurrentCell.ColumnIndex == 3)
            {
                this.dataGridView2.CurrentCell.Value = ShowKeyboard(255, this.dataGridView2.CurrentCell.Value.ToString(), defualtPoint);
            }
            else if (this.dataGridView2.CurrentCell.ColumnIndex == 4)
            {
                this.dataGridView2.CurrentCell.Value = ShowKeyboard(3, this.dataGridView2.CurrentCell.Value.ToString(), defualtPoint);
            }
        }
        private void dataGridView1_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.CurrentCell.ColumnIndex == 1)//价格
            {
                this.dataGridView1.CurrentCell.Value = ShowKeyboard(1000, this.dataGridView1.CurrentCell.Value.ToString(), defualtPoint,"Double");
            }
        }

        #endregion

        #region Button

        private void button30_Click(object sender, EventArgs e)
        {
            SwitchPage(this.panel_setup);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            SwitchPage(this.panel_CPFR);
        }

        private void button32_Click(object sender, EventArgs e)
        {
            SwitchPage(this.panel_debug);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            PLCHelper.errorToken = false;//清除故障提示
            PLCHelper.errorMsg = null;
            MessageBox.Show("清零成功");
        }

        private void button19_Click(object sender, EventArgs e)
        {
            InitCamera();
        }

        CancellationTokenSource token;
        private void button21_Click(object sender, EventArgs e)//排故测试
        {
            button23.Enabled = true;
            token = new CancellationTokenSource();
            int[] trubleNum = new int[3];
            bool truble = false;
            bool OverToken = true;
            ushort X12 = new PCHMI.VAR().GET_BIT(0, "X12");
            ushort X10 = new PCHMI.VAR().GET_BIT(0, "X10");
            ushort X7 = new PCHMI.VAR().GET_BIT(0, "X7");
            if (X12 == 1)
            {
                trubleNum[0] = 1;
                truble = true;
            }
            if (X10 == 1)
            {
                trubleNum[1] = 1;
                truble = true;
            }
            if (X7 == 1)
            {
                trubleNum[2] = 1;
                truble = true;
            }
            if (truble)
            {
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        if (OverToken)
                        {
                            if (trubleNum[0] == 1)//故障1
                            {
                                new PCHMI.VAR().SEND_INT16(0, "D12", 1);
                                OverToken = false;
                            }
                            else if (trubleNum[1] == 1)//故障2
                            {
                                new PCHMI.VAR().SEND_INT16(0, "D13", 1);
                                OverToken = false;
                            }
                            else if (trubleNum[2] == 1)//故障3
                            {
                                new PCHMI.VAR().SEND_INT16(0, "D14", 1);
                                OverToken = false;
                            }
                            else
                                break;
                        }
                        else
                        {//监控排故是否完成
                            if (trubleNum[0] != 0)
                            {
                                short D12 = new PCHMI.VAR().GET_INT16(0, "D12");
                                OverToken = D12 == 10 ? true : false;
                                trubleNum[0] = D12 == 10 ? 0 : 1;
                            }
                            else if (trubleNum[1] != 0)
                            {
                                short D13 = new PCHMI.VAR().GET_INT16(0, "D13");
                                OverToken = D13 == 12 ? true : false;
                                trubleNum[1] = D13 == 12 ? 0 : 1;
                            }
                            else if (trubleNum[2] != 0)
                            {
                                short D14 = new PCHMI.VAR().GET_INT16(0, "D14");
                                OverToken = D14 == 10 ? true : false;
                                trubleNum[2] = D14 == 10 ? 0 : 1;
                            }
                        }
                    }
                }, token.Token);
            }

        }

        private void button22_Click(object sender, EventArgs e)
        {
            if (button22.Text == "更多")
            {
                pel_More.Visible = true;
                button22.Text = "关闭";
            }
            else
            {
                pel_More.Visible = false;
                button22.Text = "更多";
                button23.Enabled = false;
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            token.Cancel();
            button23.Enabled = false;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                dataGridView2.Rows[i].Cells[1].Value = dataGridView2.Rows[i].Cells[3].Value;
            }
            needsave = true;
        }
        
        private void button4_Click(object sender, EventArgs e)
        {
            int tempnum = Convert.ToInt32(this.textBox11.Text, 10);
            XmlNode shangpinconfignode = Form1.nodelistshangpin[0].ParentNode;
            if (tempnum > Form1.nodelistshangpin.Count)//需要增加商品数量
            {
                for (int i = Form1.nodelistshangpin.Count + 1; i <= tempnum; i++)
                {
                    //创建货道节点
                    XmlNode shangpinNode = Form1.myxmldoc.CreateElement("shangpin" + (i - 1).ToString());//商品定义
                    XmlAttribute shangpinnumAttribute = Form1.myxmldoc.CreateAttribute("shangpinnum");//商品编号
                    shangpinnumAttribute.Value = i.ToString("000");
                    shangpinNode.Attributes.Append(shangpinnumAttribute);//xml节点附件属性
                    XmlAttribute shangpinnameAttribute = Form1.myxmldoc.CreateAttribute("shangpinname");//对应商品名称
                    shangpinnameAttribute.Value = "";
                    shangpinNode.Attributes.Append(shangpinnameAttribute);//xml节点附件属性
                    XmlAttribute jiageAttribute = Form1.myxmldoc.CreateAttribute("jiage");//商品价格
                    jiageAttribute.Value = "1";
                    shangpinNode.Attributes.Append(jiageAttribute);//xml节点附件属性
                    XmlAttribute huodaoAttribute = Form1.myxmldoc.CreateAttribute("huodao");//货道定义
                    huodaoAttribute.Value = i.ToString();
                    shangpinNode.Attributes.Append(huodaoAttribute);//xml节点附件属性

                    XmlAttribute stateAttribute = Form1.myxmldoc.CreateAttribute("state");//货道状态
                    stateAttribute.Value = "0";//默认正常
                    shangpinNode.Attributes.Append(stateAttribute);//xml节点附件属性
                    XmlAttribute salesumAttribute = Form1.myxmldoc.CreateAttribute("salesum");//商品销售统计
                    salesumAttribute.Value = "0";//默认正常
                    shangpinNode.Attributes.Append(salesumAttribute);//xml节点附件属性

                    shangpinconfignode.AppendChild(shangpinNode);

                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[0].Value = shangpinNode.Attributes.GetNamedItem("shangpinnum").Value;
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[1].Value = shangpinNode.Attributes.GetNamedItem("jiage").Value;
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[2].Value = shangpinNode.Attributes.GetNamedItem("huodao").Value;
                    if (shangpinNode.Attributes.GetNamedItem("state").Value == "0")
                    {
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[3].Value = "正常";
                    }
                    else// if (_node.Attributes.GetNamedItem("state").Value == "1")
                    {
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[3].Value = "暂停";
                    }
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[4].Value = shangpinNode.Attributes.GetNamedItem("salesum").Value;
                }
            }
            else if (tempnum == 0)//商品数量为0
            {
                this.textBox11.Text = Form1.nodelistshangpin.Count.ToString();
            }
            else if (tempnum < Form1.nodelistshangpin.Count)//需要减少商品数量
            {
                for (int i = Form1.nodelistshangpin.Count; i > tempnum; i--)
                {
                    shangpinconfignode.RemoveChild(Form1.nodelistshangpin[i - 1]);
                    dataGridView1.Rows.RemoveAt(i - 1);
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            updatepic();//更新图片
            try
            {
                System.Diagnostics.Process[] MyProcesses = System.Diagnostics.Process.GetProcesses();
                foreach (System.Diagnostics.Process MyProcess in MyProcesses)
                {
                    if (MyProcess.ProcessName.CompareTo("ADHStart") == 0)
                    {
                        MyProcess.Kill();
                    }
                }
            }
            catch { }
            try
            {
                System.Diagnostics.Process.Start(Application.StartupPath + "\\upd.exe");
            }
            catch
            {

            }
            Form1.needcloseform = true;
            this.Close();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int tempnum = Convert.ToInt32(this.textBox12.Text, 10);
            XmlNode huodaoconfignode = Form1.nodelisthuodao[0].ParentNode;
            if (tempnum > Form1.nodelisthuodao.Count)//需要增加货道数量
            {
                for (int i = Form1.nodelisthuodao.Count + 1; i <= tempnum; i++)
                {
                    //创建货道节点
                    XmlNode huodaoNode = Form1.myxmldoc.CreateElement("huodao" + (i - 1).ToString());//货道定义
                    XmlAttribute huodaonumAttribute = Form1.myxmldoc.CreateAttribute("huodaonum");//货道编号
                    huodaonumAttribute.Value = i.ToString();
                    huodaoNode.Attributes.Append(huodaonumAttribute);//xml节点附件属性
                    XmlAttribute fenzuAttribute = Form1.myxmldoc.CreateAttribute("fenzu");//货道分组定义默认不分组
                    fenzuAttribute.Value = i.ToString();
                    huodaoNode.Attributes.Append(fenzuAttribute);//xml节点附件属性
                    XmlAttribute kucunAttribute = Form1.myxmldoc.CreateAttribute("kucun");//货道库存
                    kucunAttribute.Value = "255";
                    huodaoNode.Attributes.Append(kucunAttribute);//xml节点附件属性
                    XmlAttribute stateAttribute = Form1.myxmldoc.CreateAttribute("state");//货道状态
                    stateAttribute.Value = "0";//默认正常
                    huodaoNode.Attributes.Append(stateAttribute);//xml节点附件属性
                    XmlAttribute typeAttribute = Form1.myxmldoc.CreateAttribute("volume");//货道容量
                    typeAttribute.Value = "8";//默认正常
                    huodaoNode.Attributes.Append(typeAttribute);//xml节点附件属性
                    XmlAttribute positionAttribute = Form1.myxmldoc.CreateAttribute("position");//印章类型1：1010，2：2020，3：2530，其他2530
                    positionAttribute.Value = "0";//默认正常
                    huodaoNode.Attributes.Append(positionAttribute);//xml节点附件属性

                    huodaoconfignode.AppendChild(huodaoNode);

                    dataGridView2.Rows.Add();
                    dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[0].Value = huodaoNode.Attributes.GetNamedItem("huodaonum").Value;
                    dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[1].Value = huodaoNode.Attributes.GetNamedItem("kucun").Value;

                    if (huodaoNode.Attributes.GetNamedItem("state").Value == "0")
                    {
                        dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[2].Value = "正常";
                        stateOK++;
                    }
                    else if (huodaoNode.Attributes.GetNamedItem("state").Value == "1")
                    {
                        dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[2].Value = "过流";
                    }
                    else if (huodaoNode.Attributes.GetNamedItem("state").Value == "2")
                    {
                        dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[2].Value = "断线";
                    }
                    else// if (_node.Attributes.GetNamedItem("state").Value == "2")
                    {
                        dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[2].Value = "故障";
                    }
                    dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[3].Value = huodaoNode.Attributes.GetNamedItem("volume").Value;
                    dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[4].Value = huodaoNode.Attributes.GetNamedItem("position").Value;
                }
            }
            else if (tempnum == 0)//货道数量为0
            {
                this.textBox12.Text = Form1.nodelisthuodao.Count.ToString();
            }
            else if (tempnum < Form1.nodelisthuodao.Count)//需要减少商品数量
            {
                for (int i = Form1.nodelisthuodao.Count; i > tempnum; i--)
                {
                    huodaoconfignode.RemoveChild(Form1.nodelisthuodao[i - 1]);
                    dataGridView2.Rows.RemoveAt(i - 1);
                }
            }
        }
        
        private void button13_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            Form1.initsalexml();
            Form1.shipmentDoc.Save(Form1.salexmlfile);
            Form1.shipmentDoc.Save(Form1.salexmlfilecopy);
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells[4].Value = "0";
            }
            for (int i = 0; i < Form1.nodelistshangpin.Count; i++)
            {
                Form1.nodelistshangpin[i].Attributes.GetNamedItem("salesum").Value = "0";
            }
            needsave = true;
        }


        private void button20_Click(object sender, EventArgs e)
        {
            PEPrinter.pxSealLeftRight_try = Convert.ToInt16(textBox15.Text);
            PEPrinter.pxSealUpDown_try = Convert.ToInt16(textBox15.Text);
        }

        /// <summary>
        /// 复位打印机
        /// </summary>
        private void button28_Click(object sender, EventArgs e)
        {
            PEPrinter.needReset = true;
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        private void button25_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "打开印章制作图片";
            openFileDialog1.InitialDirectory = Application.StartupPath + "\\bcmimages";
            openFileDialog1.ShowDialog();
            try
            {
                pictureBox1.Load(openFileDialog1.FileName);
                pictureBox1.Height = (125 * pictureBox1.Image.Height) / pictureBox1.Image.Width;
                PEPrinter.PicPath = openFileDialog1.FileName;
                PEPrinter.TYPE_STAMP mytype;
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        mytype = PEPrinter.TYPE_STAMP.TYPE_1010;
                        pictureBox2.Width = 125;
                        pictureBox2.Height = 125;
                        break;
                    case 1:
                        mytype = PEPrinter.TYPE_STAMP.TYPE_2020;
                        pictureBox2.Width = 125;
                        pictureBox2.Height = 125;
                        break;
                    case 2:
                        mytype = PEPrinter.TYPE_STAMP.TYPE_2530;
                        pictureBox2.Width = 125;
                        pictureBox2.Height = 104;
                        break;
                    default:
                        mytype = PEPrinter.TYPE_STAMP.TYPE_2530;
                        pictureBox2.Width = 125;
                        pictureBox2.Height = 104;
                        break;
                }
                Bitmap tempbitmap = PEPrinter.CreateProcessingData(PEPrinter.PicPath, mytype);
                pictureBox2.Image = tempbitmap;
                
            }
            catch
            {

            }
            
        }

        /// <summary>
        /// 打印
        /// </summary>
        private void button26_Click(object sender, EventArgs e)
        {
            PEPrinter.needPutImage = true;
            PLCHelper.PrintFaceRecord(false);
        }

        /// <summary>
        /// 托盘操作
        /// </summary>
        private void button27_Click(object sender, EventArgs e)
        {
            PEPrinter.needMoveTray = comboBox9.SelectedIndex+1;
        }

        /// <summary>
        /// 托盘定位
        /// </summary>
        private void button24_Click(object sender, EventArgs e)
        {
            try
            {
                PEPrinter.TaryPosition = UInt16.Parse(textBox13.Text);
                PEPrinter.needMoveToOrigin = true;
            }
            catch
            {
                MessageBox.Show("托盘参数范围不正确.");
            }
        }

        /// <summary>
        /// 推送媒介
        /// </summary>
        private void button29_Click(object sender, EventArgs e)
        {
            try
            {
                PEPrinter.MediaPosition = UInt16.Parse(textBox16.Text);
                PEPrinter.MediaCoefficient = UInt16.Parse(textBox14.Text);
                PEPrinter.needPushMedia = true;
            }
            catch
            {
                MessageBox.Show("参数范围不正确.");
            }
            
        }

        private void btn_PlcInfo_Click(object sender, EventArgs e)
        {
            new PCHMI.UpConfig().ShowDialog();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            new PCHMI.VAR().SEND_INT16(0, "D209", 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PEPrinter.needReset = true;
        }

        private void btn_Quit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Login_Click(object sender, EventArgs e)
        {
            if (txt_Pass.Text == CPFRPass)
            {
                panel_Back.Visible = false;
                panel_CPFR.Visible = true;
            }
            else if (txt_Pass.Text == setupPass)
            {
                panel_Back.Visible = false;
                panel_setup.Visible = true;
            }
            else if (txt_Pass.Text == debugPass)
            {
                panel_Back.Visible = false;
                panel_debug.Visible = true;
            }
            else
            {
                MessageBox.Show("密码错误！！");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            PEPrinter.needMoveTray = comboBox2.SelectedIndex + 1;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (btn_RunType.Text == "关闭")
            {
                pel_runType.Visible = false;
                pel_PCControl.Visible = false;
                btn_RunType.Text = "运行模式选择";
            }
            else
            {
                pel_runType.Visible = true;
                btn_RunType.Text = "关闭";
                if (rb_PC.Checked)
                    pel_PCControl.Visible = true;
            }
        }

        #endregion

        #region hScrollBar

        private int PEPrinterupdatedelay;
        private void hScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            label31.Text = "设定输出灰度值:" + hScrollBar2.Value.ToString();
            PEPrinterupdatedelay = 1;
            needsave = true;
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            label30.Text = "设定橡胶打印行速:" + hScrollBar1.Value.ToString();
            PEPrinterupdatedelay = 1;
            needsave = true;
        }

        #endregion

        #region radioButton

        private void rb_PLC_Click(object sender, EventArgs e)
        {
            needsave = true;
            if (rb_PLC.Checked)
            {
                if (MessageBox.Show("要切换到当前模式需要先前打印机托盘内的印面拿出!!!\r\n(确认拿出后单击\"确认\"按钮)", "重要提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    PLCHelper.PrintFaceRecord(false);
                }
                else
                {
                }
            }
            if (rb_PLC.Checked)
                pel_PCControl.Visible = false;
        }

        private void rb_PC_Click(object sender, EventArgs e)
        {
            needsave = true;
            if (pel_runType.Visible == true && rb_PC.Checked)
                pel_PCControl.Visible = true;
        }

        private void rb_RunType1_Click(object sender, EventArgs e)
        {
            needsave = true;
        }

        private void rb_RunType2_Click(object sender, EventArgs e)
        {
            needsave = true;
        }

        #endregion

        #region label

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.Items.Count > 0)
            {
                listBox1.SelectedIndex = 0;
            }
        }

        private void label22_DoubleClick(object sender, EventArgs e)
        {
            CloseProgram();
        }
        
        private void label10_Click(object sender, EventArgs e)
        {
            CloseProgram();
        }

        private void label27_Click(object sender, EventArgs e)
        {
            CloseProgram();
        }

        private void label137_Click(object sender, EventArgs e)
        {
            CloseProgram();
        }

        #endregion

        #region CameraSet

        private void button9_Click(object sender, EventArgs e)//打开摄像头设置页面
        {
            InitCamera();
        }

        private void InitCamera()
        {
            pel_CameraSet.Location = new Point(450, 300);
            pel_CameraSet.Visible = true;
            pel_CameraSet.BringToFront();
            camera = CameraHelper.GetCameraExample();
            camera.IniCamera();
            nowCamera = camera.Camera1;

            cbx_cameraFro.Items.Add("顶部相机");//位置相机选择
            cbx_cameraFro.Items.Add("底部相机");
            cbx_cameraFro.SelectedIndex = 0;

            try
            {
                tempDevice = nowCamera.VideoDevice;
                video2.VideoSource = tempDevice;
                video2.Start();

                //获取分辨率
                foreach (var rp in tempDevice.VideoCapabilities)
                {
                    cbx_Rp.Items.Add(rp.FrameSize.Width + "x" + rp.FrameSize.Height);
                }
                try
                {
                    cbx_Rp.SelectedIndex = nowCamera.CapabilitieItem;
                }
                catch { }
            }
            catch { }

            //获取摄像列表
            foreach (FilterInfo item in camera.VideoDevices)
            {
                cbx_PicFrom.Items.Add(item.Name);
                if (item.Name == nowCamera.CameraName)
                    cbx_PicFrom.Text = nowCamera.CameraName;
                else
                {
                    cbx_PicFrom.SelectedIndex = 0;
                }
            }
            //添加图片类型
            cbx_PicType.Items.AddRange(camera.PicTypes);
            cbx_PicType.Text = nowCamera.PicType;

            //添加水印类型
            cbx_PicWatermark.Items.AddRange(camera.WatermarkTypes);
            cbx_PicWatermark.Text = nowCamera.WatermarkType;

            //字体大小
            nud_fonSize.Value = nowCamera.WaterFontSzie;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                tempDevice.DisplayPropertyPage(IntPtr.Zero);//打开摄像头属性设置
            }
            catch { }
        }

        private void cbx_PicFrom_SelectedIndexChanged(object sender, EventArgs e)//切换摄像头
        {
            try
            {
                video2.SignalToStop();
                video2.WaitForStop();
                tempDevice.SignalToStop();
                tempDevice.WaitForStop();
                video2.Stop();
                video2.VideoSource = null;
                tempDevice.Stop();
            }
            catch { }
            //获取分辨率
           
            try
            {
                tempDevice = new VideoCaptureDevice(camera.VideoDevices[cbx_PicFrom.SelectedIndex].MonikerString);
                video2.VideoSource = tempDevice;
                video2.SignalToStop();
                video2.WaitForStop();
                video2.Start();
                cbx_Rp.Items.Clear();
                foreach (var rp in tempDevice.VideoCapabilities)
                {
                    cbx_Rp.Items.Add(rp.FrameSize.Width + "x" + rp.FrameSize.Height);
                }
                cbx_Rp.SelectedIndex = nowCamera.CapabilitieItem;
            }
            catch { }
        }

        private void cbx_cameraFro_SelectedIndexChanged(object sender, EventArgs e)
        {
            nowCamera = cbx_cameraFro.SelectedIndex == 0 ? camera.Camera1 : camera.Camera2;
            cbx_PicFrom.SelectedIndex = cbx_PicFrom.Items.IndexOf(nowCamera.CameraName);

        }

        private void video2_NewFrame(object sender, ref Bitmap image)//水印
        {
            if (cbx_PicWatermark.SelectedItem.ToString() != "None")
            {
                Graphics grap = Graphics.FromImage(image);
                SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.Red);
                Font drawFont = new Font("Arial", (int)nud_fonSize.Value, System.Drawing.FontStyle.Bold, GraphicsUnit.Millimeter);
                int xPos = image.Width - (image.Width - 15);
                int yPos = 10;
                string drawString;
                if (cbx_PicWatermark.SelectedItem.ToString() == "DateTime")//提货码样式
                {
                    drawString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    drawString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  PickingCode : 1234567";
                }
                grap.DrawString(drawString, drawFont, drawBrush, xPos, yPos);
            }
        }

        private void button16_Click(object sender, EventArgs e)//退出设置
        {
            try
            {
                video2.SignalToStop();
                video2.WaitForStop();
                video2.Stop();
                tempDevice.SignalToStop();
                tempDevice.WaitForStop();
                tempDevice.Stop();
            }
            catch { }
            cbx_PicFrom.Items.Clear();
            cbx_PicType.Items.Clear();
            cbx_Rp.Items.Clear();
            cbx_PicWatermark.Items.Clear();
            this.pel_CameraSet.Visible = false;

        }

        private void cbx_Rp_SelectedIndexChanged(object sender, EventArgs e)//切换分辨率
        {
            video2.SignalToStop();
            video2.WaitForStop();
            tempDevice.SignalToStop();
            tempDevice.WaitForStop();
            tempDevice.VideoResolution = tempDevice.VideoCapabilities[cbx_Rp.SelectedIndex];
            tempDevice.Start();
            video2.VideoSource = tempDevice;
            video2.Start();
        }

        private void button15_Click(object sender, EventArgs e)//保存摄像头配置
        {

            nowCamera.PicType = cbx_PicType.SelectedItem.ToString();//图片类型
            nowCamera.WatermarkType = cbx_PicWatermark.SelectedItem.ToString();//水印类型
            nowCamera.CameraName = cbx_PicFrom.SelectedItem.ToString();//摄像头名称
            nowCamera.CapabilitieItem = cbx_Rp.SelectedIndex;//分辨率
            nowCamera.WaterFontSzie = (int)nud_fonSize.Value;//字体大小
            nowCamera.VideoDevice = tempDevice;//摄像源

            string cameraIndex;
            if (cbx_cameraFro.SelectedIndex == 0)
            {
                camera.Camera1 = nowCamera;
                cameraIndex = "Camera1";
            }
            else
            {
                camera.Camera2 = nowCamera;
                cameraIndex = "Camera2";
            }
            //保存配制
            Form1.IniWriteValue(cameraIndex, "cameraName", cbx_PicFrom.SelectedItem.ToString(), Form1.cameraParaFile);
            Form1.IniWriteValue(cameraIndex, "capabilitieItem", cbx_Rp.SelectedIndex.ToString(), Form1.cameraParaFile);
            Form1.IniWriteValue(cameraIndex, "fontSize", nud_fonSize.Value.ToString(), Form1.cameraParaFile);
            Form1.IniWriteValue(cameraIndex, "watermarkType", cbx_PicWatermark.SelectedItem.ToString(), Form1.cameraParaFile);
            bool result=Form1.IniWriteValue(cameraIndex, "picType", cbx_PicType.SelectedItem.ToString(), Form1.cameraParaFile);

            if(result)
                MessageBox.Show("配制保存成功");
            else
                MessageBox.Show("配制保存失败");
        }

        private void button18_Click(object sender, EventArgs e)//打开图片
        {
            try
            {
                System.Diagnostics.Process.Start(photoTestPath);
            }
            catch { }
        }

        private void button17_Click(object sender, EventArgs e)//拍照
        {
            try
            {
                Bitmap bmp = video2.GetCurrentVideoFrame();
                string picName = photoTestPath+"\\"  +"PhotoTest"+ DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss") + "." + cbx_PicType.SelectedItem.ToString();
                ImageFormat format;
                switch (cbx_PicType.SelectedItem.ToString())//图片格式
                {
                    case "Jpeg":
                        format = ImageFormat.Jpeg;
                        break;
                    case "Bmp":
                        format = ImageFormat.Bmp;
                        break;
                    case "Png":
                        format = ImageFormat.Png;
                        break;
                    case "Gif":
                        format = ImageFormat.Gif;
                        break;
                    default:
                        format = ImageFormat.Jpeg;
                        break;
                }
                bmp.Save(picName, format);
                MessageBox.Show("保存成功");
            }
            catch(Exception ex)
            {
                MessageBox.Show("保存失败"+ex.Message);
            }
        }

        private void nud_fonSize_ValueChanged(object sender, EventArgs e)//字体大小限制
        {
            if(nud_fonSize.Value <= 0)
            {
                nud_fonSize.Value = 1;
            }
        }


        #endregion
        
    }
}
