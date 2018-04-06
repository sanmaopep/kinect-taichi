using System;
using System.Linq;

namespace KinectCSharp.core
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
        public BackgroundRemoved backgroundRemoved;

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

            ret.backgroundRemoved = new BackgroundRemoved();
            ret.backgroundRemoved.imageSource = backgroundRemoved.imageSource.Clone();
            ret.backgroundRemoved.imageSource.Freeze();
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
            backgroundRemoved = new BackgroundRemoved();

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
            backgroundRemoved.ParseFromBytes(data);
            backgroundRemoved.imageSource.Freeze();

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

            byte[] bytes = backgroundRemoved.getBuffer();
            binaryWriter.Write(bytes.Count());
            binaryWriter.Write(bytes);

            return ms.GetBuffer();
        }
    }

    // 移除背景图片
    public class BackgroundRemoved
    {
        // 直接存取rawPixelData会造成浪费大量空间，需要进行png数据压缩！
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

        public void ParseRawData(int width,int height,byte[] rawPixelData)
        {
            // 读取Bitmap数据
            WriteableBitmap foregroundBitmap;

            // 解析Raw数据
            foregroundBitmap = new WriteableBitmap(
                        width,
                        height,
                        96.0,
                        96.0,
                        PixelFormats.Bgra32,
                        null);
            foregroundBitmap.WritePixels(
                new Int32Rect(0, 0,
                foregroundBitmap.PixelWidth,
                foregroundBitmap.PixelHeight),
                rawPixelData,
                foregroundBitmap.PixelWidth * sizeof(int),
                0);

            imageSource = foregroundBitmap;
            
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


