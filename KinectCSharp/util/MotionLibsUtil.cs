using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectCore.model;
using Newtonsoft.Json;
using System.IO;

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
        
        public static SingleMotionModel[]  parseFromFile(string filePath)
        {
            string text = File.ReadAllText(filePath);
            return parseJson(text);
        }
    }
}
