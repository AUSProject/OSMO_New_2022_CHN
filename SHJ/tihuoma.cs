﻿using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SHJ
{
    public partial class tihuoma : Form
    {
        public tihuoma()
        {
            InitializeComponent();
        }
        
        public static string tihuomaresult= "请输入提货码";//验证结果提示语
        private PrintHelper print = null;

        public static bool ErrorToken = false;//设置故障标志

        private void label11_Click(object sender, EventArgs e)
        {
            Form1.checktihuoma = false;//取消验证
            Form1.HMIstep = 0;//广告页面
            Form1.needupdatePlaylist = true;//需要更新播放列表
            this.DialogResult = DialogResult.No;
            this.Close();
        }     

        public static string tihuomastring;

        private void updateshow()
        {
            if(Form1.myfunctionnode.Attributes.GetNamedItem("vendortype").Value == "1")//印章打印机
            {
                tihuomaresult = tihuomaresult.Replace("提货", "打印");
            }
            label2.Text = tihuomaresult;
            if(Form1.isICYOK)
            {
                this.label10.ForeColor = System.Drawing.SystemColors.HighlightText;
            }
            else
            {
                this.label10.ForeColor = System.Drawing.Color.Red;
            }
            this.label10.Text = "编号:" + Encoding.ASCII.GetString(Form1.IMEI) + "  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (Form1.checktihuoma)//需要验证提货码
            {
                pictureBox1.Visible = true;
            }
            else
            {
                pictureBox1.Visible = false;
            }
            System.Windows.Forms.Application.DoEvents();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            updateshow();
            if (ErrorToken)
            {
                panel_Error.Visible = true;
                lbl_Msg.Text = "设备故障，暂停使用";
                lbl_Msg2.Visible = true;
            }
            else if (Form1.ReturnStock() == 0)//库存检测
            {
                panel_Error.Visible = true;
                lbl_Msg.Text = "设备无库存，暂停使用";
                lbl_Msg2.Visible = false;
            }
            else if (print.PrintFaultInspect() != null)//打印机检查 
            {
                panel_Error.Visible = true;
                lbl_Msg.Text ="设备故障，暂停使用";
                lbl_Msg2.Visible = false;
            }
            else if (!PLCHelper.CheckPortConnect())//设备连接检查
            {
                lbl_Msg.Text = "设备故障，暂停使用";
                panel_Error.Visible = true;
                lbl_Msg2.Visible = false;
            }
            else if (!PLCHelper.GoodsInspect())//印面数量检查
            {
                lbl_Msg.Text = "设备无库存，暂停使用";
                panel_Error.Visible = true;
                lbl_Msg2.Visible = false;
            }
            else
            {
                panel_Error.Visible = false;
                lbl_Msg2.Visible = false;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.TextLength;
        }

        #region Keyboard

        private void button13_Click(object sender, EventArgs e)
        {
            this.label2.Focus();//获取焦点
            if(textBox1.Text.Length<7)//提货码七位
            {
                textBox1.Text += "1";
            }
            tihuomaresult = "请输入提货码";
            Form1.guanggaoreturntime = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.label2.Focus();//获取焦点
            if (textBox1.Text.Length < 7)//提货码七位
            {
                textBox1.Text += "2";
            }
            tihuomaresult = "请输入提货码";
            Form1.guanggaoreturntime = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.label2.Focus();//获取焦点
            if (textBox1.Text.Length < 7)//提货码七位
            {
                textBox1.Text += "3";
            }
            tihuomaresult = "请输入提货码";
            Form1.guanggaoreturntime = 0;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.label2.Focus();//获取焦点
            if (textBox1.Text.Length < 7)//提货码七位
            {
                textBox1.Text += "4";
            }
            tihuomaresult = "请输入提货码";
            Form1.guanggaoreturntime = 0;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.label2.Focus();//获取焦点
            if (textBox1.Text.Length < 7)//提货码七位
            {
                textBox1.Text += "5";
            }
            tihuomaresult = "请输入提货码";
            Form1.guanggaoreturntime = 0;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.label2.Focus();//获取焦点
            if (textBox1.Text.Length < 7)//提货码七位
            {
                textBox1.Text += "6";
            }
            tihuomaresult = "请输入提货码";
            Form1.guanggaoreturntime = 0;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            this.label2.Focus();//获取焦点
            if (textBox1.Text.Length < 7)//提货码七位
            {
                textBox1.Text += "7";
            }
            tihuomaresult = "请输入提货码";
            Form1.guanggaoreturntime = 0;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            this.label2.Focus();//获取焦点
            if (textBox1.Text.Length < 7)//提货码七位
            {
                textBox1.Text += "8";
            }
            tihuomaresult = "请输入提货码";
            Form1.guanggaoreturntime = 0;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.label2.Focus();//获取焦点
            if (textBox1.Text.Length < 7)//提货码七位
            {
                textBox1.Text += "9";
            }
            tihuomaresult = "请输入提货码";
            Form1.guanggaoreturntime = 0;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.label2.Focus();//获取焦点
            if (textBox1.Text.Length < 7)//提货码七位
            {
                textBox1.Text += "0";
            }
            tihuomaresult = "请输入提货码";
            Form1.guanggaoreturntime = 0;
        }

        private void button4_Click(object sender, EventArgs e)//清除
        {
            this.label2.Focus();//获取焦点
            if (textBox1.Text.Length>0)
            {
                textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);//去除一个字符
            }
            tihuomaresult = "请输入提货码";
            Form1.guanggaoreturntime = 0;
        }

        private void button9_Click(object sender, EventArgs e)//确认提货
        {
            if (PLCHelper.nowStep != 0x00)
            {
                MessageBox.Show("机器正在运行中，请稍等","提示");
                return;
            }
            this.label2.Focus();//获取焦点
            if (textBox1.Text.Length == 7)//提货码小于七位
            {
                Form1.myTihuomastr = textBox1.Text;
                Form1.checktihuoma = true;//需要验证提货码
                tihuomaresult = "正在验证提货码";
            }
            else
            {
                tihuomaresult = "提货码长度不对";
            }
            Form1.guanggaoreturntime = 0;
        }

        #endregion

        private void tihuoma_Load(object sender, EventArgs e)
        {
            panelTest.Visible = false;
            tihuoma.tihuomaresult = "请输入提货码";
            print = PrintHelper.GetExample();
            pic_Erweima.Image = Image.FromFile(System.IO.Path.Combine(Form1.adimagesaddress, "erweima.jpg"));
        }

        private void label10_DoubleClick(object sender, EventArgs e)
        {
            Form1.needopensettingform = true;
        }

        #region Test Run

        private string imageFilePath;
        private int cargoWayNum;

        private void FileChose()
        {
            string filePath = Application.StartupPath + "\\TestImages";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = filePath;
            openFileDialog.Filter = "jpg|*.JPG";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                imageFilePath = openFileDialog.FileName;
                ShowIamge();
            }

        }

        private void ShowIamge()
        {
            picPrintImage.Image = Image.FromFile(imageFilePath);
            picPrintImage.SizeMode = PictureBoxSizeMode.Zoom;
            picPrintImage.BorderStyle = BorderStyle.None;

        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(cmbCargoWay.Text))
            {
                MessageBox.Show("请选择货道","提示");
            }
            else if (String.IsNullOrEmpty(imageFilePath))
            {
                MessageBox.Show("请选择印章图章","提示");
            }
            else
            {
                cargoWayNum = cmbCargoWay.SelectedIndex + 1;
                Form1.CallWorkingTest(cargoWayNum, imageFilePath);
                this.Dispose();
                this.Close();
            }
        }

        private void btnTry_Click(object sender, EventArgs e)
        {
            ErrorToken = true;
            panelTest.BackColor = Color.FromArgb(98, Color.White);
            panelTest.Visible = true;
        }

        private void btnPicChoice_Click(object sender, EventArgs e)
        {
            FileChose();
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            panelTest.Visible = false;
        }
        
        #endregion

        /// <summary>
        /// 故障报错提示
        /// </summary>
        /// <param name="type">故障类型</param>
        public static void ReportErrors(ErrorType type)
        {
            switch (type)
            {
                case ErrorType.machineError:
                    break;
                case ErrorType.pictureError:
                    break;
                case ErrorType.printerError:
                    break;
                case ErrorType.stockError:
                    break;
            }
        }
    }

    /// <summary>
    /// 错误类型
    /// </summary>
    public enum ErrorType
    {
        printerError,//打印机错误
        machineError,//机器错误
        pictureError,//印章图案错误
        stockError//库存
    }
}
