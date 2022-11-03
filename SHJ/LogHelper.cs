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
            AddStep();
            Osmo = new XmlDocument();
        }

        public List<string> logMsgs = new List<string>();

        private static LogHelper _LogHepler = null;
        private XmlDocument Osmo = null;
        private XmlElement _ShipCord = null;//出货记录节点
        private XmlElement _taskStep = null;//任务步骤节点
        Dictionary<string,string> steps = new Dictionary<string,string>();//任务步骤
        private string filePath = null;//日志保存路径
        
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
        /// <param name="logName">日志保存名称,无后缀</param>
        public void CreateRunningLog(string oderId,string tradeName,string price,string paid,string logName)
        {
            WriteOderInfo(oderId, tradeName, price, paid);
            _ShipCord = Osmo.CreateElement("出货记录");
            _ShipCord.SetAttribute("出货时间", DateTime.Now.ToString());
            filePath = System.IO.Path.Combine(Form1.LogPath, logName) + ".xml";
        }

        /// <summary>
        /// 保存运行日志
        /// </summary>
        public void SaveRunningLog()
        {
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
        }

        /// <summary>
        /// 写入任务步骤
        /// </summary>
        /// <param name="stepName">步骤名称</param>
        /// <param name="msgs">步骤信息</param>
        public void WriteStepLog(stepType setpType,List<string> msgs)
        {
            _taskStep = Osmo.CreateElement(setpType.ToString());
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

        /// <summary>
        /// 添加任务步骤
        /// </summary>
        private void AddStep()
        {
            steps.Add("打印机自检", "步骤一");
            steps.Add("机器自检", "步骤二");
            steps.Add("印章图案检查", "步骤三");
            steps.Add("复位", "步骤四");
            steps.Add("放印面", "步骤五");
            steps.Add("取盒子", "步骤六");
            steps.Add("取盖子", "步骤七");
            steps.Add("印章打印中", "步骤八");
            steps.Add("取印面", "步骤九");
            steps.Add("安装印面", "步骤十");
            steps.Add("组装印盒", "步骤十一");
            steps.Add("成品出货", "步骤十二");
            steps.Add("确认出货", "步骤十三");
        }
    }
    /// <summary>
    /// 任务步骤类型
    /// </summary>
    public enum stepType
    {
        打印机自检,
        机器自检,
        印章图案检查,
        复位,
        放印面,
        取盖子,
        印章打印中,
        取印面,
        安装印面,
        组装印盒,
        成品出货,
        确认出货
    }
}
