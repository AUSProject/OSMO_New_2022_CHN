using System;
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
            Osmo = new XmlDocument();
        }
        
        private static LogHelper _LogHepler = null;
        private XmlDocument Osmo = null;
        private XmlElement _ShipCord = null;//出货记录节点
        private XmlElement _taskStep = null;//任务步骤节点
        private string filePath = null;//日志保存路径
        public List<string> logMsgs = new List<string>();
        
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static LogHelper GetLogHelper()
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
        public void CreateRunningLog(string oderId,string tradeName,string price,string paid,string largoNum,string logName)
        {
            WriteOderInfo(oderId, tradeName, price, paid);
            _ShipCord = Osmo.CreateElement("出货记录");
            _ShipCord.SetAttribute("出货时间", DateTime.Now.ToString());
            _ShipCord.SetAttribute("货道号", largoNum);
            filePath = System.IO.Path.Combine(Form1.LogPath, logName) + ".xml";
        }

        /// <summary>
        /// 保存运行日志
        /// </summary>
        public void SaveRunningLog()
        {
            WriteStepLog(stepType.运行结束, "程序运行结束");
            Osmo.AppendChild(_ShipCord);
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;
                XmlWriter writer = XmlWriter.Create(filePath, settings);
                Osmo.WriteTo(writer);
                writer.Flush();
                writer.Close();
                Osmo.RemoveAll();
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
                msgElement = Osmo.CreateElement("信息", (i + 1).ToString());
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
            msgElement = Osmo.CreateElement("信息1");
            msgElement.InnerText = msg;
            _taskStep.AppendChild(msgElement);
        }

        /// <summary>
        /// 写入任务步骤
        /// </summary>
        /// <param name="stepName">步骤名称</param>
        public void WriteStepLog(stepType setpType)
        {
            _taskStep = Osmo.CreateElement("运行步骤");
            _taskStep.SetAttribute("步骤名称", setpType.ToString());
            WriteMsgLog(logMsgs);
            _ShipCord.AppendChild(_taskStep);
        }

        /// <summary>
        /// 写入任务步骤
        /// </summary>
        /// <param name="stepName">步骤名称</param>
        /// <param name="msgs">步骤信息</param>
        public void WriteStepLog(stepType setpType, string msg)
        {
            _taskStep = Osmo.CreateElement("运行步骤");
            _taskStep.SetAttribute("步骤名称", setpType.ToString());
            WriteMsgLog(msg);
            _ShipCord.AppendChild(_taskStep);
        }

        /// <summary>
        /// 写入订单信息
        /// </summary>
        /// <param name="oderId">订单号</param>
        /// <param name="tradeName">商品名称</param>
        /// <param name="price">商品价格</param>
        /// <param name="paid">实付金额</param>
        private void WriteOderInfo(string oderId,string tradeName,string price,string paid)
        {
            XmlElement oderInfo = Osmo.CreateElement("订单信息");
            oderInfo.SetAttribute("订单号", oderId);

            XmlElement nameElement = Osmo.CreateElement("商品名称");
            nameElement.InnerText = tradeName;

            XmlElement priceElement = Osmo.CreateElement("商品价格");
            priceElement.InnerText = price;

            XmlElement paidElement = Osmo.CreateElement("实付金额");
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
    public enum stepType
    {
        货道检查,
        打印机自检,
        机器自检,
        印章图案检查,
        复位,
        货道出货,
        放印面,
        取盖子,
        印章打印中,
        取印面,
        安装印面,
        组装印盒,
        成品出货,
        确认出货,
        运行中断,
        运行结束
    }
}
