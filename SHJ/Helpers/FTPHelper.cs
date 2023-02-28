using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SHJ
{
    public class FTPHelper
    {
        private int BUFFSIZE = 1024 * 2;
        private string ServerIP { get; set; }
        private string ServerPort { get; set; }
        private string UserName { get; set; }
        private string PassWord { get; set; }
        private string Url { get; set; }
        private Action CompeteUpload = null;
        private Action<string> FailUpload = null;
        private FtpWebRequest reqFTP;
        private FileStream fs;
        private Stream strm;

        /// <summary>
        /// 设置FTP属性
        /// </summary>
        /// <param name="ip">服务器IP</param>
        /// <param name="user">用户名</param>
        /// <param name="pwd">密码</param>
        public FTPHelper(string ip, string user="", string pwd="")
        {
            this.ServerIP = ip;
            this.UserName = user;
            this.PassWord = pwd;
            this.Url = "ftp://" + this.ServerIP + this.ServerPort + "/";
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="localPath">本地文件绝对路径</param>
        /// <param name="relativePath">服务器相对路径</param>
        public void Upload(string localPath, string relativePath="")
        {
            try
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    UploadFile(localPath, relativePath);
                });
                task.ContinueWith(t =>
                {
                    if (task.Exception == null)
                    {
                        if (this.CompeteUpload != null)
                            this.CompeteUpload();
                    }
                    else if (this.FailUpload != null)
                        this.FailUpload(task.Exception.Message);
                });
            }
            catch (Exception ex)
            {
                if (this.FailUpload != null)
                    this.FailUpload(ex.Message);
            }
            finally
            {
                Dispose();
            }
        }

        private void UploadFile(string localPath, string relativePath)
        {
            reqFTP = null;
            fs = null;
            strm = null;
            try
            {
                FileInfo fileInfo = new FileInfo(localPath);
                string uri = this.Url + relativePath + "/" + Path.GetFileName(localPath);
                if (!CheckFilePath(relativePath))
                {
                    FtpMakeDir(relativePath);
                }
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                reqFTP.Credentials = new NetworkCredential(this.UserName, this.PassWord);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = false;
                reqFTP.ContentLength = fileInfo.Length;
                byte[] buff = new byte[BUFFSIZE];
                int contentlen = 0;
                fs = fileInfo.OpenRead();
                strm = reqFTP.GetRequestStream();
                while ((contentlen = fs.Read(buff, 0, buff.Length)) != 0)
                {
                    strm.Write(buff, 0, contentlen);
                }
                if (this.CompeteUpload != null)
                    this.CompeteUpload();
            }
            finally
            {
                Dispose();
            }
        }

        /// <summary>
        /// 释放FTP
        /// </summary>
        private void Dispose()
        {
            if (reqFTP != null)
            {
                reqFTP.Abort();
                reqFTP = null;
            }
            if (fs != null)
            {
                fs.Close();
                fs = null;
            }
            if (strm != null)
            {
                strm.Close();
                strm = null;
            }
            
        }

        /// <summary>
        /// 检测服务器是否存在该文件夹
        /// </summary>
        /// <param name="localFile"></param>
        /// <returns></returns>
        private bool CheckFilePath(string localFile)
        {
            bool flag = true;
            try
            {
                reqFTP = (FtpWebRequest)WebRequest.Create(Url + localFile);
                reqFTP.Credentials = new NetworkCredential(UserName, PassWord);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
            }
            catch (Exception)
            {
                reqFTP.Abort();
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 为服务器创建文件夹
        /// </summary>
        /// <param name="localFile"></param>
        /// <returns></returns>
        private bool FtpMakeDir(string localFile)
        {
            reqFTP = (FtpWebRequest)WebRequest.Create(Url + localFile);
            reqFTP.Credentials = new NetworkCredential(UserName, PassWord);
            reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
            try
            {
                reqFTP.UsePassive = true;
                reqFTP.UseBinary = false;
                reqFTP.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
            }
            catch (Exception)
            {
                reqFTP.Abort();
                return false;
            }
            reqFTP.Abort();
            return true;
        }

    }
}
