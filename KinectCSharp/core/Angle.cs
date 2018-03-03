using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectCSharp.core
{
    using Microsoft.Kinect;
    using MathNet.Numerics.LinearAlgebra.Double;
    using util;

    class JointAngle
    {
        // 人体骨架朝向机建立的三维坐标系
        private Skeleton skeleton;
        public Vector4 x, y, z;

        // 各个关节角度
        public float ElobwLeft;
        public float ElobwRight;
        public float KneeLeft;
        public float KneeRight;
        // public float WristLeft;
        // public float WristRight;
        // public float AnkleLeft;
        // public float AnkleRight;
        public float Head;  // 头歪不歪
        public float Spine; //弯腰
        public Vector4 ShoulderLeft;
        public Vector4 ShoulderRight;
        public Vector4 HipLeft;
        public Vector4 HipRight;

        public JointAngle(Skeleton skeleton)
        {
            this.skeleton = skeleton;
            caculateXYZ();

            ElobwLeft = getAngle(JointType.ElbowLeft, JointType.WristLeft, JointType.ShoulderLeft);
            ElobwRight = getAngle(JointType.ElbowRight, JointType.WristRight, JointType.ShoulderRight);
            KneeLeft = getAngle(JointType.KneeLeft, JointType.AnkleLeft, JointType.HipLeft);
            KneeRight = getAngle(JointType.KneeRight, JointType.AnkleRight, JointType.HipRight);
            Head = getAngle(JointType.ShoulderCenter, JointType.Head, JointType.Spine);
            Spine = getAngle(JointType.Spine, JointType.ShoulderCenter, JointType.HipCenter);
            ShoulderLeft = get3DAngle(JointType.ShoulderLeft, JointType.ElbowLeft);
            ShoulderRight = get3DAngle(JointType.ShoulderRight, JointType.ElbowRight);
            HipLeft = get3DAngle(JointType.HipLeft, JointType.KneeLeft);
            HipRight = get3DAngle(JointType.HipRight, JointType.KneeRight);
        }

        public string print()
        {
            string ret = "";
            ret += "左肘：" + ElobwLeft;
            ret += "右肘：" + ElobwRight;
            ret += "左膝：" + KneeLeft;
            ret += "右膝：" + KneeRight;
            ret += "左肩：" + ShoulderLeft.X + " " + ShoulderLeft.Y + " " + ShoulderLeft.Z;
            ret += "右肩：" + ShoulderRight.X + " " + ShoulderRight.Y + " " + ShoulderRight.Z;
            ret += "左腿：" + HipLeft.X + " " + HipLeft.Y + " " + HipLeft.Z;
            ret += "右腿：" + HipRight.X + " " + HipRight.Y + " " + HipRight.Z;

            return ret;
        }

        private void caculateXYZ()
        {
            // 计算x
            SkeletonPoint leftShoulder = skeleton.Joints[JointType.ShoulderLeft].Position;
            SkeletonPoint rightShoulder = skeleton.Joints[JointType.ShoulderLeft].Position;
            x.X = leftShoulder.X = rightShoulder.Y;
            x.Y = leftShoulder.Y - rightShoulder.Y;
            x.Z = leftShoulder.Z - rightShoulder.Z;
            // 计算y
            SkeletonPoint centerShuolder = skeleton.Joints[JointType.ShoulderCenter].Position;
            SkeletonPoint centerHip = skeleton.Joints[JointType.HipCenter].Position;
            y.X = centerShuolder.X - centerHip.X;
            y.Y = centerShuolder.Y - centerHip.Y;
            y.Z = centerShuolder.Z - centerHip.Z;

            // 计算x，y平面的法向量z,根据Crammer法则
            float bottomDeterminant = x.X * y.Y - x.Y * y.X;
            if (bottomDeterminant == 0)
            {
                bottomDeterminant = x.X * y.Z - x.Z * y.X;
                if (bottomDeterminant == 0)
                {
                    bottomDeterminant = x.Y * y.Z - x.Z * y.Y;
                    if (bottomDeterminant == 0)
                    {
                        // 坐标系计算错误
                        return;
                    }
                    z.X = 1;
                    z.Y = (x.Z * y.X - x.X * y.Z) / bottomDeterminant;
                    z.Z = (x.X * y.Y - y.X * x.Y) / bottomDeterminant;
                }
                else {
                    z.Y = 1;
                    z.X = (y.Y * x.Z - x.Y * y.Z) / bottomDeterminant;
                    z.Z = (x.Y * y.X - x.X * y.Y) / bottomDeterminant;
                }
            }
            else
            {
                z.Z = 1;
                z.X = (x.Y * y.Z - x.Z * y.Y) / bottomDeterminant;
                z.Y = (x.Z * y.X - x.X * y.Z) / bottomDeterminant;

            }
        }

        private Vector4 getVector(JointType before,JointType after)
        {
            Vector4 ret = new Vector4();
            ret.X = skeleton.Joints[before].Position.X - skeleton.Joints[after].Position.X;
            ret.Y = skeleton.Joints[before].Position.Y - skeleton.Joints[after].Position.Y;
            ret.Z = skeleton.Joints[before].Position.Z - skeleton.Joints[after].Position.Z;
            return ret;
        }

        private float getAngle(JointType curr,JointType before, JointType after)
        {
            return getAngle(getVector(curr, before), getVector(curr, after));
        }

        private float getAngle(Vector4 x, Vector4 y)
        {
            float len1 = x.X * x.X + x.Y * x.Y + x.Z * x.Z;
            float len2 = y.X * y.X + y.Y * y.Y + y.Z * y.Z;
            float mutiple = x.X * y.X + x.Y * y.Y + x.Z * y.Z;
            double cos = Util.toDouble(mutiple) / Math.Sqrt(Util.toDouble(len1 * len2));
            return Util.toFloat(Math.Acos(cos) * 180 / 3.14);
        }

        private Vector4 get3DAngle(JointType start,JointType end)
        {
            Vector4 bone = getVector(start, end);
            Vector4 ret = new Vector4();
            ret.X = getAngle(bone, x);
            ret.Y = getAngle(bone, y);
            ret.Z = getAngle(bone, z);
            return ret;
        }

    }
}
