using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectCore.model
{
    // 一个动作的模型
    public class SingleMotionModel
    {
        public string title; // 动作标题
        public string description;  // 动作描述
        public string data; // 二进制数据地址
        public SingleDetailDescription[] details;   // 动作细节描述
    }

    // 动作中细节描述的模型
    public class SingleDetailDescription
    {
        public int from;    // 开始帧
        public int to;  // 结束帧
        public string description;  // 对这几帧的描述
    }
}
