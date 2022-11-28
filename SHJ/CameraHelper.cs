﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Video.DirectShow;
using System.Drawing.Imaging;

namespace SHJ
{
    class CameraHelper
    {
      
        public static FilterInfoCollection _VideoDevices;//摄像设备
        public static VideoCaptureDevice VideoDevice=null;//捕获设备源

        public static string _CameraName = "usb camera";//摄像机名称
        public static int videoCapabilitieItem=0;//分辨率
        public static string watermarkType="None";//水印类型 None,DateTime,TimeAndNum
        public static ImageFormat imageExt = ImageFormat.Jpeg;//图片格式
        public static int fontSize = 6;//水印字段大小

        public static string[] PicType = new string[] {"Png", "Jpeg", "Bmp", "Gif" };
        public static string[] watermarkTypes = new string[] { "None", "DateTime", "TimeAndNum" };

        /// <summary>
        /// 打开摄像头
        /// </summary>
        /// <returns>是否打开成功</returns>
        public static bool IniCamera()
        {
            bool result = false;
            _VideoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (_VideoDevices.Count == 0)
            {
                result=false;
            }
            else
            {
                if(VideoDevice!=null)//如果已经获取到设置
                {
                    result = true;
                    return result;
                }
                for (int i = 0; i < _VideoDevices.Count; i++)
                {
                    if (_VideoDevices[i].Name == _CameraName)
                    {
                        try
                        {
                            VideoDevice = new VideoCaptureDevice(_VideoDevices[i].MonikerString);
                            result = true;
                            break;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return result;
        }
    }
}
