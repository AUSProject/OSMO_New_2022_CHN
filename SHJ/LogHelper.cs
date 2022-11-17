﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SHJ
{
    public class LogHelper
    {
        private LogHelper()
        {
            doc = new XmlDocument();
            Osmo = doc.CreateElement("OSMO");
        }

        private static LogHelper _LogHepler = null;
        private XmlDocument doc = null;
        private XmlElement Osmo = null;
        private XmlElement _ShipCord = null;//出货记录节点
        private XmlElement _taskStep = null;//任务步骤节点
        private string filePath = null;//日志保存路径
        public List<string> logMsgs = new List<string>();

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static LogHelper GetLogHelperExamlpe()
        {
            if (_LogHepler == null)
                _LogHepler = new LogHelper();
            return _LogHepler;
        }

        /// <summary>
        /// 创建运行日志
        /// </summary>
        /// <param name="oderId">订单号</param>
        /// <param name="tradeName">商品名称</param>
        /// <param name="price">价格</param>
        /// <param name="paid">实付金额</param>
        /// <param name="largoNum">货道号</param>
        /// <param name="logName">日志保存名称,无后缀</param>
        public void CreateRunningLog(string oderId, string tradeName, string price, string paid, string largoNum, string logName)
        {
            WriteOderInfo(oderId, tradeName, price, paid);
            _ShipCord = doc.CreateElement("出货记录");
            _ShipCord.SetAttribute("出货时间", DateTime.Now.ToString());
            _ShipCord.SetAttribute("货道号", largoNum);
            filePath = System.IO.Path.Combine(Form1.myTihuomastr,logName) + ".xml";
        }

        /// <summary>
        /// 保存运行日志
        /// </summary>
        public void SaveRunningLog()
        {
            WriteStepLog(StepType.运行结束, "程序运行结束");
            try
            {
                Osmo.AppendChild(_ShipCord);
                doc.AppendChild(Osmo);
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;
                XmlWriter writer = XmlWriter.Create(filePath, settings);
                doc.WriteTo(writer);
                writer.Flush();
                writer.Close();
                doc.RemoveAll();
            }
            catch
            {
            }
        }

        /// <summary>
        /// 写入步骤信息
        /// </summary>
        /// <param name="msgs"></param>
        private void WriteMsgLog(List<string> msgs)
        {
            XmlElement msgElement = null;
            for (int i = 0; i < msgs.Count; i++)
            {
                msgElement = doc.CreateElement("过程" + (i + 1).ToString());
                msgElement.InnerText = msgs[i];
                _taskStep.AppendChild(msgElement);
            }
            logMsgs.Clear();
        }

        /// <summary>
        /// 写入步骤信息
        /// </summary>
        /// <param name="msg"></param>
        private void WriteMsgLog(string msg)
        {
            XmlElement msgElement = null;
            msgElement = doc.CreateElement("过程1");
            msgElement.InnerText = msg;
            _taskStep.AppendChild(msgElement);
        }
        

        /// <summary>
        /// 写入任务步骤
        /// </summary>
        /// <param name="stepName">步骤名称</param>
        public void WriteStepLog(StepType setpType)
        {
            _taskStep = doc.CreateElement("运行步骤");
            _taskStep.SetAttribute("步骤名称", setpType.ToString());
            WriteMsgLog(logMsgs);
            _ShipCord.AppendChild(_taskStep);
        }

        /// <summary>
        /// 写入任务步骤
        /// </summary>
        /// <param name="stepName">步骤名称</param>
        /// <param name="msgs">步骤信息</param>
        public void WriteStepLog(StepType setpType, string msg)
        {
            _taskStep = doc.CreateElement("运行步骤");
            _taskStep.SetAttribute("步骤名称", setpType.ToString());
            WriteMsgLog(msg);
            _ShipCord.AppendChild(_taskStep);
        }

        /// <summary>
        /// 写入任务步骤
        /// </summary>
        /// <param name="stepName">步骤名称</param>
        /// <param name="msgs">日志信息</param>
        public void WriteStepLog(StepType setpType,List<string> msgs)
        {
            _taskStep = doc.CreateElement("运行步骤");
            _taskStep.SetAttribute("步骤名称", setpType.ToString());
            WriteMsgLog(msgs);
            _ShipCord.AppendChild(_taskStep);
        }

        /// <summary>
        /// 写入订单信息
        /// </summary>
        /// <param name="oderId">订单号</param>
        /// <param name="tradeName">商品名称</param>
        /// <param name="price">商品价格</param>
        /// <param name="paid">实付金额</param>
        private void WriteOderInfo(string oderId, string tradeName, string price, string paid)
        {
            XmlElement oderInfo = doc.CreateElement("订单信息");
            oderInfo.SetAttribute("订单号", oderId);

            XmlElement nameElement = doc.CreateElement("商品名称");
            nameElement.InnerText = tradeName;

            XmlElement priceElement = doc.CreateElement("商品价格");
            priceElement.InnerText = price;

            XmlElement paidElement = doc.CreateElement("实付金额");
            paidElement.InnerText = paid;

            oderInfo.AppendChild(nameElement);
            oderInfo.AppendChild(priceElement);
            oderInfo.AppendChild(paidElement);
            Osmo.AppendChild(oderInfo);
        }
    }
    /// <summary>
    /// 任务步骤类型
    /// </summary>
    public enum StepType
    {
        货道检测,
        印章图案检查,
        货道出货,
        印面打印,
        取盖子,
        安装印面,
        装配盒子,
        运行中断,
        运行结束
    }
}
