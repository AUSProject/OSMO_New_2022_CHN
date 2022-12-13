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
            doc = new XmlDocument();
            Osmo = doc.CreateElement("OSMO");
        }

        private static LogHelper _LogHepler = null;
        private XmlDocument doc = null;
        private XmlElement Osmo = null;
        private XmlElement _ShipCord = null;//出货记录节点
        private XmlElement _taskStep = null;//任务步骤节点
        private string filePath = null;//日志保存路径

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
        /// <param name="largoNum">货道号</param>
        /// <param name="logName">日志名称</param>
        /// <returns>日志路径</returns>
        public string CreateRunningLog(string largoNum,string logName)
        {
            string path = Form1.logPath + "\\" + logName + "-" + DateTime.Now.ToString("MM-dd HH：mm：ss");
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }//添加日志文件夹
            _ShipCord = doc.CreateElement("出货记录");
            _ShipCord.SetAttribute("出货时间", DateTime.Now.ToString());
            _ShipCord.SetAttribute("货道号", largoNum);
            filePath = System.IO.Path.Combine(path,logName + ".xml");
            return path;
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
                Osmo.RemoveAll();
                _ShipCord.RemoveAll();
                _taskStep.RemoveAll();
            }
            catch
            {
            }
        }

        /// <summary>
        /// 写入步骤信息
        /// </summary>
        /// <param name="msgs"></param>
        private void WriteMsgLog(ref HashSet<string> msgs)
        {
            XmlElement msgElement = null;
            foreach (var item in msgs)
            {
                msgElement = doc.CreateElement("过程");
                msgElement.InnerText = item;
                _taskStep.AppendChild(msgElement);
            }
            msgs.Clear();
        }

        /// <summary>
        /// 写入步骤信息
        /// </summary>
        /// <param name="msg"></param>
        private void WriteMsgLog(string msg)
        {
            XmlElement msgElement = null;
            msgElement = doc.CreateElement("过程");
            msgElement.InnerText = msg;
            _taskStep.AppendChild(msgElement);
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
        public void WriteStepLog(StepType setpType,ref HashSet<string> msgs)
        {
            _taskStep = doc.CreateElement("运行步骤");
            _taskStep.SetAttribute("步骤名称", setpType.ToString());
            WriteMsgLog(ref msgs);
            _ShipCord.AppendChild(_taskStep);
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
        运行故障,
        运行结束
    }
}
