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

        public string[] PicTypes = new string[] { "Png", "Jpeg", "Bmp", "Gif" };
        public string[] WatermarkTypes = new string[] { "None", "DateTime", "TimeAndNum" };
        public CameraPara Camera1 { get; set; }
        public CameraPara Camera2 { get; set; }

        public VideoCaptureDevice VideoDevice1 { get; set; }//摄像源1
        public VideoCaptureDevice VideoDevice2 { get; set; }//摄像源2
        #region CameraPara

        public string WatermarkType { get; set; }//水印类型
        public string CameraName { get; set; }//像机名称
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
            Camera1 = new CameraPara() { WaterFontSzie = 6, WatermarkType = "None", CapabilitieItem = 0, PicType = "Jpeg", CameraName = "usb camera" };
            Camera2 = new CameraPara() { WaterFontSzie = 6, WatermarkType = "None", CapabilitieItem = 0, PicType = "Jpeg", CameraName = "usb camera" };
        }

        /// <summary>
        /// 初始化相机参数
        /// </summary>
        public void IniCameraPara()
        {
            Form1.IniWriteValue("Camera1", "watermarkType", Camera1.WatermarkType, Form1.cameraParaFile);
            Form1.IniWriteValue("Camera1", "cameraName", Camera1.CameraName, Form1.cameraParaFile);
            Form1.IniWriteValue("Camera1", "capabilitieItem", Camera1.CapabilitieItem.ToString(), Form1.cameraParaFile);
            Form1.IniWriteValue("Camera1", "fontSize", Camera1.WaterFontSzie.ToString(), Form1.cameraParaFile);
            Form1.IniWriteValue("Camera1", "picType", Camera1.PicType, Form1.cameraParaFile);
            
            Form1.IniWriteValue("Camera2", "watermarkType", Camera2.WatermarkType, Form1.cameraParaFile);
            Form1.IniWriteValue("Camera2", "cameraName", Camera2.CameraName, Form1.cameraParaFile);
            Form1.IniWriteValue("Camera2", "capabilitieItem", Camera2.CapabilitieItem.ToString(), Form1.cameraParaFile);
            Form1.IniWriteValue("Camera2", "fontSize", Camera2.WaterFontSzie.ToString(), Form1.cameraParaFile);
            Form1.IniWriteValue("Camera2", "picType", Camera2.PicType, Form1.cameraParaFile);
        }

        /// <summary>
        /// 初始化像机源
        /// </summary>
        public void IniCamera()
        {
            try
            {
                VideoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (VideoDevices.Count == 0)
                {
                    return;
                }
                else
                {
                    for (int i = 0; i < VideoDevices.Count; i++)//第一个相机
                    {
                        if (VideoDevices[i].Name == Camera1.CameraName)
                        {
                            Camera1.VideoDevice = new VideoCaptureDevice(VideoDevices[i].MonikerString);
                            Camera1.VideoDevice.VideoResolution = Camera1.VideoDevice.VideoCapabilities[Camera1.CapabilitieItem];//设置分辨率
                            break;
                        }
                        else if (i == VideoDevices.Count)
                        {
                            Camera1.VideoDevice = new VideoCaptureDevice(VideoDevices[0].MonikerString);
                            Camera1.VideoDevice.VideoResolution = Camera1.VideoDevice.VideoCapabilities[Camera1.CapabilitieItem];
                            Camera1.CameraName = VideoDevices[0].Name;
                            break;
                        }
                    }
                    for (int i = 0; i < VideoDevices.Count; i++)//第二个相机
                    {
                        if (VideoDevices[i].Name == Camera2.CameraName && VideoDevices[i].Name!=Camera1.CameraName)
                        {
                            Camera2.VideoDevice = new VideoCaptureDevice(VideoDevices[i].MonikerString);
                            Camera2.VideoDevice.VideoResolution = Camera2.VideoDevice.VideoCapabilities[Camera2.CapabilitieItem];
                            break;
                        }
                        else if (i == VideoDevices.Count)
                        {
                            Camera2.VideoDevice = new VideoCaptureDevice(VideoDevices[0].MonikerString);
                            Camera2.VideoDevice.VideoResolution = Camera2.VideoDevice.VideoCapabilities[Camera2.CapabilitieItem];
                            Camera2.CameraName = VideoDevices[0].Name;
                            break;
                        }
                    }
                }
            }
            catch { }
        }
    }
    public class CameraPara
    {
        public VideoCaptureDevice VideoDevice { get; set; }//摄像源
        public string WatermarkType { get; set; }//水印类型
        public string CameraName { get; set; }//像机名称
        public int CapabilitieItem { get; set; }//分辨率相对位置
        public int WaterFontSzie { get; set; }//水印字体大小
        public ImageFormat picType;
        public string PicType
        {
            get { return picType.ToString(); }
            set {
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

    }
}
