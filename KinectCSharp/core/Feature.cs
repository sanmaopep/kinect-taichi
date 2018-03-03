using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectCSharp.core
{
    using Microsoft.Kinect;
    using KinectCSharp.util;

    class Feature
    {
        // 保存的信息
        public Int64 frameNum;
        public Skeleton skeleton;
        public bool ok;
        public JointAngle jointAngle;

        // 计算获得的信息
        public Feature()
        {
        }

        // 计算角度
        public void caculateAngle()
        {
            jointAngle = new JointAngle(skeleton);
        }

        public string print()
        {
            string ret = "";
            ret += "帧数：" + frameNum.ToString()  + "\n";
            ret += jointAngle.print();

            return ret;
        }


        // 二进制转化为byte
        public Feature(byte[] binaryData)
        {

        }

        // 获取二进制byte
        public byte[] getByte()
        {
            byte[] skeleton = Util.ObjectToBytes(this.skeleton);

            return skeleton;
        }
    }
}
