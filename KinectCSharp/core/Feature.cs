using System;
using System.Linq;

namespace KinectCore.core
{
    using Microsoft.Kinect;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class Feature
    {
        // 保存的信息
        public Int64 frameNum;
        public Skeleton skeleton;
        public bool ok;
        public JointAngle jointAngle;
        public RGBImage rgbImage;

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

            ret.rgbImage = new RGBImage();
            ret.rgbImage.imageSource = rgbImage.imageSource.Clone();
            ret.rgbImage.imageSource.Freeze();
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
            if (jointAngle != null)
            {
                ret += jointAngle.print();
            }
            return ret;
        }

        // 计算欧氏距离
        public static double EDistance(Feature a, Feature b)
        {
            double sum = JointAngle.diffAngle(a.jointAngle, b.jointAngle);
            // 不用计算平方就可以比大小，所以不用Math.sqrt
            if (sum == 0)
            {
                return sum;
            }
            return Math.Sqrt(sum);
        }

        /**
         * 关于二进制数据的格式
         * -------------20个关节重复-----------
         * float 关节X坐标
         * float 关节Y坐标
         * float 关节Z坐标
         * Int32 关节状态
         * -----------------------------------
         * Int64 当前帧数
         * -----------------------------------
         * Int32 二进制数据长度
         * byte 前景的二进制数据
         * */

        // 解析byte(true代表解析没结束)
        public bool parseFromStream(Stream stream)
        {
            BinaryReader binaryReader = new BinaryReader(stream);

            skeleton = new Skeleton();
            rgbImage = new RGBImage();

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
            }
            frameNum = binaryReader.ReadInt64();
            int count = binaryReader.ReadInt32();
            byte[] data = binaryReader.ReadBytes(count);
            rgbImage.ParseFromBytes(data);
            rgbImage.imageSource.Freeze();

            ok = true;
            return true;
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
            }
            binaryWriter.Write(frameNum);

            byte[] bytes = rgbImage.getBuffer();
            binaryWriter.Write(bytes.Count());
            binaryWriter.Write(bytes);

            return ms.GetBuffer();
        }
    }

    // AR显示
    public class RGBImage
    {
        // 直接存取rawPixelData会造成浪费大量空间，需要进行jpeg有损数据压缩！
        public BitmapSource imageSource;

        public void ParseFromBytes(byte[] bytes)
        {
            using(MemoryStream ms = new MemoryStream(bytes))
            {
                // jpeg数据解码
                BitmapDecoder decoder = new JpegBitmapDecoder(ms, 
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.OnLoad);
                imageSource = decoder.Frames[0];
            }
        }

        // 从colorFrame进行解析
        public void ParsePixelData(int width,int height,byte[] pixelData, int bytesPerPixel)
        {
            // 解析Raw数据
            imageSource = BitmapSource.Create(
                        width,
                        height,
                        96.0,
                        96.0,
                        PixelFormats.Bgr32,
                        null,
                        pixelData,
                        width * bytesPerPixel);
        }

        public byte[] getBuffer()
        {
            MemoryStream ms = new MemoryStream();
            
            // 解析为jpeg压缩数据
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageSource));
            encoder.Save(ms);
            
            return ms.GetBuffer();
        }
    }
}


