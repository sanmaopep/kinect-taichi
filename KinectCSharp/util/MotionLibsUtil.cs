using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectCore.model;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Media;
using KinectCore.core;

namespace KinectCore.util
{
    // 动作库操作工具集
    public class MotionLibsUtil
    {
        public static SingleMotionModel[] parseJson(string jsonStr)
        {
            SingleMotionModel[] singleMotionModels = JsonConvert.DeserializeObject<SingleMotionModel[]>(jsonStr);
            return singleMotionModels;
        }
        
        public static SingleMotionModel[]  parseFromFile(string motionLibPath,string motionDescriptionFile = "motions.json")
        {
            string filePath = motionLibPath + "/" + motionDescriptionFile;
            string text = File.ReadAllText(filePath);
            return parseJson(text);
        }


        public static ImageSource getPicSource(string motionLibPath,SingleMotionModel singleMotionModel)
        {
            string filePath = motionLibPath + "/" + singleMotionModel.data;
            KinectControl kinectControl = new KinectControl();
            kinectControl.loadFramesFromFile(filePath, 1);
            return kinectControl.featureBuffer[0].rgbImage.imageSource;
        }
    }
}
