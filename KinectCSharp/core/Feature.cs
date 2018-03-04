using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectCSharp.core
{
    using Microsoft.Kinect;
    using KinectCSharp.util;
    using System.IO;

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

        // 克隆一个feature
        public Feature clone()
        {
            const int JOINT_TYPE_LEN = 20;

            Feature ret = new Feature();
            ret.frameNum = frameNum;
            ret.skeleton = new Skeleton();
            ret.ok = ok;

            for (int i = 0; i < JOINT_TYPE_LEN; i++)
            {
                Joint joint = skeleton.Joints[(JointType)i];
                ret.skeleton.Joints[(JointType)i] = joint;
            }
            return ret;
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

        // 解析byte
        public void parseByte(byte[] bytes)
        {
            if(bytes.Length == 0)
            {
                return;
            }
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;
            BinaryReader binaryReader = new BinaryReader(ms);

            const int JOINT_TYPE_LEN = 20;
            for (int i = 0; i < JOINT_TYPE_LEN; i++)
            {
                Joint joint = skeleton.Joints[(JointType)i];        
                SkeletonPoint skeletonPoint = new SkeletonPoint();

                skeletonPoint.X = binaryReader.ReadSingle();
                skeletonPoint.Y = binaryReader.ReadSingle();
                skeletonPoint.Z = binaryReader.ReadSingle();
                joint.TrackingState = (JointTrackingState)binaryReader.ReadInt32();
                joint.Position = skeletonPoint;

                skeleton.Joints[(JointType)i] = joint;
                frameNum = binaryReader.ReadInt64();
            }

            ok = true;
        }

        // 获取二进制（用于写文件）
        public byte[] getByte()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(ms);
            if (skeleton == null)
            {
                skeleton = new Skeleton();
            }

            const int JOINT_TYPE_LEN = 20;
            for(int i = 0;i < JOINT_TYPE_LEN; i++)
            {
                Joint joint = skeleton.Joints[(JointType)i];

                binaryWriter.Write(joint.Position.X);
                binaryWriter.Write(joint.Position.Y);
                binaryWriter.Write(joint.Position.Z);
                binaryWriter.Write((Int32)joint.TrackingState);
                binaryWriter.Write(frameNum);
            }

            return ms.GetBuffer();
        }
    }
}
