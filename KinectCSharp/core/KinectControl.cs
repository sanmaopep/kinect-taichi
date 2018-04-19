using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectCore.core
{
    using Microsoft.Kinect;
    using System.Collections.ObjectModel;
    using Microsoft.Kinect.Toolkit;
    using Microsoft.Kinect.Toolkit.BackgroundRemoval;
    using System.IO;
    using KinectCore.util;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using LZ4;

    // TODO 多台Kinect可考虑用OpenMultiSourceFrameReader 
    // https://msdn.microsoft.com/en-us/library/microsoft.kinect.kinectsensor.openmultisourceframereader.aspx

    // Kinect控制
    public class KinectControl
    {
        private KinectSensor sensor;    // sensor
        public bool record = false; // 控制是否存储到buffer
        public bool recordRgb = false; // 控制是否存储Rgb图像

        private static bool sensorOpen = false; // 控制一个时候只能初始化一个sensor

        public List<Feature> featureBuffer = new List<Feature>();   // Feature的缓存

        public delegate void FeatureReadyDelegate(Feature feature);
        public FeatureReadyDelegate featureReady;// Feature 准备好的委托

        public KinectControl()
        {
        }

        // 像素数据缓存
        private byte[] pixelData;
        private ColorImageFormat lastImageFormat = ColorImageFormat.Undefined;
        public WriteableBitmap outputImage;

        // 收到一个所有帧的处理就函数
        private void AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            var colorFrame = e.OpenColorImageFrame();
            var skeletonFrame = e.OpenSkeletonFrame();

            if (colorFrame != null && skeletonFrame != null)
            {
                Skeleton[] skeletons = new Skeleton[0];
                Feature feature = new Feature();
                RGBImage readyImage = new RGBImage();

                // 图片处理
                bool haveNewFormat = this.lastImageFormat != colorFrame.Format;
                int width = colorFrame.Width;
                int height = colorFrame.Height;
                int bytesPerPixel = colorFrame.BytesPerPixel;
                if (haveNewFormat)
                {
                    pixelData = new byte[colorFrame.PixelDataLength];
                }

                colorFrame.CopyPixelDataTo(pixelData);
                readyImage.ParsePixelData(width, height, pixelData, bytesPerPixel);
                this.lastImageFormat = colorFrame.Format;

                //骨骼处理
                skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                skeletonFrame.CopySkeletonDataTo(skeletons);

                // 找到第一个有数据的骨骼，并存入feature
                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            feature.skeleton = skel;
                            break;
                        }
                    }
                }
                feature.ok = (feature.skeleton != null);

                feature.caculateAngle();
                feature.rgbImage = readyImage;
                this.featureReady(feature);

                // 如果开启录制
                if (this.record && feature.ok)
                {
                    Feature cloneFeature = feature.clone();
                    if (!recordRgb)
                    {
                        cloneFeature.rgbImage = null;
                    }
                    this.featureBuffer.Add(cloneFeature);
                }

            }

        }


        // 提供sensor访问
        public KinectSensor getSensor()
        {
            return this.sensor;
        }

        // 初始化Kinect设备
        public void InitializeFaculty()
        {
            // 确保设备只会被启动一次
            if (KinectControl.sensorOpen)
            {
                MessageBox.Show("设备已启动");
                return;
            }

            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }
            if (null != this.sensor)
            {
                KinectControl.sensorOpen = true;
                // 打开Color流和深度图像流
                ColorImageFormat colorImageFormat = ColorImageFormat.RgbResolution640x480Fps30;
                this.sensor.ColorStream.Enable(colorImageFormat);
                // 打开骨骼数据流
                TransformSmoothParameters parameters = new TransformSmoothParameters();
                parameters.Smoothing = 0.2f;
                parameters.Correction = 0.8f;
                parameters.Prediction = 0.0f;
                parameters.JitterRadius = 0.5f;
                this.sensor.SkeletonStream.Enable(parameters);
                sensor.AllFramesReady += this.AllFramesReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }
            else
            {
                MessageBox.Show("没有找到设备");
            }
        }

        // 停止设备
        public void stopFaculty()
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
                KinectControl.sensorOpen = false;
            }
        }

        // 是否启动设备
        public bool isSensorOpen()
        {
            return KinectControl.sensorOpen;
        }

        // 从文件加载固定个数，并不进行计算
        public void loadFramesFromFile(string filePath, int frameNum)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);
            LZ4Stream lz4s = new LZ4Stream(fs, LZ4StreamMode.Decompress);

            Feature featureTemp = new Feature();

            try
            {
                for (int i = 0; i < frameNum; i++)
                {
                    featureTemp.parseFromStream(lz4s);
                    // 记住要注意深拷贝！！！
                    featureBuffer.Add(featureTemp.clone());
                }
            }
            catch (EndOfStreamException exception)
            {
                // DO NOTHING NOW
            }

            lz4s.Close();
            fs.Close();
        }

        // 从文件加载
        public void loadFromFile(string filePath,bool parseJpeg = true)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);
            LZ4Stream lz4s = new LZ4Stream(fs, LZ4StreamMode.Decompress);

            Feature featureTemp = new Feature();

            try
            {
                while (true)
                {
                    featureTemp.parseFromStream(lz4s, parseJpeg);
                    // 记住要注意深拷贝！！！
                    featureBuffer.Add(featureTemp.clone());
                }
            }
            catch (EndOfStreamException exception)
            {
                lz4s.Close();
                fs.Close();
            }

            // 计算角度
            for (int i = 0; i < featureBuffer.Count; i++)
            {
                featureBuffer[i].caculateAngle();
            }
        }

        // 保存到文件
        public void saveToFile(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Create);
            LZ4Stream lz4s = new LZ4Stream(fs, LZ4StreamMode.Compress);

            for (int i = 0; i < featureBuffer.Count; i++)
            {
                byte[] btData = featureBuffer[i].getByte();
                lz4s.Write(btData, 0, btData.Length);
            }
            lz4s.Close();
            fs.Close();
            MessageBox.Show("保存文件成功");
        }

        // 清空缓存
        public void emptyBuffer()
        {
            this.featureBuffer.Clear();
        }
    }
}
