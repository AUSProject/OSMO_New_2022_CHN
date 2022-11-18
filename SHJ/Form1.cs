using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using Ibms.Net.TcpCSFramework;
using ThoughtWorks.QRCode.Codec;    
using System.Threading.Tasks;

namespace SHJ
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll", EntryPoint = "ShowCursor", CharSet = CharSet.Auto)]
        public extern static void ShowCursor(int status);

        #region Ini

        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);

        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern bool GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern uint GetPrivateProfileStringA(string section, string key, string defVal, byte[] retVal, uint size, string filePath);

        /// <summary>
        /// 写入ini文件
        /// </summary>
        /// <param name="section">名称</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="path">路径</param>
        private void IniWriteValue(string section, string key, string value, string path)
        {
            WritePrivateProfileString(section, key, value, path);
        }

        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="section">项目名称</param>
        /// <param name="key">键</param>
        /// <param name="path">路径</param>
        private string IniReadValue(string section, string key, string path)
        {
            StringBuilder temp = new StringBuilder(6000);
            GetPrivateProfileString(section, key, "error", temp, 6000, path);
            return temp.ToString();
        }

        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="section"></param>
        /// <param name="path"></param>
        private void DeleteSection(string section, string path)
        {
            WritePrivateProfileString(section, null, null, path);
        }

        private List<string> ReadSections(string filePath)
        {
            List<string> sections = new List<string>();
            byte[] buff = new byte[1000];
            var charLength = GetPrivateProfileStringA(null, null, "", buff, 1000, imageUrlPath);
            int j = 0;
            for (int i = 0; i < charLength; i++)
            {
                if (buff[i] == 0)
                {
                    sections.Add(Encoding.UTF8.GetString(buff, j, i - j));
                    j = i + 1;
                }
            }
            return sections;
        }

        #endregion

        #region Form1MethodCallBack

        private static Form1 nowform1;
        /// <summary>
        /// 模拟运行
        /// </summary>
        /// <param name="num">货道号</param>
        /// <param name="path">图片地址</param>
        public static void CallWorkingTest(int num, string path)
        {
            if (nowform1 != null)
            {
                nowform1.WorkingTest(num, path);
            }
        }
        
        
        #endregion

        #region Feild
        
        private PrintHelper print = null;
        public static string LogPath=null;
        
        private string imageUrlPath;//印章图片URL文件夹
        private string ErweimaUrl = "https://fun.shachihata-china.com/boot/make/qmyz/SHAK/";//二维码地址
        PLCHelper PLC;//机器控制
        LogHelper log;//运行日志

        public static string runningImage;

        public static bool needcloseform = false;//是否需要关闭窗体
        public static int HMIstep;//界面页面：0广告 1触摸选择商品 2支付页面
        //private const int RXTXBUFLEN = 512;
        private const int GSMRXTXBUFLEN = 1500;
        private setting mysetting;//设置窗口

        private int guanggaoindex = 0;//广告文件夹中图片索引号 
        public static string adimagesaddress;//广告图片路径
        public static string bkimagesaddress;//背景图片路径
        public static string cmimagesaddress;//商品图片路径
        public static string bcmimagesaddress;//商品图片路径
        public static string usedbcmimagesaddress;//已经提货的打印图片
        public static string dataaddress;
        public static FileStream netdatastream;
        public static string[] adimagefiles;//广告图片名
        public static bool needupdatePlaylist;//是否需要更新播放列表
        public static string[] cmimagefiles;//商品图片名
        public static string[] bcmimagefiles;//商品图片名
        public static string configxmlfile;//配置文件名
        public static string salexmlfile;//销售记录文件名
        public static string configxmlfilecopy;//配置文件名
        public static string salexmlfilecopy;//销售记录文件名
        private string regxmlfile;//注册文件名
        public static XmlDocument myxmldoc = new XmlDocument();//配置文件XML
        public static XmlNodeList mynodelistshangpin;//商品列表
        public static XmlNodeList mynodelisthuodao;//货道列表
        public static XmlNode mynetcofignode;//网络配置
        public static XmlNode myfunctionnode;//功能配置
        public static XmlNode mypayconfignode;//支付配置
        public static XmlDocument mysalexmldoc = new XmlDocument();//销售记录配置文件XML
        public static XmlNodeList mynodelistpay;//支付记录

        public static string localsalerID = "";//本机商家号
        public static string vendortype = "0";//机器类型
        private XmlDocument myregxmldoc = new XmlDocument();//注册配置文件XML
        private bool isregedit = false;//是否已经注册
        public static int guanggaoreturntime;//返回广告页面计时。3分钟不操作，则返回广告页面
        private int MAXreturntime = 120;
        private QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();//二维码
        private PEPrinter myprint;
        
        public static int paytypes;//第一位为支付宝、第二位为微信、第三位为一码付、第四位为银联闪付、第五位为会员卡

        //GPRS变量
        private byte[] GSMRxBuffer;  //GSM发送缓冲区
        private byte[] GSMTxBuffer = new byte[GSMRXTXBUFLEN];  //GSM接收缓冲区
        private byte[,] timerecord = new byte[4, 6];//二维码时间戳+下发印章图片时间戳
        private byte[,] netsendrecord = new byte[210, 34];//缓存200条
        private int netsendindex, netsendrecordindex, needsendrecordnum;//发送和缓存序号已经需要发送的条数
        private int netstep;//网络状态 0表示空闲 1等待返回2表示发送状态数据 3表示发送交易数据 4表示发送支付宝二维码请求数据5表示发送微信二维码请求数据
        private TcpCli myTcpCli = new TcpCli(new Coder(Coder.EncodingMothord.Unicode));//定义网络客户端
        public static bool isICYOK = false;//与服务器握手成功

        private string ipAddress;//服务器IP地址 
        private int netport;//服务器网络端口号
        private string myMAC;//MAC地址
        public static byte[] IMEI = new byte[15];//设备唯一号
        public static string versionstring = "ADH816AZV3.3.02";
        private int liushuirecv;//接收到的流水号
        public static string myTihuomastr = "";//输入的7位提货码
        public static bool checktihuoma;//需要验证提货码
        public static string showprintstate;//制作过程状态显示
        public static int OSMOtype;
        
        public static int wulihuodao;//物理货道号
        private double shangpinjiage;
        private int zhifutype;//0现金1支付宝2微信3一码付4提货码
        private int totalshangpinnum = 16;//显示的商品总数
        private int totalhuodaonum = 16;//显示的货道总数
        
        private int Aisleoutcount;//电机输出超时计时
        public static bool istestmode;//测试出货模式

        #endregion

        #region Crc

        //private UInt16[] CrcTbl =
        //{
        //    0x0000, 0xC0C1, 0xC181, 0x0140, 0xC301, 0x03C0, 0x0280, 0xC241,
        //    0xC601, 0x06C0, 0x0780, 0xC741, 0x0500, 0xC5C1, 0xC481, 0x0440,
        //    0xCC01, 0x0CC0, 0x0D80, 0xCD41, 0x0F00, 0xCFC1, 0xCE81, 0x0E40,
        //    0x0A00, 0xCAC1, 0xCB81, 0x0B40, 0xC901, 0x09C0, 0x0880, 0xC841,
        //    0xD801, 0x18C0, 0x1980, 0xD941, 0x1B00, 0xDBC1, 0xDA81, 0x1A40,
        //    0x1E00, 0xDEC1, 0xDF81, 0x1F40, 0xDD01, 0x1DC0, 0x1C80, 0xDC41,
        //    0x1400, 0xD4C1, 0xD581, 0x1540, 0xD701, 0x17C0, 0x1680, 0xD641,
        //    0xD201, 0x12C0, 0x1380, 0xD341, 0x1100, 0xD1C1, 0xD081, 0x1040,
        //    0xF001, 0x30C0, 0x3180, 0xF141, 0x3300, 0xF3C1, 0xF281, 0x3240,
        //    0x3600, 0xF6C1, 0xF781, 0x3740, 0xF501, 0x35C0, 0x3480, 0xF441,
        //    0x3C00, 0xFCC1, 0xFD81, 0x3D40, 0xFF01, 0x3FC0, 0x3E80, 0xFE41,
        //    0xFA01, 0x3AC0, 0x3B80, 0xFB41, 0x3900, 0xF9C1, 0xF881, 0x3840,
        //    0x2800, 0xE8C1, 0xE981, 0x2940, 0xEB01, 0x2BC0, 0x2A80, 0xEA41,
        //    0xEE01, 0x2EC0, 0x2F80, 0xEF41, 0x2D00, 0xEDC1, 0xEC81, 0x2C40,
        //    0xE401, 0x24C0, 0x2580, 0xE541, 0x2700, 0xE7C1, 0xE681, 0x2640,
        //    0x2200, 0xE2C1, 0xE381, 0x2340, 0xE101, 0x21C0, 0x2080, 0xE041,
        //    0xA001, 0x60C0, 0x6180, 0xA141, 0x6300, 0xA3C1, 0xA281, 0x6240,
        //    0x6600, 0xA6C1, 0xA781, 0x6740, 0xA501, 0x65C0, 0x6480, 0xA441,
        //    0x6C00, 0xACC1, 0xAD81, 0x6D40, 0xAF01, 0x6FC0, 0x6E80, 0xAE41,
        //    0xAA01, 0x6AC0, 0x6B80, 0xAB41, 0x6900, 0xA9C1, 0xA881, 0x6840,
        //    0x7800, 0xB8C1, 0xB981, 0x7940, 0xBB01, 0x7BC0, 0x7A80, 0xBA41,
        //    0xBE01, 0x7EC0, 0x7F80, 0xBF41, 0x7D00, 0xBDC1, 0xBC81, 0x7C40,
        //    0xB401, 0x74C0, 0x7580, 0xB541, 0x7700, 0xB7C1, 0xB681, 0x7640,
        //    0x7200, 0xB2C1, 0xB381, 0x7340, 0xB101, 0x71C0, 0x7080, 0xB041,
        //    0x5000, 0x90C1, 0x9181, 0x5140, 0x9301, 0x53C0, 0x5280, 0x9241,
        //    0x9601, 0x56C0, 0x5780, 0x9741, 0x5500, 0x95C1, 0x9481, 0x5440,
        //    0x9C01, 0x5CC0, 0x5D80, 0x9D41, 0x5F00, 0x9FC1, 0x9E81, 0x5E40,
        //    0x5A00, 0x9AC1, 0x9B81, 0x5B40, 0x9901, 0x59C0, 0x5880, 0x9841,
        //    0x8801, 0x48C0, 0x4980, 0x8941, 0x4B00, 0x8BC1, 0x8A81, 0x4A40,
        //    0x4E00, 0x8EC1, 0x8F81, 0x4F40, 0x8D01, 0x4DC0, 0x4C80, 0x8C41,
        //    0x4400, 0x84C1, 0x8581, 0x4540, 0x8701, 0x47C0, 0x4680, 0x8641,
        //    0x8201, 0x42C0, 0x4380, 0x8341, 0x4100, 0x81C1, 0x8081, 0x4040
        //};

        ////查表算法CRC-16
        //UInt16 crcVal(byte[] pcMess, UInt16 wLen)
        //{
        //    int i = 0;
        //    UInt16 nCRCData, Index = 0;
        //    nCRCData = 0xffff;
        //    while (wLen > 0)
        //    {
        //        wLen--;
        //        Index = (UInt16)(nCRCData >> 8);
        //        Index = (UInt16)(Index ^ (pcMess[i++] & 0x00ff));
        //        nCRCData = (UInt16)((nCRCData ^ CrcTbl[Index]) & 0x00ff);
        //        nCRCData = (UInt16)((nCRCData << 8) | (CrcTbl[Index] >> 8));
        //    }
        //    return (UInt16)(nCRCData >> 8 | nCRCData << 8);
        //}

        #endregion

        #region  Load

        private void Form1_Load(object sender, EventArgs e)
        {
            nowform1 = this;
            pic_Erweima.Visible = false;//隐藏二维码
            
            PLC = new PLCHelper();
            print = PrintHelper.GetExample();//获取打印机实例
            log = LogHelper.GetLogHelperExamlpe();//获取日志实例
            
            config1.START((Control)this, System.Reflection.Assembly.GetExecutingAssembly(), null);
            
            this.panel1.Dock = DockStyle.Fill;
            this.panel4.Dock = DockStyle.Fill;

            imageUrlPath = Directory.GetCurrentDirectory() + "\\imageUrl.ini";
            LogPath = System.IO.Directory.GetCurrentDirectory() + "\\Log";
            runningImage= Directory.GetCurrentDirectory() + "\\runningImage";

            adimagesaddress = System.IO.Directory.GetCurrentDirectory() + "\\adimages";
            bkimagesaddress = System.IO.Directory.GetCurrentDirectory() + "\\bkimages";
            cmimagesaddress = System.IO.Directory.GetCurrentDirectory() + "\\cmimages";
            bcmimagesaddress = System.IO.Directory.GetCurrentDirectory() + "\\bcmimages";
            usedbcmimagesaddress = bcmimagesaddress + "\\used";
            configxmlfile = System.IO.Directory.GetCurrentDirectory() + "\\app.dat";
            salexmlfile = System.IO.Directory.GetCurrentDirectory() + "\\sale.dat";
            configxmlfilecopy = System.IO.Directory.GetCurrentDirectory() + "\\app.xml";
            salexmlfilecopy = System.IO.Directory.GetCurrentDirectory() + "\\sale.xml";
            dataaddress = System.IO.Directory.GetCurrentDirectory() + "\\netdata";
            regxmlfile = "C:\\flexlm\\regEPTON.dll";
            if (System.IO.Directory.Exists(adimagesaddress) == false)//广告文件夹不存在
            {
                System.IO.Directory.CreateDirectory(adimagesaddress);
            }
            if (System.IO.Directory.Exists(bkimagesaddress) == false)//背景文件夹不存在
            {
                System.IO.Directory.CreateDirectory(bkimagesaddress);
            }
            if (System.IO.Directory.Exists(cmimagesaddress) == false)//商品文件夹不存在
            {
                System.IO.Directory.CreateDirectory(cmimagesaddress);
            }
            if (System.IO.Directory.Exists(bcmimagesaddress) == false)//商品文件夹不存在
            {
                System.IO.Directory.CreateDirectory(bcmimagesaddress);
            }
            if (System.IO.Directory.Exists(usedbcmimagesaddress) == false)//广告文件夹不存在
            {
                System.IO.Directory.CreateDirectory(usedbcmimagesaddress);
            }
            if (System.IO.Directory.Exists("C:\\flexlm") == false)//注册文件夹不存在
            {
                System.IO.Directory.CreateDirectory("C:\\flexlm");
            }
            if (System.IO.Directory.Exists(dataaddress) == false)//netdata文件夹不存在
            {
                System.IO.Directory.CreateDirectory(dataaddress);
            }
            if (!File.Exists(imageUrlPath))//二维码图片路径文件
            {
                File.Create(imageUrlPath);
            }
            if (!Directory.Exists(LogPath))//日志文件路径
            {
                Directory.CreateDirectory(LogPath);
            }
            if(Directory.Exists(runningImage)==false)
            {
                Directory.CreateDirectory(runningImage);
            }
            if (System.IO.File.Exists(configxmlfile))
            {
                try
                {
                    myxmldoc.Load(configxmlfile);
                    myxmldoc.Save(configxmlfilecopy);
                }
                catch
                {
                    if (System.IO.File.Exists(configxmlfilecopy))
                    {
                        try
                        {
                            myxmldoc.Load(configxmlfilecopy);
                            myxmldoc.Save(configxmlfile);
                        }
                        catch
                        {
                            initconfigxml();
                            myxmldoc.Save(configxmlfile);
                            myxmldoc.Save(configxmlfilecopy);
                        }
                    }
                }
            }
            else if (System.IO.File.Exists(configxmlfilecopy))
            {
                try
                {
                    myxmldoc.Load(configxmlfilecopy);
                    myxmldoc.Save(configxmlfile);
                }
                catch
                {
                    initconfigxml();
                    myxmldoc.Save(configxmlfile);
                    myxmldoc.Save(configxmlfilecopy);
                }
            }
            else
            {
                initconfigxml();
                myxmldoc.Save(configxmlfile);
                myxmldoc.Save(configxmlfilecopy);

            }

            if (System.IO.File.Exists(salexmlfile))
            {
                try
                {
                    mysalexmldoc.Load(salexmlfile);
                    mysalexmldoc.Save(salexmlfilecopy);
                }
                catch
                {
                    if (System.IO.File.Exists(salexmlfilecopy))
                    {
                        try
                        {
                            mysalexmldoc.Load(salexmlfilecopy);
                            mysalexmldoc.Save(salexmlfile);
                        }
                        catch
                        {
                            initsalexml();
                            mysalexmldoc.Save(salexmlfile);
                            mysalexmldoc.Save(salexmlfilecopy);
                        }
                    }
                }
            }
            else if (System.IO.File.Exists(salexmlfilecopy))
            {
                try
                {
                    mysalexmldoc.Load(salexmlfilecopy);
                    mysalexmldoc.Save(salexmlfile);
                }
                catch
                {
                    initsalexml();
                    mysalexmldoc.Save(salexmlfile);
                    mysalexmldoc.Save(salexmlfilecopy);
                }
            }
            else
            {
                initsalexml();
                mysalexmldoc.Save(salexmlfile);
                mysalexmldoc.Save(salexmlfilecopy);
            }
            

            if (System.IO.File.Exists(regxmlfile))//加载注册文件
            {
                myregxmldoc.Load(regxmlfile);
                string adress = myregxmldoc.SelectSingleNode("reg").Attributes.GetNamedItem("regid").Value;
                ErweimaUrl = String.Concat(ErweimaUrl, adress);
                if (!File.Exists(Path.Combine(adimagesaddress, "erweima.jpg")))
                {
                    QRCode(ErweimaUrl);
                }
            }
            else
            {
                initregxml();
                myregxmldoc.Save(regxmlfile);
            }
            try
            {
                updatenodeaddress();
                InitFormsize();
                //ShowCursor(0);//关闭鼠标
                HMIstep = 0;//触摸选货界面

                adimagefiles = System.IO.Directory.GetFiles(adimagesaddress);//广告页面图片文件路径列表
                if (adimagefiles != null)
                {
                    if (guanggaoindex >= adimagefiles.Length)
                    {
                        guanggaoindex = 0;
                    }

                    bool ispicture = adimagefiles[guanggaoindex].EndsWith(".bmp") || adimagefiles[guanggaoindex].EndsWith(".jpg")
                                    || adimagefiles[guanggaoindex].EndsWith(".png") || adimagefiles[guanggaoindex].EndsWith(".gif")
                                    || adimagefiles[guanggaoindex].EndsWith(".tif") || adimagefiles[guanggaoindex].EndsWith(".jpeg");//是否是图片
                    if (ispicture)//是图片
                    {
                        this.pictureBox1.Load(adimagefiles[guanggaoindex]);
                    }
                }

                cmimagefiles = System.IO.Directory.GetFiles(cmimagesaddress);//选择商品图片文件路径列表
                bcmimagefiles = System.IO.Directory.GetFiles(bcmimagesaddress);//选择商品图片文件路径列表
                string str1;
                for (int i = 0; i < cmimagefiles.Length; i++)//文件名称排序
                {
                    for (int j = cmimagefiles.Length - 1; j > i; j--)
                    {
                        if (cmimagefiles[j].CompareTo(cmimagefiles[j - 1]) < 0)
                        {
                            str1 = cmimagefiles[j];
                            cmimagefiles[j] = cmimagefiles[j - 1];
                            cmimagefiles[j - 1] = str1;
                        }
                    }
                }
                
                if (myfunctionnode.Attributes.GetNamedItem("netlog").Value == "1")
                {
                    dataaddress += "\\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";
                    netdatastream = System.IO.File.Create(dataaddress);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)//查找有线网
            {
                if ((!ni.Description.Contains("Wireless")) && (ni.Description.Contains("Realtek PCIe")) && (!ni.GetPhysicalAddress().ToString().Equals("")))
                {
                    myMAC = ni.GetPhysicalAddress().ToString().ToUpper();
                }
            }
            if (myMAC == null)
            {
                foreach (NetworkInterface ni in interfaces)//查找无线网络
                {
                    if ((ni.Description.Contains("Wireless")) && (!ni.GetPhysicalAddress().ToString().Equals("")))
                    {
                        myMAC = ni.GetPhysicalAddress().ToString().ToUpper();
                    }
                }
            }
            if (myMAC != null)
            {
                byte[] mbyteMAC = Encoding.ASCII.GetBytes(myMAC);
                byte adddata = 0;
                for (int i = 0; i < 12; i++)
                {
                    adddata += mbyteMAC[i];
                    IMEI[i] = mbyteMAC[i];
                }
                byte[] mbyteadddata = Encoding.ASCII.GetBytes(adddata.ToString("000"));
                IMEI[12] = mbyteadddata[0];
                IMEI[13] = mbyteadddata[1];
                IMEI[14] = mbyteadddata[2];
            }
            //判断是否注册
            UInt64 mregdata = UInt64.Parse(myregxmldoc.SelectSingleNode("reg").Attributes.GetNamedItem("regid").Value);
            UInt64 mimeidata = 0;
            for (int i = 0; i < 15; i++)
            {
                mimeidata = (mimeidata << 8) + (byte)(IMEI[i] & 0x77);
            }
            if (mimeidata != mregdata)
            {
                isregedit = false;
            }
            else
            {
                isregedit = true;
            }

            myprint = PEPrinter.GetPEPrinterExample();
            
            myTcpCli.ReceivedDatagram += new NetEvent(myTcpCli_ReceivedDatagram);
            myTcpCli.DisConnectedServer += new NetEvent(myTcpCli_DisConnectedServer);
            myTcpCli.ConnectedServer += new NetEvent(myTcpCli_ConnectedServer);

            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;//二维码
            qrCodeEncoder.QRCodeScale = 5;
            qrCodeEncoder.QRCodeVersion = 8;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;

            pic_Erweima.Image = Image.FromFile(Path.Combine(adimagesaddress, "erweima.jpg"));
          
            PLCHelper.isAutoRun = myfunctionnode.Attributes.GetNamedItem("isAutoRun").Value == "true" ? true : false;
            PLCHelper._MachineRunPlan = myfunctionnode.Attributes.GetNamedItem("runType").Value;
            PLCHelper.isRigPrint = myfunctionnode.Attributes.GetNamedItem("isRigPrint").Value == "True" ? true : false;

            setting.CPFRPass = myfunctionnode.Attributes.GetNamedItem("CPFRPass").Value;
            setting.debugPass = myfunctionnode.Attributes.GetNamedItem("debugPass").Value;
            setting.setupPass = myfunctionnode.Attributes.GetNamedItem("setupPass").Value;

        }

        #endregion

        #region Timer1

        private int netcount;//网络状态循环计时
        private int netreturncount;//网络等待计时
        private int myminute;//分钟计时
        List<string> imageNames = new List<string>();
        private void timer1_Tick(object sender, EventArgs e)//1s
        {
            imageNames = ReadSections(imageUrlPath);
            foreach(var picFile in imageNames)
            {
                string picUrl = IniReadValue(picFile, "url", imageUrlPath);
                bool picExist = Directory.GetFiles(bcmimagesaddress, Path.GetFileNameWithoutExtension(picFile)).Length > 0 ? true : false;
                if (picExist)
                {
                    FileInfo fileInfo = new FileInfo(picFile);
                    long len = 0;
                    if (fileInfo.Length == 0)
                    {
                        len = fileInfo.Length;
                        while (len < 5)
                        {
                            DownLoadPicture(picUrl, picFile);
                            fileInfo.Refresh();
                            len = len + fileInfo.Length + 1;
                        }
                        fileInfo.Refresh();
                        if (fileInfo.Length > 0)
                        {
                            DeleteSection(picFile, imageUrlPath);
                        }
                    }
                    else
                    {
                        DeleteSection(picFile, imageUrlPath);
                    }
                }
            }//重新下载空包图片

            for (int i = 0; i < imageNames.Count; i++)
            {
                string name = imageNames[i];
                string url = IniReadValue(name, "url", imageUrlPath);
                bool alikeName = false;
                for (int m = 0; m < bcmimagefiles.Length; m++)//文件名称排序
                {
                    if (bcmimagefiles[m].Contains(name))
                    {
                        alikeName = true;
                        FileInfo file = new FileInfo(bcmimagefiles[m]);
                        long len = 0;
                        if (file.Length == 0)
                        {
                            len = file.Length;
                            while (len < 5)
                            {
                                DownLoadPicture(url, name);
                                file.Refresh();
                                len = len + file.Length + 1;
                            }
                            file.Refresh();
                            if (file.Length > 0)
                            {
                                DeleteSection(name, imageUrlPath);
                            }
                        }
                    }
                }
                if (!alikeName)
                    DeleteSection(name, imageUrlPath);
            }

            if (myminute < 59)//最长1分钟，循环时间
            {
                myminute++;
            }
            else
            {
                myminute = 0;
            }
            if (guanggaoreturntime < 600)//最长计时10分钟
            {
                guanggaoreturntime++;
            }
            switch (HMIstep)
            {
                case 0:
                    //刷新广告页面
                    if (needupdatePlaylist)
                    {
                        needupdatePlaylist = false;
                        axWindowsMediaPlayer1.currentPlaylist.clear();
                        //检测播放列表是否更新
                        for (int i = 0; i < adimagefiles.Length; i++)//广告文件名列表
                        {

                            int mystartindex = adimagefiles[i].LastIndexOf('\\');
                            int myendindex = adimagefiles[i].LastIndexOf('.');
                            bool myisvideo = adimagefiles[i].EndsWith(".wav") || adimagefiles[i].EndsWith(".mid")
                                || adimagefiles[i].EndsWith(".mp4") || adimagefiles[i].EndsWith(".mp3")
                                || adimagefiles[i].EndsWith(".mpg") || adimagefiles[i].EndsWith(".avi")
                                || adimagefiles[i].EndsWith(".asf") || adimagefiles[i].EndsWith(".wmv")
                                || adimagefiles[i].EndsWith(".rm") || adimagefiles[i].EndsWith(".rmvb");
                            if ((mystartindex >= 0) && (myendindex >= 0) && (myisvideo == true))//文件名正确
                            {
                                axWindowsMediaPlayer1.currentPlaylist.appendItem(axWindowsMediaPlayer1.newMedia(adimagefiles[i]));
                            }
                        }
                        if (axWindowsMediaPlayer1.currentPlaylist.count > 0)//有播放文件
                        {
                            axWindowsMediaPlayer1.Visible = true;
                            axWindowsMediaPlayer1.Ctlcontrols.play();
                        }
                        else
                        {
                            axWindowsMediaPlayer1.Visible = false;
                            axWindowsMediaPlayer1.Ctlcontrols.stop();
                        }
                    }
                    if (guanggaoreturntime >= 3)
                    {
                        guanggaoreturntime = 0;

                        try
                        {
                            //播放图片
                            if (adimagefiles != null)
                            {
                                if (guanggaoindex >= adimagefiles.Length)
                                {
                                    guanggaoindex = 0;
                                }
                                bool ispicture = adimagefiles[guanggaoindex].EndsWith(".bmp") || adimagefiles[guanggaoindex].EndsWith(".jpg")
                                    || adimagefiles[guanggaoindex].EndsWith(".png") || adimagefiles[guanggaoindex].EndsWith(".gif")
                                    || adimagefiles[guanggaoindex].EndsWith(".tif") || adimagefiles[guanggaoindex].EndsWith(".jpeg");//是否是图片
                                if (ispicture)//是图片
                                {
                                    this.pictureBox1.Load(adimagefiles[guanggaoindex]);
                                }
                                guanggaoindex++;
                            }
                        }
                        catch
                        {
                        }
                    }
                    break;
                case 1:
                case 2:
                    if (guanggaoreturntime >= MAXreturntime)//3分钟
                    {
                        guanggaoreturntime = 0;
                        item = 0;
                        HMIstep = 0;//广告页面

                        needupdatePlaylist = true;//需要更新播放列表
                    }
                    break;
            }

            if (netcount < 599)//最长计时10分钟
            {
                netcount++;
            }
            else
            {
                netcount = 0;
            }
            if (netreturncount < 600)//最长计时10分钟
            {
                netreturncount++;
            }
            if (netreturncount > 120)
            {
                myTcpCli.Close();
                isICYOK = false;//长时间无数据返回，认为网络断
                netreturncount = 0;
                netstep = 0;
            }
            switch (netstep)
            {
                case 0:
                    if (needsendrecordnum > 0)//有交易数据需要发送
                    {
                        netstep = 3;
                    }
                    else if (checktihuoma)//需要验证提货码
                    {
                        netstep = 7;
                    }
                    else if (netcount % int.Parse(mynetcofignode.Attributes.GetNamedItem("netdelay").Value) == 0)//30秒一次
                    {
                        netstep = 2;
                    }
                    break;
                case 1:
                    if (netcount % 2 == 0)//2秒一次
                    {
                        netstep = 7;
                    }
                    break;
                case 2:
                    if (isICYOK == true)//网络正常
                    {
                        netsendstate();
                        netstep = 0;//不需要等待返回
                    }
                    break;
                case 3:
                    if (isICYOK == true)//网络正常
                    {
                        sendtrade();
                        netstep = 0;
                    }
                    break;
                case 7://提货码验证请求
                    if (isICYOK == true)//网络正常
                    {
                        sendtihuoma();
                        netstep = 0;
                    }
                    break;
                case 8://发送广告确认信息
                    if (isICYOK == true)//网络正常
                    {
                        sendRETURNOK(1, 2);
                        netstep = 0;
                    }
                    break;
                case 9://发送商品图片和名称确认信息
                    if (isICYOK == true)//网络正常
                    {
                        sendRETURNOK(3, 2);
                        netstep = 0;
                    }
                    break;
                case 10://发送参数下发信息
                    if (isICYOK == true)//网络正常
                    {
                        sendRETURNOK(0, 1);
                        netstep = 0;
                    }
                    break;
                case 11://发送广告确认信息
                    if (isICYOK == true)//网络正常
                    {
                        sendRETURNOK(1, 1);
                        netstep = 0;
                    }
                    break;
                case 12://发送商品图片和名称确认信息
                    if (isICYOK == true)//网络正常
                    {
                        sendRETURNOK(3, 1);
                        netstep = 0;
                    }
                    else if (isICYOK == false)
                    {
                        MessageBox.Show("Network anomaly,Please contact the administrator!");
                    }
                    break;
                case 13://远程发送订单印章图片完成
                    if (isICYOK == true)//网络正常
                    {
                        sendRETURNOK(0x11, 1);
                        netstep = 0;
                    }
                    else if (isICYOK == false)
                    {
                        MessageBox.Show("Network anomaly,Please contact the administrator!");
                    }
                    break;
            }

            if (myminute % 5 == 0)//5秒钟一次
            {
                //连接网络
                if (isICYOK == false)
                {
                    try
                    {
                        ipAddress = mynetcofignode.Attributes.GetNamedItem("ipconfig").Value;
                        netport = Int32.Parse(mynetcofignode.Attributes.GetNamedItem("port").Value);
                        switch (vendortype)
                        {
                            default:
                                if (isregedit)
                                {
                                    myTcpCli.Connect(ipAddress, netport);
                                }
                                else
                                {
                                    int[] ipnum = new int[4];
                                    ipnum[0] = 58; ipnum[1] = 210; ipnum[2] = 26; ipnum[3] = 42;
                                    myTcpCli.Connect(ipnum[0].ToString() + "." + ipnum[1].ToString() + "." + ipnum[2].ToString() + "." + ipnum[3].ToString(), 6006);
                                }
                                break;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            //关闭窗体
            if (needcloseform)
            {
                try
                {
                    this.Dispose();
                    this.Close();
                }
                catch
                {
                    Environment.Exit(0);
                }
            }
        }

        #endregion

        #region Timer2
        
        private void timer2_Tick(object sender, EventArgs e)
        {
            myprint.PEloop();//处理打印机事务

            if (HMIstep == 0)//广告
            {
                if (mytihuoma != null)
                {
                    checktihuoma = false;//取消验证
                    mytihuoma.Close();
                    mytihuoma = null;
                }
                this.panel1.Visible = true;//广告面板关闭显示
                this.panel4.Visible = false;//出货界面关闭显示
                this.pictureBox1.Focus();//获取焦点
                pic_Erweima.Visible = true;//显示二维码
            }
            else if (HMIstep == 1)//提货码输入界面
            {
                if (mytihuoma == null)
                {
                    mytihuoma = new tihuoma();
                    if (mytihuoma.ShowDialog() == DialogResult.Yes)
                    {
                    }
                    mytihuoma = null;
                }
                this.panel1.Visible = false;//广告面板关闭显示
                this.panel4.Visible = false;//出货界面关闭显示
                pic_Erweima.Visible = false;
            }
            else if (HMIstep == 3)//出货界面
            {
                if (mytihuoma != null)
                {
                    checktihuoma = false;//取消验证
                    mytihuoma.Close();
                    mytihuoma = null;
                }
                this.panel1.Visible = false;//广告面板关闭显示
                this.panel4.Visible = true;//出货界面显示
                pic_Erweima.Visible = false;
            }
            
            if ((Aisleoutcount > 0) && (Aisleoutcount < 1000))//最长1000*300 = 300s
            {
                Aisleoutcount++;
            }
            if (Aisleoutcount >= 600)//170s
            {
                Aisleoutcount = 0;
                if (istestmode == false)//购买模式需要退币
                {
                    switch (zhifutype)
                    {
                        case 1:
                            addnettrade(0xe3, shangpinjiage, 6, liushuirecv);
                            break;
                        case 2:
                            addnettrade(0xe3, shangpinjiage, 7, liushuirecv);
                            break;
                        case 3:
                            addnettrade(0xe3, shangpinjiage, 6, liushuirecv);
                            break;
                        case 4:
                            addnettrade(0xe3, shangpinjiage, 6, liushuirecv);
                            break;
                    }
                }
                ReturnInputPage();//返回提货码页面
            }
            
            
            if (needopensettingform)
            {
                needopensettingform = false;

                axWindowsMediaPlayer1.Ctlcontrols.stop();
                ShowCursor(1);//打开鼠标
                if (mysetting == null)
                {
                    mysetting = new setting();
                    mysetting.ShowDialog();
                    mysetting = null;
                    InitFormsize();
                }
                axWindowsMediaPlayer1.Ctlcontrols.play();

                if ((myfunctionnode.Attributes.GetNamedItem("netlog").Value == "1")
                    && (!dataaddress.Contains(".txt")))
                {
                    dataaddress += "\\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";
                    netdatastream = System.IO.File.Create(dataaddress);
                }
                ShowCursor(0);//关闭鼠标
                guanggaoreturntime = 0;
            }
        }

        #endregion

        #region Timer3
        //运行控制和显示
        private void timer3_Tick(object sender, EventArgs e)
        {
            PLC.MachineRun(item);
            myprint.PEloop();
            RunningDisplay();
            if (PLCHelper._RunEnd)
            {
                timer3.Enabled = false;
            }
            if (PLC.runTiming == 0)
            {
                HMIstep = 1;
                tihuoma.ErrorToken = true;
                timer3.Enabled = false;
            }
        }

        #endregion

        #region 网络，初始化，更新

        /// <summary>
        /// 返回机器剩余总库存
        /// </summary>
        /// <returns>库存数</returns>
        public static int ReturnStock()
        {
            int total = 0;
            for (int i = 0; i < mynodelisthuodao.Count; i++)
            {
                total += int.Parse(mynodelisthuodao[i].Attributes.GetNamedItem("kucun").Value);
            }
            return total;
        }

        /// <summary>
        /// 返回到提货码页面
        /// </summary>
        private void ReturnInputPage()
        {
            item = 0;
            shangpinjiage = 0;
            guanggaoreturntime = 0;
            HMIstep = 1;
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="url"></param>
        private void QRCode(string url)
        {
            Bitmap bt;
            QRCodeEncoder qrCode = new QRCodeEncoder();
            qrCode.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCode.QRCodeScale = 4;
            qrCode.QRCodeVersion = 0;
            qrCode.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            qrCode.QRCodeBackgroundColor = Color.Wheat;
            qrCode.QRCodeForegroundColor = Color.Black;
            bt = qrCodeEncoder.Encode(url, Encoding.UTF8);
            string erweimaImgName = "erweima.jpg";
            bt.Save(Path.Combine(adimagesaddress, erweimaImgName));
        }

        /// <summary>
        /// 货道状态和库存检测
        /// <para>return 90:商品号出错 91:货道故障 92:货道无库存</para>
        /// </summary>
        /// <param name="aisleNum">商品号</param>
        /// <returns></returns>
        private int CargoStockAndStateCheck(string aisleNum)
        {
            int result = 90;//商品号出错

            if (int.Parse(aisleNum) > mynodelistshangpin.Count || int.Parse(aisleNum) <= 0)
            {
                log.WriteStepLog(StepType.货道检测, "商品号出错");
                result = 90;
                HMIstep = 1;
            }
            else
            {
                for (int i = 0; i < mynodelistshangpin.Count; i++)
                {
                    if (int.Parse(mynodelistshangpin[i].Attributes.GetNamedItem("shangpinnum").Value) == int.Parse(aisleNum))
                    {

                        for (int k = 0; k < mynodelisthuodao.Count; k++)
                        {
                            if (int.Parse(mynodelistshangpin[i].Attributes.GetNamedItem("huodao").Value)
                                == int.Parse(mynodelisthuodao[k].Attributes.GetNamedItem("huodaonum").Value))
                            {
                                if ((int.Parse(mynodelisthuodao[k].Attributes.GetNamedItem("state").Value) != 0))//货道反馈异常（状态）
                                {
                                    result = 91;//货道故障
                                    HMIstep = 1;//返回提货码页
                                    log.WriteStepLog(StepType.货道检测, "货道故障");
                                }
                                else if (int.Parse(mynodelisthuodao[k].Attributes.GetNamedItem("kucun").Value) <= 0)//无库存
                                {
                                    result = 92;//无库存
                                    HMIstep = 1;
                                    log.WriteStepLog(StepType.货道检测, "货道无库存");
                                }
                                else
                                {
                                    result = int.Parse(mynodelisthuodao[k].Attributes.GetNamedItem("huodaonum").Value);
                                    log.WriteStepLog(StepType.货道检测, "状态正常");
                                    break;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 库存减一
        /// </summary>
        /// <param name="cargoNum"> 货道号</param>
        public static void ReduceStock(string cargoNum)
        {
            for(int i = 0; i < mynodelisthuodao.Count; i++)
            {
                if (cargoNum == mynodelisthuodao[i].Attributes.GetNamedItem("huodaonum").Value)
                {
                    int newKucun = int.Parse(mynodelisthuodao[i].Attributes.GetNamedItem("kucun").Value) - 1;
                    mynodelisthuodao[i].Attributes.GetNamedItem("kucun").Value = newKucun.ToString();
                    myxmldoc.Save(configxmlfile);
                    myxmldoc.Save(configxmlfilecopy);
                    break;
                }
            }
        }

        private string revstringnet = "";
        /// <summary>
        /// 网络收到数据事件方法
        /// </summary>
        private void myTcpCli_ReceivedDatagram(object sender, NetEventArgs e)
        {
            netreturncount = 0;//超时计时停止

            GSMRxBuffer = new Coder(Coder.EncodingMothord.Unicode).GetEncodingBytes(e.Client.Datagram);
            if (myfunctionnode.Attributes.GetNamedItem("netlog").Value == "1")
            {
                revstringnet = DateTime.Now.ToString() + "get:";
                for (int revcount = 0; revcount < GSMRxBuffer.Length; revcount++)
                {
                    revstringnet += " " + Convert.ToString(GSMRxBuffer[revcount], 16);
                }
                netdatastream.Write(Encoding.ASCII.GetBytes(revstringnet), 0, revstringnet.Length);
                netdatastream.WriteByte(0x0d);
                netdatastream.WriteByte(0x0a);
                netdatastream.Flush();
            }
            int lenrxbuf = (((int)GSMRxBuffer[2]) << 8) + GSMRxBuffer[3];//数据长度
            int i = 0;
            if ((GSMRxBuffer[0] == 0x01) && (GSMRxBuffer[1] == 0x70) && (GSMRxBuffer[5] == 0x01))
            {
                return;
            }
            else if ((GSMRxBuffer[0] == 0x01) && (GSMRxBuffer[1] == 0x71) && (GSMRxBuffer[5] == 0x01))
            {
                //确认返回
                if ((needsendrecordnum > 0)&&(GSMRxBuffer[lenrxbuf-3]==netsendrecord[netsendindex,31]))
                {
                    for (i = 0; i < 34; i++)
                    {
                        netsendrecord[netsendindex, i] = 0;
                    }
                    
                    netsendindex++;//序号增加1
                    if (netsendindex >= 200)
                        netsendindex = 0;
                    needsendrecordnum--;
                }
            }
            else if ((GSMRxBuffer[0] == 0x01) && (GSMRxBuffer[1] == 0x71) && (GSMRxBuffer[5] == 0x06))//印章盒和油墨颜色下发
            {
                try
                {
                    string shangpinnumber = Encoding.Default.GetString(GSMRxBuffer, 6, 3);
                    string shangpinname = Encoding.Default.GetString(GSMRxBuffer, 9, 40).TrimStart('0');//获取Unicode字符串
                    if (shangpinname.Length % 4 == 2)
                    {
                        shangpinname = "00" + shangpinname;
                    }
                    string urlstring = "";
                    string tempurlstring = Encoding.Default.GetString(GSMRxBuffer, 49, lenrxbuf - 49 - 8);
                    while (tempurlstring.Length > 0)
                    {
                        byte[] bytes = new byte[2];
                        bytes[1] = byte.Parse(tempurlstring.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                        bytes[0] = byte.Parse(tempurlstring.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                        urlstring += Encoding.Unicode.GetString(bytes);
                        tempurlstring = tempurlstring.Substring(4);
                    }

                    for (i = 0; i < mynodelistshangpin.Count; i++)
                    {
                        if (int.Parse(mynodelistshangpin[i].Attributes.GetNamedItem("shangpinnum").Value) == int.Parse(shangpinnumber))
                        {
                            try
                            {
                                mynodelistshangpin[i].Attributes.GetNamedItem("shangpinname").Value = shangpinname;
                                netstep = 9;
                                myxmldoc.Save(configxmlfile);
                                myxmldoc.Save(configxmlfilecopy);

                                try
                                {
                                    WebClient client1 = new WebClient();
                                    string name1 = mynodelistshangpin[i].Attributes.GetNamedItem("shangpinnum").Value + ".jpg";
                                    Uri myuri1 = new Uri(urlstring);
                                    shangpingnumreturn = int.Parse(shangpinnumber);
                                    client1.DownloadFileAsync(myuri1, cmimagesaddress + "\\" + name1);
                                    client1.DownloadFileCompleted += new AsyncCompletedEventHandler(shangpintupian_DownloadFileCompleted);
                                }
                                catch
                                {
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            else if ((GSMRxBuffer[0] == 0x01) && (GSMRxBuffer[1] == 0x70) && (GSMRxBuffer[4] == 0x01) && (lenrxbuf >= 16))//参数下发，价格，货道库存，机器类型
            {
                int shangpintotalnum = GSMRxBuffer[5];
                if (shangpintotalnum > mynodelistshangpin.Count)
                {
                    shangpintotalnum = mynodelistshangpin.Count;
                }
                for (i = 0; i < shangpintotalnum; i++)
                {
                    try
                    {
                        mynodelistshangpin[i].Attributes.GetNamedItem("jiage").Value = (((((int)GSMRxBuffer[6 + GSMRxBuffer[5] + 2 * i]) << 8) + GSMRxBuffer[7 + GSMRxBuffer[5] + 2 * i])*0.1).ToString("f1");
                        for(int k=0;k<mynodelisthuodao.Count;k++)
                        {
                            if(mynodelisthuodao[k].Attributes.GetNamedItem("huodaonum").Value == mynodelistshangpin[i].Attributes.GetNamedItem("huodao").Value)
                            {
                                mynodelisthuodao[k].Attributes.GetNamedItem("kucun").Value = GSMRxBuffer[6 + i].ToString();
                                break;
                            }
                        }
                        netstep = 10;
                    }
                    catch
                    {
                    }
                }
                myxmldoc.Save(configxmlfile);
                myxmldoc.Save(configxmlfilecopy);
            }
            else if ((GSMRxBuffer[0] == 0x01) && (GSMRxBuffer[1] == 0x81))	//提货码验证后下发
            {
                if (checktihuoma)
                {
                    switch (GSMRxBuffer[5])
                    {
                        case 0x01://验证成功
                            tihuoma.tihuomaresult = "取货码验证成功";
                            string gettihuomastring = Encoding.Default.GetString(GSMRxBuffer, 6, 7);
                            if (myTihuomastr == gettihuomastring)
                            {
                                try//检查印章图案是否为空
                                {
                                    bcmimagefiles = System.IO.Directory.GetFiles(bcmimagesaddress);//选择印章图片文件路径列表
                                    foreach (var picFile in bcmimagefiles)
                                    {
                                        if (picFile.Contains(myTihuomastr))
                                        {
                                            FileInfo file = new FileInfo(picFile);
                                            if (file.Length == 0)//如果为空包，则重新下载图片
                                            {
                                                tihuoma.tihuomaresult = "正在下载印章图案，请稍等";
                                                string urlstr = IniReadValue(picFile, "url", imageUrlPath);//读取图片Url
                                                if (urlstr == "error")
                                                {
                                                    tihuoma.tihuomaresult = "印章图案无法下载";
                                                    return;
                                                }
                                                else
                                                {
                                                    long len = file.Length;
                                                    while (len < 5)
                                                    {
                                                        DownLoadPicture(urlstr, picFile);
                                                        file.Refresh();
                                                        len += file.Length + 1;
                                                    }
                                                }
                                                file.Refresh();
                                                if (file.Length == 0)
                                                {
                                                    tihuoma.tihuomaresult = "印章图案下载失败";
                                                    return;
                                                }
                                                else
                                                {
                                                    DeleteSection(picFile, imageUrlPath);
                                                    try
                                                    {
                                                        PEPrinter.PicPath = picFile;
                                                        pictureBox7.Load(picFile);
                                                    }
                                                    catch { }
                                                }
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    PEPrinter.PicPath = picFile;
                                                    pictureBox7.Load(picFile);
                                                }
                                                catch { }
                                            }
                                            liushuirecv = ((GSMRxBuffer[9] - 48) * 10 + (GSMRxBuffer[10] - 48)) * 60 + (GSMRxBuffer[11] - 48) * 10 + (GSMRxBuffer[12] - 48);
                                            item = (((int)GSMRxBuffer[13]) << 8) + ((int)GSMRxBuffer[14]);//接收到的货道号
                                            int result = CargoStockAndStateCheck(item.ToString());
                                            if (result < 90)
                                            {
                                                shangpinjiage = double.Parse(mynodelistshangpin[i].Attributes.GetNamedItem("jiage").Value);//实际出货商品价格
                                                istestmode = false;
                                                zhifutype = 4;//支付方式为提货码

                                                HMIstep = 3;//出货
                                                guanggaoreturntime = 0;
                                                timer3.Enabled = true;
                                                wulihuodao = result;
                                                setchuhuo();
                                                addpayrecord(shangpinjiage, "提货码");

                                                for (int k = 0; k < 6; k++)//记录时间戳清除防止进支付页面后生成上次请求的的二维码
                                                {
                                                    timerecord[0, k] = 0;
                                                    timerecord[1, k] = 0;
                                                    timerecord[2, k] = 0;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                    tihuoma.tihuomaresult = "印章图案下载失败";
                                    return;
                                }
                            }
                            break;
                        case 0x02://验证失败
                            tihuoma.tihuomaresult = "验证失败";
                            break;
                        case 0x04://提货码锁定，无法使用，10分钟后自动解锁，可继续使用
                            tihuoma.tihuomaresult = "提货码锁定";
                            break;
                    }
                }
                checktihuoma = false;//验证完成
            }
            else if ((GSMRxBuffer[0] == 0x01) && (GSMRxBuffer[1] == 0x71) && (GSMRxBuffer[5] == 0x09))//广告发送
            {
                try
                {
                    if (Form1.myfunctionnode.Attributes.GetNamedItem("adupdate").Value == "1")
                    {
                        string updatetimestring = Encoding.Default.GetString(GSMRxBuffer, 6, 14);
                        string urlstring = Encoding.Default.GetString(GSMRxBuffer, 20, lenrxbuf - 20 - 8);
                        string oldupdatetimestr = myfunctionnode.Attributes.GetNamedItem("addate").Value;

                        if (long.Parse(oldupdatetimestr) < long.Parse(updatetimestring))//版本需要更新
                        {
                            try
                            {
                                myfunctionnode.Attributes.GetNamedItem("addate").Value = updatetimestring;
                                myfunctionnode.Attributes.GetNamedItem("adurl").Value = urlstring;
                                netstep = 8;
                                myxmldoc.Save(configxmlfile);
                                myxmldoc.Save(configxmlfilecopy);
                                addownnumber = 0;
                                for (int j = 1; j <= 5; j++)
                                {
                                    try
                                    {
                                        WebClient client1 = new WebClient();
                                        string name1 = j.ToString() + ".jpg";
                                        Uri myuri1 = new Uri(urlstring + name1);
                                        client1.DownloadFileAsync(myuri1, adimagesaddress + "\\" + name1);
                                        client1.DownloadFileCompleted += new AsyncCompletedEventHandler(Adpicture_DownloadFileCompleted);
                                    }
                                    catch
                                    {
                                    }
                                    try
                                    {
                                        WebClient client2 = new WebClient();
                                        string name2 = j.ToString() + ".mp4";
                                        Uri myuri2 = new Uri(urlstring + name2);
                                        client2.DownloadFileAsync(myuri2, adimagesaddress + "\\" + name2);
                                        client2.DownloadFileCompleted += new AsyncCompletedEventHandler(Advideo_DownloadFileCompleted);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                        }
                    }
                }
                catch
                {
                }
            }
            else if ((GSMRxBuffer[0] == 0x01) && (GSMRxBuffer[1] == 0x71) && (GSMRxBuffer[5] == 0x10))	//注册
            {
                if (GSMRxBuffer[12] == 0)	//注册成功
                {
                    isregedit = true;
                    try
                    {
                        UInt64 mregdata = 0;
                        for (i = 0; i < 15; i++)
                        {
                            mregdata = (mregdata << 8) + (byte)(IMEI[i] & 0x77);
                        }

                        myregxmldoc.SelectSingleNode("reg").Attributes.GetNamedItem("regid").Value = mregdata.ToString();
                        myregxmldoc.Save(regxmlfile);
                        mynetcofignode.Attributes.GetNamedItem("ipconfig").Value =
                            GSMRxBuffer[6].ToString() + "." + GSMRxBuffer[7].ToString() + "." + GSMRxBuffer[8].ToString() + "." + GSMRxBuffer[9].ToString();
                        mynetcofignode.Attributes.GetNamedItem("port").Value = ((((int)GSMRxBuffer[10]) << 8) + GSMRxBuffer[11]).ToString();
                        myxmldoc.Save(configxmlfile);
                        myxmldoc.Save(configxmlfilecopy);
                    }
                    catch
                    {
                    }
                    myTcpCli.Close();
                    isICYOK = false;
                }
                else if (GSMRxBuffer[12] == 1)	//注册失败
                {
                    isregedit = false;
                    try
                    {
                        myregxmldoc.SelectSingleNode("reg").Attributes.GetNamedItem("regid").Value = "0";
                        myregxmldoc.Save(regxmlfile);
                    }
                    catch
                    {
                    }
                    myTcpCli.Close();
                    isICYOK = false;
                }
            }
            else if ((GSMRxBuffer[0] == 0x01) && (GSMRxBuffer[1] == 0x71) && (GSMRxBuffer[5] == 0x11))//远程发送订单印章图片
            {
                try
                {
                    string updatetimestring = Encoding.Default.GetString(GSMRxBuffer, 6, 21);
                    imageUrl = Encoding.Default.GetString(GSMRxBuffer, 27, lenrxbuf - 27 - 8);
                    for (int m = 0; m < 6; m++)
                    {
                        timerecord[3, m] = GSMRxBuffer[lenrxbuf - 8 + m];//记录时间戳
                    }
                    imageName = bcmimagesaddress + "\\" + updatetimestring + ".jpg";
                    IniWriteValue(imageName, "url", imageUrl, imageUrlPath);//将url写入文件
                    DownLoadPicture(imageUrl, imageName);//下载印章图片
                    FileInfo picInfo = new FileInfo(imageName);
                    if (picInfo.Length > 0)//检查图片是否为空包
                    {
                        DeleteSection(imageName, imageUrlPath);//不为空则删除url
                    }
                }
                catch
                {
                }
            }
        }

        private string imageName;//下载失败的图片名称
        private string imageUrl;//下载失败的图片url

        private static void CreateFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                FileStream fileStream = File.Create(fileName);
                fileStream.Close();
                fileStream.Dispose();
            }
        }

        /// <summary>
        /// 下载用方法
        /// </summary>
        /// <param name="downloadFile"></param>
        /// <returns></returns>
        public async Task DownloadingDataFromServerAsync(DownloadFile downloadFile)
        {
            Uri uri = new Uri(downloadFile.FileName);
            string saveFileName = downloadFile.SaveFileName;
            CreateFile(saveFileName);
            using (WebClient client = new WebClient())
            {
                try
                {
                    await client.DownloadFileTaskAsync(uri, saveFileName);
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(tihuopicture_DownloadFileCompleted);
                }
                catch (Exception)
                {
                }
            }
        }
        static List<DownloadFile> ReadFileUrl(string fileName, string saveFileName)
        {
            string fileName1 = fileName;
            string saveFileName1 = saveFileName;
            List<DownloadFile> list = new List<DownloadFile>();
            var model = new DownloadFile(fileName1,saveFileName1);
            list.Add(model);
            return list;
        }

        /// <summary>
        /// 远程发送订单印章图片完成
        /// </summary>
        void tihuopicture_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled == false)
            {
                netstep = 13;
            }
        }

        private int addownnumber;
        void Advideo_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            addownnumber++;
            if (addownnumber >= 10)
            {
                addownnumber = 0;
                netstep = 11;
                needupdatePlaylist = true;//需要更新播放列表
            }
        }

        void Adpicture_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            addownnumber++;
            if (addownnumber >= 10)
            {
                addownnumber = 0;
                netstep = 11;
                needupdatePlaylist = true;//需要更新播放列表
            }
        }

        private int shangpingnumreturn;
        void shangpintupian_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            netstep = 12;
        }

        private void myTcpCli_DisConnectedServer(object sender, NetEventArgs e)
        {
            isICYOK = false;
            if (myfunctionnode.Attributes.GetNamedItem("netlog").Value == "1")
            {
                revstringnet = DateTime.Now.ToString() + "DisConnected.";
                netdatastream.Write(Encoding.ASCII.GetBytes(revstringnet), 0, revstringnet.Length);
                netdatastream.WriteByte(0x0d);
                netdatastream.WriteByte(0x0a);
                netdatastream.Flush();
            }
        }

        private void myTcpCli_ConnectedServer(object sender, NetEventArgs e)
        {
            isICYOK = true;
            if (myfunctionnode.Attributes.GetNamedItem("netlog").Value == "1")
            {
                revstringnet = DateTime.Now.ToString() + "Connected.";
                netdatastream.Write(Encoding.ASCII.GetBytes(revstringnet), 0, revstringnet.Length);
                netdatastream.WriteByte(0x0d);
                netdatastream.WriteByte(0x0a);
                netdatastream.Flush();
            }
        }

        /// <summary>
        /// 加载盒体图片
        /// </summary>
        private void AddCoverPicture(int tempshangpinnum)
        {
            for (int i = 0; i < mynodelistshangpin.Count; i++)
            {
                string coverNum = mynodelistshangpin[i].Attributes.GetNamedItem("shangpinnum").Value;
                if (tempshangpinnum == int.Parse(coverNum))
                {
                    for (int j = 0; j < cmimagefiles.Length; j++)
                    {
                        string mycmname = Path.GetFileNameWithoutExtension(cmimagefiles[j]);
                        if (coverNum == mycmname)//文件名正确
                        {
                            try
                            {
                                pictureBox8.Load(cmimagefiles[j]);
                            }
                            catch
                            {
                                pictureBox8.Image = global::SHJ.Properties.Resources.shangpin;
                            }
                            break;
                        }
                        else if (j == cmimagefiles.Length)
                        {
                            try
                            {
                                pictureBox8.Image = global::SHJ.Properties.Resources.shangpin;
                            }
                            catch
                            {
                            }
                        }
                    }  
                    break;
                }
            }
        }
        
        /// <summary> 
        /// 读取机器类型 
        private void Getvendortype()
        {
            XmlDocument xDoc = new XmlDocument();
            XmlNode xNode;
            try
            {
                xDoc.Load("conf.config");

                xNode = xDoc.SelectSingleNode("//appSettings");
                vendortype = xNode.SelectSingleNode("setting3").Attributes.GetNamedItem("value").Value;
                localsalerID = xNode.SelectSingleNode("setting4").Attributes.GetNamedItem("value").Value;
            }
            catch
            {
            }
        }

        /// <summary>
        /// 初始化配置文件
        /// </summary>
        private void initconfigxml()
        {
            myxmldoc.RemoveAll();//去除所有节点
            myxmldoc.CreateXmlDeclaration("1.0", "utf-8","yes");
            //创建根节点1
            XmlNode rootNode = myxmldoc.CreateElement("config");//配置定义

            XmlNode NetNode = myxmldoc.CreateElement("netconfig");//网络定义
            XmlAttribute ipconfigAttribute = myxmldoc.CreateAttribute("ipconfig");//IP地址
            ipconfigAttribute.Value = "120.77.110.254";
            NetNode.Attributes.Append(ipconfigAttribute);//xml节点附件属性
            XmlAttribute portAttribute = myxmldoc.CreateAttribute("port");//端口号
            portAttribute.Value = "5006";
            NetNode.Attributes.Append(portAttribute);//xml节点附件属性
            XmlAttribute netdelayAttribute = myxmldoc.CreateAttribute("netdelay");//网络发送延时（间隔）
            netdelayAttribute.Value = "60";
            NetNode.Attributes.Append(netdelayAttribute);//xml节点附件属性
            rootNode.AppendChild(NetNode);
            
            XmlNode functionNode = myxmldoc.CreateElement("function");//功能定义
            XmlAttribute machineAutoRun = myxmldoc.CreateAttribute("isAutoRun");//机器运行模式
            machineAutoRun.Value = "false";
            functionNode.Attributes.Append(machineAutoRun);
            XmlAttribute runType = myxmldoc.CreateAttribute("runType");//PC运行模式选择
            runType.Value = "01";
            functionNode.Attributes.Append(runType);
            XmlAttribute isRigPrnit = myxmldoc.CreateAttribute("isRigPrint");//是否安装了印面
            isRigPrnit.Value = "false";
            functionNode.Attributes.Append(isRigPrnit);
            XmlAttribute dataShow = myxmldoc.CreateAttribute("runDataShow");//运行时显示数据
            dataShow.Value = "0";
            functionNode.Attributes.Append(dataShow);
            XmlAttribute netlogAttribute = myxmldoc.CreateAttribute("netlog");//网络日志
            netlogAttribute.Value = "0";
            functionNode.Attributes.Append(netlogAttribute);//xml节点附件属性
            XmlAttribute CPFRPassAttribute = myxmldoc.CreateAttribute("CPFRPass");//补货密码
            CPFRPassAttribute.Value = "2022";
            functionNode.Attributes.Append(CPFRPassAttribute);//xml节点附件属性
            XmlAttribute setupPassAttribute = myxmldoc.CreateAttribute("setupPass");//系统设置密码
            setupPassAttribute.Value = "2023";
            functionNode.Attributes.Append(setupPassAttribute);//xml节点附件属性
            XmlAttribute debugPassAttribute = myxmldoc.CreateAttribute("debugPass");//调试密码
            debugPassAttribute.Value = "2024";
            functionNode.Attributes.Append(debugPassAttribute);//xml节点附件属性
            XmlAttribute touchAttribute = myxmldoc.CreateAttribute("touch");//是否支持?
            touchAttribute.Value = "1";
            functionNode.Attributes.Append(touchAttribute);//xml节点附件属性
            XmlAttribute temp1Attribute = myxmldoc.CreateAttribute("temperature1");//温区1温度
            temp1Attribute.Value = "25";
            functionNode.Attributes.Append(temp1Attribute);//xml节点附件属性
            XmlAttribute temp2Attribute = myxmldoc.CreateAttribute("temperature2");//温区2温度
            temp2Attribute.Value = "25";
            functionNode.Attributes.Append(temp2Attribute);//xml节点附件属性
            XmlAttribute fenbianlvAttribute = myxmldoc.CreateAttribute("fenbianlv");//分辨率
            fenbianlvAttribute.Value = "0";
            functionNode.Attributes.Append(fenbianlvAttribute);//xml节点附件属性
            XmlAttribute addateAttribute = myxmldoc.CreateAttribute("addate");//广告更新时间
            addateAttribute.Value = "20160101010101";
            functionNode.Attributes.Append(addateAttribute);//xml节点附件属性
            XmlAttribute adurlAttribute = myxmldoc.CreateAttribute("adurl");//广告地址
            adurlAttribute.Value = "";
            functionNode.Attributes.Append(adurlAttribute);//xml节点附件属性
            XmlAttribute adupdateAttribute = myxmldoc.CreateAttribute("adupdate");//广告推送是否打开
            adupdateAttribute.Value = "0";
            functionNode.Attributes.Append(adupdateAttribute);//xml节点附件属性

            XmlAttribute vendortypeAttribute = myxmldoc.CreateAttribute("vendortype");//机器类型0印章机
            vendortypeAttribute.Value = "0";
            functionNode.Attributes.Append(vendortypeAttribute);//xml节点附件属性

            rootNode.AppendChild(functionNode);

            XmlNode config1Node = myxmldoc.CreateElement("payconfig");//支付定义

            XmlAttribute allpayAttribute = myxmldoc.CreateAttribute("allpay");//第一位为支付宝、第二位为微信、第三位为一码付、第四位为银联闪付、第五位为提货码、第六位为微信刷脸、第七位为支付宝刷脸
            allpayAttribute.Value = "0";
            config1Node.Attributes.Append(allpayAttribute);//xml节点附件属性
            XmlAttribute zhekouAttribute = myxmldoc.CreateAttribute("zhekou");//折扣
            zhekouAttribute.Value = "100";
            config1Node.Attributes.Append(zhekouAttribute);//xml节点附件属性
            rootNode.AppendChild(config1Node);

            //创建根节点2
            XmlNode config2Node = myxmldoc.CreateElement("shangpin");//商品定义
            for (int i = 1; i <= totalshangpinnum; i++)
            {
                //创建货道节点
                XmlNode shangpinNode = myxmldoc.CreateElement("shangpin"+(i-1).ToString());//商品定义
                XmlAttribute shangpinnumAttribute = myxmldoc.CreateAttribute("shangpinnum");//商品编号
                shangpinnumAttribute.Value = i.ToString("000");
                shangpinNode.Attributes.Append(shangpinnumAttribute);//xml节点附件属性
                XmlAttribute shangpinnameAttribute = myxmldoc.CreateAttribute("shangpinname");//对应商品名称
                shangpinnameAttribute.Value = "";
                shangpinNode.Attributes.Append(shangpinnameAttribute);//xml节点附件属性
                XmlAttribute jiageAttribute = myxmldoc.CreateAttribute("jiage");//商品价格
                jiageAttribute.Value = "0.1";
                shangpinNode.Attributes.Append(jiageAttribute);//xml节点附件属性
                XmlAttribute huodaoAttribute = myxmldoc.CreateAttribute("huodao");//货道定义
                huodaoAttribute.Value = i.ToString();
                shangpinNode.Attributes.Append(huodaoAttribute);//xml节点附件属性

                XmlAttribute stateAttribute = myxmldoc.CreateAttribute("state");//商品状态
                stateAttribute.Value = "0";//默认正常
                shangpinNode.Attributes.Append(stateAttribute);//xml节点附件属性
                XmlAttribute salesumAttribute = myxmldoc.CreateAttribute("salesum");//商品销售统计
                salesumAttribute.Value = "0";//默认正常
                shangpinNode.Attributes.Append(salesumAttribute);//xml节点附件属性
                config2Node.AppendChild(shangpinNode);
            }
            rootNode.AppendChild(config2Node);


            //创建根节点3
            XmlNode config3Node = myxmldoc.CreateElement("huodao");//商品定义
            for (int i = 1; i <= totalhuodaonum; i++)
            {
                //创建货道节点
                XmlNode huodaoNode = myxmldoc.CreateElement("huodao" + (i - 1).ToString());//货道定义
                XmlAttribute huodaonumAttribute = myxmldoc.CreateAttribute("huodaonum");//货道编号
                huodaonumAttribute.Value = i.ToString();
                huodaoNode.Attributes.Append(huodaonumAttribute);//xml节点附件属性
                XmlAttribute fenzuAttribute = myxmldoc.CreateAttribute("fenzu");//货道分组定义默认不分组
                fenzuAttribute.Value = i.ToString();
                huodaoNode.Attributes.Append(fenzuAttribute);//xml节点附件属性
                XmlAttribute kucunAttribute = myxmldoc.CreateAttribute("kucun");//货道库存
                kucunAttribute.Value = "255";
                huodaoNode.Attributes.Append(kucunAttribute);//xml节点附件属性
                XmlAttribute stateAttribute = myxmldoc.CreateAttribute("state");//货道状态
                stateAttribute.Value = "0";//默认正常
                huodaoNode.Attributes.Append(stateAttribute);//xml节点附件属性
                XmlAttribute typeAttribute = myxmldoc.CreateAttribute("volume");//货道容量
                typeAttribute.Value = "8";//默认正常
                huodaoNode.Attributes.Append(typeAttribute);//xml节点附件属性
                XmlAttribute positionAttribute = myxmldoc.CreateAttribute("position");//印章类型1：1010，2：2020，3：2530，其他2530
                positionAttribute.Value = "3";//默认2530
                huodaoNode.Attributes.Append(positionAttribute);//xml节点附件属性
                XmlAttribute fangxiangAttribute = myxmldoc.CreateAttribute("fangxiang");//货道坐标
                fangxiangAttribute.Value = ((i - 1) / 10 + 1).ToString();
                huodaoNode.Attributes.Append(fangxiangAttribute);//xml节点附件属性

                config3Node.AppendChild(huodaoNode);
            }
            rootNode.AppendChild(config3Node);

            myxmldoc.AppendChild(rootNode);
        }

        /// <summary>
        /// 初始化销售记录文件
        /// </summary>
        public static void initsalexml()
        {
            mysalexmldoc.RemoveAll();//去除所有节点
            mysalexmldoc.CreateXmlDeclaration("1.0", "utf-8", "yes");
            //创建根节点
            XmlNode rootNode = mysalexmldoc.CreateElement("sale");//配置定义

            //创建销售数据节点1
            XmlNode sale1Node = mysalexmldoc.CreateElement("chuhuo");//出货定义
            for (int i = 0; i < 500; i++)
            {
                //创建货道节点
                XmlNode chuhuoNode = mysalexmldoc.CreateElement("chuhuo" + i.ToString());//出货定义
                XmlAttribute timeAttribute = mysalexmldoc.CreateAttribute("time");//时间戳
                timeAttribute.Value = "";
                chuhuoNode.Attributes.Append(timeAttribute);//xml节点附件属性
                XmlAttribute shangpinnumAttribute = mysalexmldoc.CreateAttribute("shangpinnum");//对应商品编号
                shangpinnumAttribute.Value = "";
                chuhuoNode.Attributes.Append(shangpinnumAttribute);//xml节点附件属性
                XmlAttribute jiageAttribute = mysalexmldoc.CreateAttribute("jiage");//商品价格
                jiageAttribute.Value = "";
                chuhuoNode.Attributes.Append(jiageAttribute);//xml节点附件属性
                XmlAttribute typeAttribute = mysalexmldoc.CreateAttribute("type");//支付方式
                typeAttribute.Value = "";
                chuhuoNode.Attributes.Append(typeAttribute);//xml节点附件属性

                XmlAttribute startAttribute = mysalexmldoc.CreateAttribute("start");//是否是最新记录
                startAttribute.Value = "";
                chuhuoNode.Attributes.Append(startAttribute);//xml节点附件属性

                sale1Node.AppendChild(chuhuoNode);
            }
            rootNode.AppendChild(sale1Node);

            //创建销售数据节点1
            XmlNode sale2Node = mysalexmldoc.CreateElement("pay");//支付定义
            for (int i = 0; i < 500; i++)
            {
                //创建货道节点
                XmlNode payNode = mysalexmldoc.CreateElement("pay" + i.ToString());//支付定义
                XmlAttribute timeAttribute = mysalexmldoc.CreateAttribute("time");//时间戳
                timeAttribute.Value = "";
                payNode.Attributes.Append(timeAttribute);//xml节点附件属性
                XmlAttribute moneyAttribute = mysalexmldoc.CreateAttribute("money");//支付金额
                moneyAttribute.Value = "";
                payNode.Attributes.Append(moneyAttribute);//xml节点附件属性
                XmlAttribute typeAttribute = mysalexmldoc.CreateAttribute("type");//支付方式
                typeAttribute.Value = "";
                payNode.Attributes.Append(typeAttribute);//xml节点附件属性

                XmlAttribute startAttribute = mysalexmldoc.CreateAttribute("start");//是否是最新记录
                startAttribute.Value = "";
                payNode.Attributes.Append(startAttribute);//xml节点附件属性

                sale2Node.AppendChild(payNode);
            }
            rootNode.AppendChild(sale2Node);

            mysalexmldoc.AppendChild(rootNode);
        }

        /// <summary>
        /// 初始化注册文件
        /// </summary>
        public void initregxml()
        {
            myregxmldoc.RemoveAll();//去除所有节点
            myregxmldoc.CreateXmlDeclaration("1.0", "utf-8", "yes");
            //创建根节点
            XmlNode rootNode = myregxmldoc.CreateElement("reg");//配置定义
            XmlAttribute regAttribute0 = myregxmldoc.CreateAttribute("regid");//注册号
            regAttribute0.Value = "0";
            rootNode.Attributes.Append(regAttribute0);//xml节点附件属性

            myregxmldoc.AppendChild(rootNode);
        }
        
        /// <summary>
        /// 更新各个配置节点路径
        /// </summary>
        private void updatenodeaddress()
        {
            mynetcofignode = myxmldoc.SelectSingleNode("config").SelectSingleNode("netconfig");
            myfunctionnode = myxmldoc.SelectSingleNode("config").SelectSingleNode("function");
            mypayconfignode = myxmldoc.SelectSingleNode("config").SelectSingleNode("payconfig");
            mynodelistshangpin = myxmldoc.SelectSingleNode("config").SelectSingleNode("shangpin").ChildNodes;
            mynodelisthuodao = myxmldoc.SelectSingleNode("config").SelectSingleNode("huodao").ChildNodes;
            mynodelistpay = mysalexmldoc.SelectSingleNode("sale").SelectSingleNode("pay").ChildNodes;
            try
            {
                paytypes = int.Parse(mypayconfignode.Attributes.GetNamedItem("allpay").Value);
            }
            catch
            {
            }
            try
            {
                Getvendortype();
                if (vendortype != myfunctionnode.Attributes.GetNamedItem("vendortype").Value)
                {
                    myfunctionnode.Attributes.GetNamedItem("vendortype").Value = vendortype;
                    mysalexmldoc.Save(salexmlfile);
                    mysalexmldoc.Save(salexmlfilecopy);
                }
            }
            catch { }
        }
        
        /// <summary>
        /// 添加支付记录
        /// </summary>
        /// <param name="money">支付金额</param>
        /// <param name="type">支付方式</param>
        private void addpayrecord(double money,string type)
        {
            int i;
            for (i = 0; i < mynodelistpay.Count; i++)
            {
                if (mynodelistpay[i].Attributes.GetNamedItem("start").Value == "1")
                {
                    mynodelistpay[i].Attributes.GetNamedItem("time").Value = DateTime.Now.ToString("MM-dd HH:mm:ss");
                    mynodelistpay[i].Attributes.GetNamedItem("money").Value = money.ToString();
                    mynodelistpay[i].Attributes.GetNamedItem("type").Value = type;
                    mynodelistpay[i].Attributes.GetNamedItem("start").Value = "";
                    if (i == mynodelistpay.Count - 1)
                    {
                        mynodelistpay[0].Attributes.GetNamedItem("start").Value = "1";
                    }
                    else
                    {
                        mynodelistpay[i + 1].Attributes.GetNamedItem("start").Value = "1";
                    }
                    break;
                }
            }
            if (i == mynodelistpay.Count)//未找到起始位置
            {
                mynodelistpay[0].Attributes.GetNamedItem("time").Value = DateTime.Now.ToString("MM-dd HH:mm:ss");
                mynodelistpay[0].Attributes.GetNamedItem("money").Value = money.ToString();
                mynodelistpay[0].Attributes.GetNamedItem("type").Value = type;
                mynodelistpay[0].Attributes.GetNamedItem("start").Value = "";
                mynodelistpay[1].Attributes.GetNamedItem("start").Value = "1";
            }
            mysalexmldoc.Save(salexmlfile);
            mysalexmldoc.Save(salexmlfilecopy);
        }

        /// <summary>
        /// 设置出货
        /// </summary>
        private void setchuhuo()
        {
            //出货设置
            Aisleoutcount = 1;//超时计时开始
        }
       
        private void InitFormsize()
        {
            this.Width = 1920;
            this.Height = 1080;
            this.pictureBox1.Dock = DockStyle.None;
            this.pictureBox1.Width = 0;
            this.pictureBox1.Height = 0;
            this.axWindowsMediaPlayer1.Width = 1920;//视频1000x525
            this.axWindowsMediaPlayer1.Height = 1080;
            this.axWindowsMediaPlayer1.uiMode = "None";
            this.axWindowsMediaPlayer1.stretchToFit = true;
            this.axWindowsMediaPlayer1.Location = new Point(0, 0);
            this.axWindowsMediaPlayer1.settings.autoStart = true;
            this.axWindowsMediaPlayer1.settings.setMode("loop", true);
            this.imageList1.ImageSize = new Size(215, 215);

            this.label2.Location = new Point(1400, 10);
            this.label5.Location = new Point(210, 740);
            this.label15.Location = new Point(668, 700);
            this.label16.Location = new Point(1133, 700);
            this.pictureBox6.Location = new Point(800, 800);
            this.pictureBox7.Location = new Point(550, 400);
            this.pictureBox8.Location = new Point(1020, 400);
            
            needupdatePlaylist = true;
        }

        #endregion

        #region 服务器操作

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="url">图片Url</param>
        /// <param name="path">保存地址</param>
        public void DownLoadPicture(string url, string path)
        {
            List<string> folders = new List<string>()
            {
                url
            };
            List<DownloadFile> downloadFiles = new List<DownloadFile>();
            Parallel.ForEach(folders, folder =>
            {
                downloadFiles.AddRange(ReadFileUrl(url, path));
            });
            List<Task> tList = new List<Task>();
            downloadFiles.ForEach(p =>
            {
                tList.Add(
                    DownloadingDataFromServerAsync(p)
                );
            });
            Task.WaitAll(tList.ToArray());
        }

        /// <summary>
        /// 向服务器发送交易数据
        /// </summary>
        private void sendtrade()
        {
            if (netsendrecord[netsendindex, 0] != 0)
            {
                for (int i = 0; i < 34; i++)
                {
                    GSMTxBuffer[i] = netsendrecord[netsendindex, i];
                }
                myTcpCli.Sendbytes(GSMTxBuffer, 34);
                if (myfunctionnode.Attributes.GetNamedItem("netlog").Value == "1")
                {
                    revstringnet = DateTime.Now.ToString() + "Send:";
                    for (int revcount = 0; revcount < 34; revcount++)
                    {
                        revstringnet += " " + Convert.ToString(GSMTxBuffer[revcount], 16);
                    }
                    netdatastream.Write(Encoding.ASCII.GetBytes(revstringnet), 0, revstringnet.Length);
                    netdatastream.WriteByte(0x0d);
                    netdatastream.WriteByte(0x0a);
                    netdatastream.Flush();
                }

                //netreturncount = 1;//超时计时开始
                netcount = 0;//状态数据发送间隔重新开始

                //如果是出货失败的不需要返回确认
                if (netsendrecord[netsendindex, 19] == 0xE3)
                {
                    for (int i = 0; i < 34; i++)
                    {
                        netsendrecord[netsendindex, i] = 0;
                    }
                    
                    netsendindex++;//序号增加1
                    if (netsendindex >= 200)
                        netsendindex = 0;
                    needsendrecordnum--;
                }
            }
        }

        /// <summary>
        /// 向服务器发送确认信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="state">状态</param>
        private void sendRETURNOK(int type, int state)
        {
            int i;
            GSMTxBuffer[0] = 0x01;
            GSMTxBuffer[1] = 0x71;
            GSMTxBuffer[2] = 0x00;
            GSMTxBuffer[3] = 0x22;
            for (i = 0; i < 15; i++)
            {
                GSMTxBuffer[4 + i] = IMEI[i];
            }
            GSMTxBuffer[19] = 0xff;
	        GSMTxBuffer[20] = 0x00;
            GSMTxBuffer[21] = (byte)state;
            GSMTxBuffer[22] = (byte)type;//类型为广告更新,图片确认等

            if (type == 3)
            {
                GSMTxBuffer[23] = (byte)shangpingnumreturn;
            }
            else
            {
                GSMTxBuffer[23] = 0x00;
            }
            GSMTxBuffer[24] = 0x00;
            GSMTxBuffer[25] = 0x00;

            if(netstep == 13)
            {
                GSMTxBuffer[26] = timerecord[3,0] ;
                GSMTxBuffer[27] = timerecord[3,1];
                GSMTxBuffer[28] = timerecord[3,2];
                GSMTxBuffer[29] = timerecord[3,3];
                GSMTxBuffer[30] = timerecord[3,4];
                GSMTxBuffer[31] = timerecord[3,5];
            }
            else
            {
                GSMTxBuffer[26] = (byte)(System.DateTime.Now.Year - 2000);
                GSMTxBuffer[27] = (byte)(System.DateTime.Now.Month);
                GSMTxBuffer[28] = (byte)(System.DateTime.Now.Day);
                GSMTxBuffer[29] = (byte)(System.DateTime.Now.Hour);
                GSMTxBuffer[30] = (byte)(System.DateTime.Now.Minute);
                GSMTxBuffer[31] = (byte)(System.DateTime.Now.Second);
            }
            

            GSMTxBuffer[32] = 0x0d;
            GSMTxBuffer[33] = 0x0a;
            myTcpCli.Sendbytes(GSMTxBuffer, 34);
            if (myfunctionnode.Attributes.GetNamedItem("netlog").Value == "1")
            {
                revstringnet = DateTime.Now.ToString() + "Send:";
                for (int revcount = 0; revcount < 34; revcount++)
                {
                    revstringnet += " " + Convert.ToString(GSMTxBuffer[revcount], 16);
                }
                netdatastream.Write(Encoding.ASCII.GetBytes(revstringnet), 0, revstringnet.Length);
                netdatastream.WriteByte(0x0d);
                netdatastream.WriteByte(0x0a);
                netdatastream.Flush();
            }
            //netreturncount = 1;//超时计时开始
            netcount = 0;//状态数据发送间隔重新开始
        }

        /// <summary>
        /// 向服务器发送提货码
        /// </summary>
        private void sendtihuoma()
        {
            int i;
            GSMTxBuffer[0] = 0x01;
            GSMTxBuffer[1] = 0x81;
            GSMTxBuffer[2] = 0x00;
            GSMTxBuffer[3] = 0x2d;
            for (i = 0; i < 15; i++)
            {
                GSMTxBuffer[4 + i] = IMEI[i];
            }
            GSMTxBuffer[19] = 0x01;
            byte[] tihuomatemp=Encoding.ASCII.GetBytes(myTihuomastr);
            for (i = 0; i < 7; i++)
            {
                GSMTxBuffer[20 + i] = tihuomatemp[i];
            }
            for (i = 0; i < 10; i++)
            {
                GSMTxBuffer[27 + i] = 0x00;
            }

            GSMTxBuffer[37] = (byte)(System.DateTime.Now.Year - 2000);
            GSMTxBuffer[38] = (byte)(System.DateTime.Now.Month);
            GSMTxBuffer[39] = (byte)(System.DateTime.Now.Day);
            GSMTxBuffer[40] = (byte)(System.DateTime.Now.Hour);
            GSMTxBuffer[41] = (byte)(System.DateTime.Now.Minute);
            GSMTxBuffer[42] = (byte)(System.DateTime.Now.Second);

            GSMTxBuffer[43] = 0x0d;
            GSMTxBuffer[44] = 0x0a;
            myTcpCli.Sendbytes(GSMTxBuffer, 45);
            if (myfunctionnode.Attributes.GetNamedItem("netlog").Value == "1")
            {
                revstringnet = DateTime.Now.ToString() + "Send:";
                for (int revcount = 0; revcount < 45; revcount++)
                {
                    revstringnet += " " + Convert.ToString(GSMTxBuffer[revcount], 16);
                }
                netdatastream.Write(Encoding.ASCII.GetBytes(revstringnet), 0, revstringnet.Length);
                netdatastream.WriteByte(0x0d);
                netdatastream.WriteByte(0x0a);
                netdatastream.Flush();
            }
            //netreturncount = 1;//超时计时开始
            netcount = 0;//状态数据发送间隔重新开始
        }

        private int item;//商品号
        private int suijinshu;
        /// <summary>
        ///  添加向服务器发送的数据
        /// </summary>
        /// <param name="tradetype">交易类型：收款，退款，出货</param>
        /// <param name="jine">金额</param>
        /// <param name="paytype">支付方式</param>
        /// <param name="netliushui">网络流水号</param>
        /// <param name="item">商品号</param>
        private void addnettrade(byte tradetype,double jine,byte paytype, int netliushui)
        {
            if (needsendrecordnum < 200)
            {
                netsendrecord[netsendrecordindex, 0] = 0x01;
                netsendrecord[netsendrecordindex, 1] = 0x71;
                netsendrecord[netsendrecordindex, 2] = 0x00;
                netsendrecord[netsendrecordindex, 3] = 0x22;
                for (int i = 0; i < 15; i++)
                {
                    netsendrecord[netsendrecordindex, 4 + i] = IMEI[i];
                }
                netsendrecord[netsendrecordindex, 19] = tradetype;
                netsendrecord[netsendrecordindex, 20] = (byte)(((int)(jine * 10)) >> 8);
                netsendrecord[netsendrecordindex, 21] = (byte)((int)(jine * 10));
                if ((tradetype == 0x03)|| (tradetype == 0xE3))//出货
                {
                    netsendrecord[netsendrecordindex, 22] = 0x00;
                    netsendrecord[netsendrecordindex, 23] = (byte)item;
                }
                else
                {
                    netsendrecord[netsendrecordindex, 22] = paytype;
                    netsendrecord[netsendrecordindex, 23] = 0x00;
                   
                }
                if ((paytype == 0x06) || (paytype == 0x07))//支付宝或微信
                {
                    netsendrecord[netsendrecordindex, 24] = (byte)(((int)netliushui) >> 8);
                    netsendrecord[netsendrecordindex, 25] = (byte)((int)netliushui);
                }
                else
                {
                    netsendrecord[netsendrecordindex, 24] = 0x00;
                    netsendrecord[netsendrecordindex, 25] = 0x00;
                }
                netsendrecord[netsendrecordindex, 26] = (byte)(System.DateTime.Now.Year - 2000);
                netsendrecord[netsendrecordindex, 27] = (byte)(System.DateTime.Now.Month);
                netsendrecord[netsendrecordindex, 28] = (byte)(System.DateTime.Now.Day);
                netsendrecord[netsendrecordindex, 29] = (byte)(System.DateTime.Now.Hour);
                netsendrecord[netsendrecordindex, 30] = (byte)(System.DateTime.Now.Minute);
                suijinshu = suijinshu % 3;
                netsendrecord[netsendrecordindex, 31] = (byte)(System.DateTime.Now.Second + suijinshu * 60);
                suijinshu++;

                netsendrecord[netsendrecordindex, 32] = 0x0d;
                netsendrecord[netsendrecordindex, 33] = 0x0a;

                netsendrecordindex++;//序号增加1
                if (netsendrecordindex >= 200)
                    netsendrecordindex = 0;
                needsendrecordnum++;//需要发送的记录数量
                if (needsendrecordnum > 200)
                    needsendrecordnum = 200;
            }
        }

        /// <summary>
        /// 向服务器发送状态数据
        /// </summary>
        private void netsendstate()
        {
            int i;
            totalshangpinnum = mynodelistshangpin.Count;//商品数量

            GSMTxBuffer[0] = 0x01;
            GSMTxBuffer[1] = 0x70;
            GSMTxBuffer[2] = (byte)((56 + totalshangpinnum * 4) >> 8);
            GSMTxBuffer[3] = (byte)((56 + totalshangpinnum * 4) & 0xff);

            for (i = 0; i < 15; i++)
            {
                GSMTxBuffer[4 + i] = IMEI[i];
            }
            byte[] softversion = new Coder(Coder.EncodingMothord.ASCII).GetEncodingBytes(versionstring);
            if (softversion.Length >= 15)
            {
                for (i = 0; i < 15; i++)
                    GSMTxBuffer[19 + i] = softversion[i];
            }
            else
            {
                for (i = 0; i < softversion.Length; i++)
                    GSMTxBuffer[19 + i] = softversion[i];
            }
            GSMTxBuffer[34] = (byte)mynodelistshangpin.Count;
            //查找库存和货道状态
            for (i = 0; i < mynodelistshangpin.Count; i++)
            {
                GSMTxBuffer[35 + i] = byte.Parse(mynodelistshangpin[i].Attributes.GetNamedItem("state").Value);//Aislestate[i];
                //查找库存状态
                int totalkuncun = 0;//计算总库存
                for (int k = 0; k < mynodelisthuodao.Count; k++)
                {
                    if (int.Parse(mynodelistshangpin[i].Attributes.GetNamedItem("huodao").Value)
                        == int.Parse(mynodelisthuodao[k].Attributes.GetNamedItem("huodaonum").Value))
                    {
                        if (int.Parse(mynodelisthuodao[k].Attributes.GetNamedItem("state").Value) == 0)//货道反馈正常（状态） 
                        {
                            totalkuncun += int.Parse(mynodelisthuodao[k].Attributes.GetNamedItem("kucun").Value);//对应货道库存
                        }
                        for (int index = 0; index < mynodelisthuodao.Count; index++)
                        {
                            if ((int.Parse(mynodelisthuodao[index].Attributes.GetNamedItem("fenzu").Value)
                                 == int.Parse(mynodelisthuodao[k].Attributes.GetNamedItem("fenzu").Value))
                                 &&(index != k))
                            {
                                if (int.Parse(mynodelisthuodao[index].Attributes.GetNamedItem("state").Value) == 0)//货道反馈正常（状态）
                                {
                                    totalkuncun += int.Parse(mynodelisthuodao[index].Attributes.GetNamedItem("kucun").Value);
                                }
                            }
                        }
                    }
                }
                if (totalkuncun > 255)
                {
                    GSMTxBuffer[35 + totalshangpinnum + i] = 0xff;//库存大于255，发送255
                }
                else
                {
                    GSMTxBuffer[35 + totalshangpinnum + i] = (byte)totalkuncun;//库存大于255，发送255
                }
                int shangpinprices = (int)(double.Parse(mynodelistshangpin[i].Attributes.GetNamedItem("jiage").Value) * 10);
                GSMTxBuffer[35 + 2 * totalshangpinnum + 2 * i] = (byte)(shangpinprices >> 8);
                GSMTxBuffer[35 + 2 * totalshangpinnum + 2 * i + 1] = (byte)shangpinprices;

            }

            GSMTxBuffer[35 + 4 * totalshangpinnum] = 0;//zhibijistate
            GSMTxBuffer[36 + 4 * totalshangpinnum] = 0;//yingbiqistate
            GSMTxBuffer[37 + 4 * totalshangpinnum] = 0;//EXboardstate
            GSMTxBuffer[38 + 4 * totalshangpinnum] = 0;//POS机
            GSMTxBuffer[39 + 4 * totalshangpinnum] = 1;//GPRS状态GPRSstate
            GSMTxBuffer[40 + 4 * totalshangpinnum] = 0;//numinpayout
            GSMTxBuffer[41 + 4 * totalshangpinnum] = 0;

            GSMTxBuffer[42 + 4 * totalshangpinnum] = 32;//NET_dBm
            GSMTxBuffer[43 + 4 * totalshangpinnum] = 0;//备用信息
            GSMTxBuffer[44 + 4 * totalshangpinnum] = 0;
            GSMTxBuffer[45 + 4 * totalshangpinnum] = 0;
            GSMTxBuffer[46 + 4 * totalshangpinnum] = 0;
            GSMTxBuffer[47 + 4 * totalshangpinnum] = 0;
            GSMTxBuffer[48 + 4 * totalshangpinnum] = (byte)(System.DateTime.Now.Year - 2000);
            GSMTxBuffer[49 + 4 * totalshangpinnum] = (byte)(System.DateTime.Now.Month);
            GSMTxBuffer[50 + 4 * totalshangpinnum] = (byte)(System.DateTime.Now.Day);
            GSMTxBuffer[51 + 4 * totalshangpinnum] = (byte)(System.DateTime.Now.Hour);
            GSMTxBuffer[52 + 4 * totalshangpinnum] = (byte)(System.DateTime.Now.Minute);
            GSMTxBuffer[53 + 4 * totalshangpinnum] = (byte)(System.DateTime.Now.Second);
            GSMTxBuffer[54 + 4 * totalshangpinnum] = 0x0d;
            GSMTxBuffer[55 + 4 * totalshangpinnum] = 0x0a;
            myTcpCli.Sendbytes(GSMTxBuffer, 56 + 4 * totalshangpinnum);
            if (myfunctionnode.Attributes.GetNamedItem("netlog").Value == "1")
            {
                revstringnet = DateTime.Now.ToString() + "Send:";
                for (int revcount = 0; revcount < 56 + 4 * totalshangpinnum; revcount++)
                {
                    revstringnet += " " + Convert.ToString(GSMTxBuffer[revcount], 16);
                }
                netdatastream.Write(Encoding.ASCII.GetBytes(revstringnet), 0, revstringnet.Length);
                netdatastream.WriteByte(0x0d);
                netdatastream.WriteByte(0x0a);
                netdatastream.Flush();
            }
            //netreturncount = 1;//超时计时开始
            netcount = 0;//状态数据发送间隔重新开始
        }

        private tihuoma mytihuoma;//提货码页面

        #endregion

        #region 控件
        
        public static bool needopensettingform;
        private void button2_Click(object sender, EventArgs e)
        {
            needopensettingform = true;
        }
        
        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            //如果已播放完毕就播放下一个文件
            if ((WMPLib.WMPPlayState)e.newState == WMPLib.WMPPlayState.wmppsReady) axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            HMIstep = 1;//触摸选货界面
            guanggaoreturntime = 0;
            axWindowsMediaPlayer1.Visible = false;
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            axWindowsMediaPlayer1.currentPlaylist.clear();

            if (mytihuoma == null)
            {
                mytihuoma = new tihuoma();
                if (mytihuoma.ShowDialog() == DialogResult.Yes)
                {

                }
                //mytihuoma.Dispose();
                mytihuoma = null;
            }
        }

        private void axWindowsMediaPlayer1_ClickEvent(object sender, AxWMPLib._WMPOCXEvents_ClickEvent e)
        {
            pictureBox1_Click(null, null);
        }

        private void pictureBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            pictureBox1_Click(null, null);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            PEPrinter.PE_Close(PEPrinter.PEhandle);
        }

        #endregion

        #region WorkingTest
        /// <summary>
        /// 模拟运行
        /// </summary>
        /// <param name="huodaoNum">商品号</param>
        /// <param name="PicPath">印章图案路径</param>
        public void WorkingTest(int huodaoNum,string PicPath)
        {
            log.CreateRunningLog("A1234", "白色印章", "90", "00", huodaoNum.ToString(), "模拟运行" + DateTime.Now.ToShortTimeString());
            int result = CargoStockAndStateCheck(huodaoNum.ToString());
            if(result < 90 && PLCHelper.nowStep == 0x00)//无报错
            {
                netreturncount = 0;//超时计时停止
                tihuoma.tihuomaresult = "模拟运行开始";
                try
                {
                    AddCoverPicture(huodaoNum);//加载盒体图片
                    pictureBox7.Load(PicPath);//加载印章图案
                    HMIstep = 3;//显示出货页面

                    log.WriteStepLog(StepType.印章图案检查, "状态正常");//日志记录
                }
                catch { }
                myprint = PEPrinter.GetPEPrinterExample();
                wulihuodao = result;
                timer3.Enabled = true;
                PLCHelper.nowStep = 0x01;

                item = result;//实际出货商品号
                guanggaoreturntime = 0;
                shangpinjiage = 0;//实际出货商品价格
                zhifutype = 4;//支付方式为提货码
                try
                {
                    setchuhuo();
                    PEPrinter.PicPath = PicPath;
                }
                catch(Exception ex)
                {
                }
            }
            else if (result == 90)
            {
            }
            else if (result == 91)
            {
            }
            else if (result == 92)
            {
            }
        }

        #endregion

        #region 运行步骤显示

        int timingCount = 0;
        /// <summary>
        /// 设备运行过程显示
        /// </summary>
        private void RunningDisplay()
        {
            switch (PLCHelper.nowStep)
            {
                case 0x00:
                    break;
                case 0x01:
                    showprintstate = "印章制作中:取外壳,请稍等";
                    break;
                case 0x02:
                    showprintstate = "印章制作中:取印面,请稍等";
                    break;
                case 0x03:
                    showprintstate = "印章制作中:等待打印,请稍等";
                    break;
                case 0x04:
                    showprintstate = "印章制作中:正在打印,请稍等";
                    break;
                case 0x05:
                    showprintstate = "印章制作中:正在组装,请稍等";
                    break;
                case 0x06:
                    showprintstate = "印章制作中:正在出货,请稍等";
                    break;
                case 0x07:
                    showprintstate = "印章制作完成:等待取货,请稍等";
                    break;
                case 0x09:
                    break;
                case 0x70:
                    showprintstate = "打印机正在复位,请稍等";
                    break;
                case 0x80:
                    showprintstate = "机器正在复位,请稍等";
                    break;
                case 0x81:
                    showprintstate = "机器正在排故,请稍等";
                    break;
                case 0x98:
                    showprintstate = "打印机故障";
                    break;
                case 0x99:
                    showprintstate = "机器故障,请稍等";
                    break;
                default:
                    break;
            }
            if(PLCHelper.nowStep >= 0x10)//出现故障
            {
                label5.Text =  showprintstate + "     " + (PLC.runTiming).ToString() + "s";
            }
            else
            {
                if (timingCount < 2)
                {
                    timingCount++;
                }
                else
                {
                    label5.Text =  showprintstate + "     " + (PLC.runTiming--).ToString() + "s";
                    timingCount = 0;
                }
            }
            this.label2.Text = "编号:" + Encoding.ASCII.GetString(Form1.IMEI) + "  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (isICYOK)
                label2.ForeColor = System.Drawing.SystemColors.HighlightText;
            else
            {
                this.label2.ForeColor = System.Drawing.Color.Red;
            }
        }

        #endregion
        
    }
    public class DownloadFile
    {
        public DownloadFile(string fileName, string saveFileName)
        {
            FileName = fileName;
            SaveFileName = saveFileName;
        }
        public string FileName;
        public string SaveFileName;
    }
}
