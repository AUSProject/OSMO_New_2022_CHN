using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SHJ
{
    public partial class Keyboard : Form
    {
        private static Keyboard _keyboard=null;
        public string inputValue;//输入的值
        public int maxNum;//合法值
        public string valueType;//数值类型

        private Keyboard()
        {
            InitializeComponent();
        }

        public static Keyboard GetKeyboard()
        {
            if (_keyboard == null)
                _keyboard = new Keyboard();
            return _keyboard;
        }

        private void button1_Click(object sender, EventArgs e)//1
        {
            inputValue += "1";
            Form1.guanggaoreturntime = 0;//返回广告页面计时清零
            textBox1.Text = inputValue;
        }

        private void button2_Click(object sender, EventArgs e)//2
        {
            inputValue += "2";
            Form1.guanggaoreturntime = 0;//返回广告页面计时清零
            textBox1.Text = inputValue;
        }

        private void button3_Click(object sender, EventArgs e)//3
        {
            inputValue += "3";
            Form1.guanggaoreturntime = 0;//返回广告页面计时清零
            textBox1.Text = inputValue;
        }

        private void button8_Click(object sender, EventArgs e)//4
        {
            inputValue += "4";
            Form1.guanggaoreturntime = 0;//返回广告页面计时清零
            textBox1.Text = inputValue;
        }

        private void button7_Click(object sender, EventArgs e)//5
        {
            inputValue += "5";
            Form1.guanggaoreturntime = 0;//返回广告页面计时清零
            textBox1.Text = inputValue;
        }

        private void button6_Click(object sender, EventArgs e)//6
        {
            inputValue += "6";
            Form1.guanggaoreturntime = 0;//返回广告页面计时清零
            textBox1.Text = inputValue;
        }

        private void button12_Click(object sender, EventArgs e)//7
        {
            inputValue += "7";
            Form1.guanggaoreturntime = 0;//返回广告页面计时清零
            textBox1.Text = inputValue;
        }

        private void button11_Click(object sender, EventArgs e)//8
        {
            inputValue += "8";
            Form1.guanggaoreturntime = 0;//返回广告页面计时清零
            textBox1.Text = inputValue;
        }

        private void button10_Click(object sender, EventArgs e)//9
        {
            inputValue += "9";
            Form1.guanggaoreturntime = 0;//返回广告页面计时清零
            textBox1.Text = inputValue;
        }

        private void button5_Click(object sender, EventArgs e)//0
        {
            //if (inputValue.Length > 0)
            {
                inputValue += "0";
                Form1.guanggaoreturntime = 0;//返回广告页面计时清零
                textBox1.Text = inputValue;
            }
        }

        private void button9_Click(object sender, EventArgs e)//确定
        {
            Form1.guanggaoreturntime = 0;//返回广告页面计时清零            
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)//清除
        {
            Form1.guanggaoreturntime = 0;//返回广告页面计时清零
            if (inputValue.Length > 0)
            {
                inputValue = inputValue.Substring(0, inputValue.Length - 1);//去除一个字符
                textBox1.Text = inputValue;
            }
        }

        private void button13_Click(object sender, EventArgs e)//点
        {
            if (inputValue.Length == 0)
            {
                inputValue += "0.";
                Form1.guanggaoreturntime = 0;//返回广告页面计时清零
                textBox1.Text = inputValue;
            }
            else if (!inputValue.Contains(".") && inputValue.Length > 0)
            {
                inputValue += ".";
                Form1.guanggaoreturntime = 0;//返回广告页面计时清零
                textBox1.Text = inputValue;
            }
        }
        
        private void Keyboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (inputValue.Length == 0)
            {
                inputValue = "0";
            }
            else if(inputValue[inputValue.Length-1]=='.')//如果最后一位是小数
            {
                inputValue = inputValue.Substring(0, inputValue.Length - 1);//去除一个字符
            }

            if (inputValue.Contains(".") && valueType == "Int")//检查数值类型
            {
                MessageBox.Show("不能为小数");
                e.Cancel = true;
            }
            else
            {
                double value = Convert.ToDouble(inputValue);
                if (value > maxNum)//检查数值合法性
                {
                    MessageBox.Show("数值过大");
                    inputValue = maxNum.ToString();
                }
                else if(!inputValue.Contains("."))
                {
                    inputValue = inputValue == "0" ? "0" : inputValue.TrimStart('0');
                }
                this.DialogResult = DialogResult.OK;
            }
        }

        private void Keyboard_Load(object sender, EventArgs e)
        {
            textBox1.Text = inputValue;
        }
        
    }
}
