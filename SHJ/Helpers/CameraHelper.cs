using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Video.DirectShow;
using System.Drawing.Imaging;

namespace SHJ
{
    public class CameraHelper
    {

        public FilterInfoCollection VideoDevices;//摄像设备

        private static CameraHelper _cameraHelper = null;

        public string[] picTypeList= new string[] { "Png", "Jpeg", "Bmp", "Gif" };
        public string[] watermarkTypeList = new string[] { "None", "DateTime", "TimeAndNum" };

        public VideoCaptureDevice VideoDevice1 { get; set; }//摄像源1
        public VideoCaptureDevice VideoDevice2 { get; set; }//摄像源2

        #region CameraPara

        public string Camera1Name { get; set; }//相机1名称
        public string Camera2Name { get; set; }//相机2名称
        public string WatermarkType { get; set; }//水印类型
        public int CapabilitieItem { get; set; }//分辨率相对位置
        public int WaterFontSzie { get; set; }//水印字体大小
        public ImageFormat picType;
        public string PicType
        {
            get { return picType.ToString(); }
            set
            {
                switch (value)//图片格式
                {
                    case "Jpeg":
                        picType = ImageFormat.Jpeg;
                        break;
                    case "Bmp":
                        picType = ImageFormat.Bmp;
                        break;
                    case "Png":
                        picType = ImageFormat.Png;
                        break;
                    case "Gif":
                        picType = ImageFormat.Gif;
                        break;
                    default:
                        picType = ImageFormat.Jpeg;
                        break;
                }
            }
        }

        #endregion

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static CameraHelper GetCameraExample()
        {
            if (_cameraHelper == null)
                _cameraHelper = new CameraHelper();
            return _cameraHelper;
        }

        private CameraHelper()
        {
            IniCameraPara();
        }
        
        /// <summary>
        /// 写入初始化相机参数
        /// </summary>
        public void WriteIniCameraPara()
        {
            Form1.IniWriteValue("CameraPara", "watermarkType", WatermarkType, Form1.cameraParaFile);
            Form1.IniWriteValue("CameraPara", "capabilitieItem", CapabilitieItem.ToString(), Form1.cameraParaFile);
            Form1.IniWriteValue("CameraPara", "fontSize", WaterFontSzie.ToString(), Form1.cameraParaFile);
            Form1.IniWriteValue("CameraPara", "picType", PicType, Form1.cameraParaFile);
            Form1.IniWriteValue("CameraPara", "Camera1Name","usb camera", Form1.cameraParaFile);
            Form1.IniWriteValue("CameraPara", "Camera2Name","usb camera", Form1.cameraParaFile);
            
        }
        
        /// <summary>
        /// 获取像机源配制
        /// </summary>
        public void GetCameraDevices()
        {
            try
            {
                VideoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                ReadCameraPara();
                if (VideoDevices.Count == 0)
                {
                    return;
                }
                else
                {
                    for (int i = 0; i < VideoDevices.Count; i++)//获取第一个相机源
                    {
                        if (VideoDevices[i].Name == Camera1Name)
                        {
                            VideoDevice1 = new VideoCaptureDevice(VideoDevices[i].MonikerString);
                            VideoDevice1.VideoResolution = VideoDevice1.VideoCapabilities[CapabilitieItem];//设置分辨率
                            VideoDevices.RemoveAt(i);
                            break;
                        }
                        else if (i == VideoDevices.Count-1)//如果没有相同名称相机则选择第一个摄像源
                        {
                            VideoDevice1 = new VideoCaptureDevice(VideoDevices[0].MonikerString);
                            VideoDevice1.VideoResolution = VideoDevice1.VideoCapabilities[CapabilitieItem];
                            Camera1Name = VideoDevices[0].Name;
                            VideoDevices.RemoveAt(0);
                            break;
                        }
                    }
                    for (int i = 0; i < VideoDevices.Count; i++)//获取第二个相机源
                    {
                        if (VideoDevices[i].Name == Camera2Name && VideoDevices[i].Name!=Camera1Name)
                        {
                            VideoDevice2 = new VideoCaptureDevice(VideoDevices[i].MonikerString);
                            VideoDevice2.VideoResolution = VideoDevice2.VideoCapabilities[CapabilitieItem];
                            break;
                        }
                        else if(i== VideoDevices.Count-1)
                        {
                            VideoDevice2 = new VideoCaptureDevice(VideoDevices[0].MonikerString);
                            VideoDevice2.VideoResolution = VideoDevice2.VideoCapabilities[CapabilitieItem];
                            Camera2Name = VideoDevices[0].Name;
                            break;
                        }
                    }
                }
            }
            catch { }
            finally
            {
                VideoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            }
        }

        private void IniCameraPara()
        {
            WaterFontSzie = 6;
            WatermarkType = "None";
            CapabilitieItem = 0;
            PicType = "Jpeg";
        }

        private void ReadCameraPara()
        {
            try
            {
                Camera1Name = Form1.IniReadValue("CameraPara", "Camera1Name", Form1.cameraParaFile);
                Camera2Name = Form1.IniReadValue("CameraPara", "Camera2Name", Form1.cameraParaFile);
                WaterFontSzie = int.Parse(Form1.IniReadValue("CameraPara", "fontSize", Form1.cameraParaFile));//字体大小
                WatermarkType = Form1.IniReadValue("CameraPara", "watermarkType", Form1.cameraParaFile);//水印类型
                CapabilitieItem = int.Parse(Form1.IniReadValue("CameraPara", "capabilitieItem", Form1.cameraParaFile));//分辨率
                PicType = Form1.IniReadValue("CameraPara", "picType", Form1.cameraParaFile);
            }
            catch { }
        }
    }
}
