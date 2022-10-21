using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHJ
{
    class LogHelper
    {
        private List<string> writeLog = null;
        StreamWriter stream = null;
        private LogHelper()
        {
            writeLog= new List<string>();
        }
        private static volatile LogHelper _LogHelpeer = null;
        private static object LogHepler_Lock = new object();

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static LogHelper GetExample()
        {
            if (_LogHelpeer == null)
            {
                lock (LogHepler_Lock)
                {
                    if (_LogHelpeer == null)
                    {
                        _LogHelpeer = new LogHelper();
                    }
                }
            }
            return _LogHelpeer;
        }
        /// <summary>
        /// 写入Log
        /// </summary>
        /// <param name="log"></param>
        public void Log(string log)
        {
            if (!writeLog.Contains(log))
            {
                try
                {
                    writeLog.Add(log);
                    stream = new StreamWriter(Form1.logAddress, true, System.Text.Encoding.UTF8);
                    stream.WriteLine((log).PadRight(20) + "Date：" + DateTime.Now.ToString());
                    stream.Flush();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Log写入完成,释放写入记录
        /// </summary>
        public void LogWriteComplete()
        {
            stream = new StreamWriter(Form1.logAddress, true, System.Text.Encoding.UTF8);
            stream.WriteLine(("\r\n本次记录完成").PadRight(20) + DateTime.Now.ToString()+"\r\n\r\n");
            writeLog.Clear();
            stream.Close();
        }

    }
}
