﻿namespace SHJ
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            PCHMI.limits limits1 = new PCHMI.limits();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.imageList3 = new System.Windows.Forms.ImageList(this.components);
            this.imageList4 = new System.Windows.Forms.ImageList(this.components);
            this.imageList5 = new System.Windows.Forms.ImageList(this.components);
            this.config1 = new PCHMI.CONFIG();
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.video1 = new AForge.Controls.VideoSourcePlayer();
            this.pic_Erweima = new System.Windows.Forms.PictureBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.pel_SellTips = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.lbl_D8 = new System.Windows.Forms.Label();
            this.lbl_D7 = new System.Windows.Forms.Label();
            this.lbl_Photoing = new System.Windows.Forms.Label();
            this.lbl_D10 = new System.Windows.Forms.Label();
            this.lbl_D6 = new System.Windows.Forms.Label();
            this.lbl_D9 = new System.Windows.Forms.Label();
            this.lbl_D11 = new System.Windows.Forms.Label();
            this.lbl_D0 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Erweima)).BeginInit();
            this.panel4.SuspendLayout();
            this.pel_SellTips.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.axWindowsMediaPlayer1);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(672, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(380, 135);
            this.panel1.TabIndex = 0;
            this.panel1.Visible = false;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.button2.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.ForeColor = System.Drawing.Color.Transparent;
            this.button2.Location = new System.Drawing.Point(0, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(131, 77);
            this.button2.TabIndex = 12;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // axWindowsMediaPlayer1
            // 
            this.axWindowsMediaPlayer1.Enabled = true;
            this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(157, 12);
            this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
            this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(149, 96);
            this.axWindowsMediaPlayer1.TabIndex = 1;
            this.axWindowsMediaPlayer1.Visible = false;
            this.axWindowsMediaPlayer1.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(this.axWindowsMediaPlayer1_PlayStateChange);
            this.axWindowsMediaPlayer1.ClickEvent += new AxWMPLib._WMPOCXEvents_ClickEventHandler(this.axWindowsMediaPlayer1_ClickEvent);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(139, 96);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.pictureBox1_PreviewKeyDown);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(135, 150);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 500;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "xh0.jpg");
            this.imageList2.Images.SetKeyName(1, "xh1.jpg");
            this.imageList2.Images.SetKeyName(2, "xh2.jpg");
            this.imageList2.Images.SetKeyName(3, "xh3.jpg");
            this.imageList2.Images.SetKeyName(4, "xh4.jpg");
            // 
            // imageList3
            // 
            this.imageList3.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList3.ImageStream")));
            this.imageList3.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList3.Images.SetKeyName(0, "nolan.jpg");
            this.imageList3.Images.SetKeyName(1, "LAN.jpg");
            this.imageList3.Images.SetKeyName(2, "WIFI.jpg");
            this.imageList3.Images.SetKeyName(3, "3G.jpg");
            // 
            // imageList4
            // 
            this.imageList4.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList4.ImageStream")));
            this.imageList4.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList4.Images.SetKeyName(0, "人民币.jpg");
            this.imageList4.Images.SetKeyName(1, "支付宝.jpg");
            this.imageList4.Images.SetKeyName(2, "微信.jpg");
            this.imageList4.Images.SetKeyName(3, "一码多付.jpg");
            this.imageList4.Images.SetKeyName(4, "nfcall.jpg");
            this.imageList4.Images.SetKeyName(5, "会员卡.jpg");
            // 
            // imageList5
            // 
            this.imageList5.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList5.ImageStream")));
            this.imageList5.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList5.Images.SetKeyName(0, "NFC大.jpg");
            this.imageList5.Images.SetKeyName(1, "会员卡大.jpg");
            // 
            // config1
            // 
            this.config1.MAIN_HMI_IP = "";
            this.config1.MODBUS服务器配置 = null;
            this.config1.PC时间保存地址 = null;
            this.config1.允许同时运行多个程序 = false;
            this.config1.快速登录注销时间 = ((uint)(60u));
            this.config1.数据库连接 = null;
            this.config1.数据路径 = "D:\\";
            this.config1.画面 = null;
            this.config1.登录方式 = PCHMI.CONFIG.LOGType.快速登录;
            limits1.PLC = ((uint)(0u));
            limits1.地址 = "";
            limits1.限制类型 = PCHMI.limits.LType.无效;
            this.config1.运行限制 = limits1;
            this.config1.通讯配置 = new string[] {
        "MITSUBISHI_FX_SERIAL;COM=4,9600,2,7,1;SN=1;JumpBit="};
            this.config1.通讯配置文件名 = "PLC1";
            this.config1.随机数保存地址 = null;
            // 
            // timer3
            // 
            this.timer3.Interval = 500;
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // video1
            // 
            this.video1.Location = new System.Drawing.Point(0, 0);
            this.video1.Name = "video1";
            this.video1.Size = new System.Drawing.Size(878, 597);
            this.video1.TabIndex = 17;
            this.video1.Text = "videoSourcePlayer1";
            this.video1.VideoSource = null;
            this.video1.Visible = false;
            this.video1.NewFrame += new AForge.Controls.VideoSourcePlayer.NewFrameHandler(this.video1_NewFrame);
            // 
            // pic_Erweima
            // 
            this.pic_Erweima.Location = new System.Drawing.Point(40, 130);
            this.pic_Erweima.Name = "pic_Erweima";
            this.pic_Erweima.Size = new System.Drawing.Size(200, 200);
            this.pic_Erweima.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pic_Erweima.TabIndex = 16;
            this.pic_Erweima.TabStop = false;
            // 
            // panel4
            // 
            this.panel4.BackgroundImage = global::SHJ.Properties.Resources.tihuo;
            this.panel4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.pel_SellTips);
            this.panel4.Controls.Add(this.label16);
            this.panel4.Controls.Add(this.label15);
            this.panel4.Controls.Add(this.label5);
            this.panel4.Controls.Add(this.pictureBox8);
            this.panel4.Controls.Add(this.pictureBox7);
            this.panel4.Controls.Add(this.pictureBox6);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Location = new System.Drawing.Point(672, 174);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(784, 841);
            this.panel4.TabIndex = 14;
            this.panel4.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.label3.Location = new System.Drawing.Point(220, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 20);
            this.label3.TabIndex = 55;
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pel_SellTips
            // 
            this.pel_SellTips.BackColor = System.Drawing.Color.Transparent;
            this.pel_SellTips.Controls.Add(this.label1);
            this.pel_SellTips.Controls.Add(this.pictureBox2);
            this.pel_SellTips.Location = new System.Drawing.Point(268, 421);
            this.pel_SellTips.Name = "pel_SellTips";
            this.pel_SellTips.Size = new System.Drawing.Size(206, 217);
            this.pel_SellTips.TabIndex = 54;
            this.pel_SellTips.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("幼圆", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.DarkGreen;
            this.label1.Location = new System.Drawing.Point(29, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 29);
            this.label1.TabIndex = 1;
            this.label1.Text = "下方出货";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::SHJ.Properties.Resources.arrow_down_green;
            this.pictureBox2.Location = new System.Drawing.Point(20, 41);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(158, 162);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.Color.Transparent;
            this.label16.Font = new System.Drawing.Font("幼圆", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label16.ForeColor = System.Drawing.Color.DarkGreen;
            this.label16.Location = new System.Drawing.Point(284, 344);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(110, 24);
            this.label16.TabIndex = 53;
            this.label16.Text = "印章外观";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.Color.Transparent;
            this.label15.Font = new System.Drawing.Font("幼圆", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label15.ForeColor = System.Drawing.Color.DarkGreen;
            this.label15.Location = new System.Drawing.Point(152, 344);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(110, 24);
            this.label15.TabIndex = 52;
            this.label15.Text = "印章图案";
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("幼圆", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.Color.DarkGreen;
            this.label5.Location = new System.Drawing.Point(20, 368);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(1500, 50);
            this.label5.TabIndex = 50;
            this.label5.Text = "正在制作印章,请稍等..";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox8
            // 
            this.pictureBox8.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox8.Image = global::SHJ.Properties.Resources.shangpin;
            this.pictureBox8.Location = new System.Drawing.Point(475, 51);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(350, 290);
            this.pictureBox8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox8.TabIndex = 49;
            this.pictureBox8.TabStop = false;
            // 
            // pictureBox7
            // 
            this.pictureBox7.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox7.Image = global::SHJ.Properties.Resources.shangpin;
            this.pictureBox7.Location = new System.Drawing.Point(268, 51);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(350, 290);
            this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox7.TabIndex = 48;
            this.pictureBox7.TabStop = false;
            // 
            // pictureBox6
            // 
            this.pictureBox6.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox6.Image = global::SHJ.Properties.Resources.waitnew_unscreen;
            this.pictureBox6.Location = new System.Drawing.Point(12, 51);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(316, 248);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox6.TabIndex = 47;
            this.pictureBox6.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.label2.Location = new System.Drawing.Point(543, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(238, 24);
            this.label2.TabIndex = 46;
            this.label2.Text = "2016-03-07 12:00:00";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button7);
            this.panel2.Controls.Add(this.button6);
            this.panel2.Controls.Add(this.button5);
            this.panel2.Controls.Add(this.button4);
            this.panel2.Controls.Add(this.button3);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.lbl_D8);
            this.panel2.Controls.Add(this.lbl_D7);
            this.panel2.Controls.Add(this.lbl_Photoing);
            this.panel2.Controls.Add(this.lbl_D10);
            this.panel2.Controls.Add(this.lbl_D6);
            this.panel2.Controls.Add(this.lbl_D9);
            this.panel2.Controls.Add(this.lbl_D11);
            this.panel2.Controls.Add(this.lbl_D0);
            this.panel2.Location = new System.Drawing.Point(0, 603);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(471, 253);
            this.panel2.TabIndex = 18;
            this.panel2.Visible = false;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(234, 19);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(82, 31);
            this.button7.TabIndex = 1;
            this.button7.Text = "位置记录";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(376, 164);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(82, 31);
            this.button6.TabIndex = 1;
            this.button6.Text = "出货位置";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(376, 127);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(82, 31);
            this.button5.TabIndex = 1;
            this.button5.Text = "装载印章";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(376, 88);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(82, 31);
            this.button4.TabIndex = 1;
            this.button4.Text = "印面拍照";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(376, 14);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(82, 31);
            this.button3.TabIndex = 1;
            this.button3.Text = "放印面";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button1_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(376, 51);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(82, 31);
            this.button1.TabIndex = 1;
            this.button1.Text = "装配盖子";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // lbl_D8
            // 
            this.lbl_D8.AutoSize = true;
            this.lbl_D8.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_D8.Location = new System.Drawing.Point(25, 193);
            this.lbl_D8.Name = "lbl_D8";
            this.lbl_D8.Size = new System.Drawing.Size(40, 16);
            this.lbl_D8.TabIndex = 0;
            this.lbl_D8.Text = "D8：";
            // 
            // lbl_D7
            // 
            this.lbl_D7.AutoSize = true;
            this.lbl_D7.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_D7.Location = new System.Drawing.Point(124, 110);
            this.lbl_D7.Name = "lbl_D7";
            this.lbl_D7.Size = new System.Drawing.Size(40, 16);
            this.lbl_D7.TabIndex = 0;
            this.lbl_D7.Text = "D7：";
            // 
            // lbl_Photoing
            // 
            this.lbl_Photoing.AutoSize = true;
            this.lbl_Photoing.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_Photoing.Location = new System.Drawing.Point(348, 223);
            this.lbl_Photoing.Name = "lbl_Photoing";
            this.lbl_Photoing.Size = new System.Drawing.Size(120, 16);
            this.lbl_Photoing.TabIndex = 0;
            this.lbl_Photoing.Text = "正在拍照！！！";
            this.lbl_Photoing.Visible = false;
            // 
            // lbl_D10
            // 
            this.lbl_D10.AutoSize = true;
            this.lbl_D10.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_D10.Location = new System.Drawing.Point(254, 193);
            this.lbl_D10.Name = "lbl_D10";
            this.lbl_D10.Size = new System.Drawing.Size(48, 16);
            this.lbl_D10.TabIndex = 0;
            this.lbl_D10.Text = "D10：";
            // 
            // lbl_D6
            // 
            this.lbl_D6.AutoSize = true;
            this.lbl_D6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_D6.Location = new System.Drawing.Point(25, 110);
            this.lbl_D6.Name = "lbl_D6";
            this.lbl_D6.Size = new System.Drawing.Size(40, 16);
            this.lbl_D6.TabIndex = 0;
            this.lbl_D6.Text = "D6：";
            // 
            // lbl_D9
            // 
            this.lbl_D9.AutoSize = true;
            this.lbl_D9.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_D9.Location = new System.Drawing.Point(124, 193);
            this.lbl_D9.Name = "lbl_D9";
            this.lbl_D9.Size = new System.Drawing.Size(40, 16);
            this.lbl_D9.TabIndex = 0;
            this.lbl_D9.Text = "D9：";
            // 
            // lbl_D11
            // 
            this.lbl_D11.AutoSize = true;
            this.lbl_D11.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_D11.Location = new System.Drawing.Point(123, 19);
            this.lbl_D11.Name = "lbl_D11";
            this.lbl_D11.Size = new System.Drawing.Size(48, 16);
            this.lbl_D11.TabIndex = 0;
            this.lbl_D11.Text = "D11：";
            // 
            // lbl_D0
            // 
            this.lbl_D0.AutoSize = true;
            this.lbl_D0.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_D0.Location = new System.Drawing.Point(24, 19);
            this.lbl_D0.Name = "lbl_D0";
            this.lbl_D0.Size = new System.Drawing.Size(40, 16);
            this.lbl_D0.TabIndex = 0;
            this.lbl_D0.Text = "D0：";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1490, 1080);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pic_Erweima);
            this.Controls.Add(this.video1);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = " ";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Erweima)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.pel_SellTips.ResumeLayout(false);
            this.pel_SellTips.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.ImageList imageList1;
        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.ImageList imageList3;
        private System.Windows.Forms.ImageList imageList4;
        private System.Windows.Forms.ImageList imageList5;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureBox8;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.PictureBox pic_Erweima;
        internal PCHMI.CONFIG config1;
        private System.Windows.Forms.Timer timer3;
        private AForge.Controls.VideoSourcePlayer video1;
        private System.Windows.Forms.Panel pel_SellTips;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbl_D8;
        private System.Windows.Forms.Label lbl_D7;
        private System.Windows.Forms.Label lbl_D10;
        private System.Windows.Forms.Label lbl_D6;
        private System.Windows.Forms.Label lbl_D9;
        private System.Windows.Forms.Label lbl_D11;
        private System.Windows.Forms.Label lbl_D0;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label lbl_Photoing;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Label label3;
    }
}

