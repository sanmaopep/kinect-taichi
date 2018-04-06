using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectCSharp.core
{
    using Microsoft.Kinect;
    using System.Collections.ObjectModel;
    using Microsoft.Kinect.Toolkit;
    using Microsoft.Kinect.Toolkit.BackgroundRemoval;
    using System.IO;
    using KinectCSharp.util;
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
        private static bool sensorOpen = false; // 控制一个时候只能初始化一个sensor

        public List<Feature> featureBuffer = new List<Feature>();   // Feature的缓存
        public List<BackgroundRemoved> backgroundBuffer = new List<BackgroundRemoved>();    // 前景缓存
        
        public delegate void FeatureReadyDelegate(Feature feature); 
        public FeatureReadyDelegate featureReady;// Feature 准备好的委托

        private BackgroundRemovedColorStream backgroundRemovedColorStream;

        public delegate void BackgroundReadyDelegate(BitmapSource writeableBitmap);
        public BackgroundReadyDelegate backgroundReady;// Feature 准备好的委托

        public KinectControl()
        {
        }

        // 收到一个骨骼帧的事件处理函数
        private void HandleSkeletonFrame(SkeletonFrame skeletonFrame)
        {
            Skeleton[] skeletons = new Skeleton[0];
            Feature feature = new Feature();

            if (skeletonFrame != null)
            {
                feature.frameNum = skeletonFrame.FrameNumber;
                skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                skeletonFrame.CopySkeletonDataTo(skeletons);
                this.backgroundRemovedColorStream.ProcessSkeleton(skeletons, skeletonFrame.Timestamp);


                // 找到第一个有数据的骨骼，并存入feature
                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            feature.skeleton = skel;
                            backgroundRemovedColorStream.SetTrackedPlayer(skel.TrackingId);
                            break;
                        }
                    }
                }
            }
            feature.ok = (feature.skeleton != null);
            feature.caculateAngle();
            this.featureReady(feature);
            // 如果开启录制
            if (this.record)
            {
                this.featureBuffer.Add(feature);
            }
        }

        // 收到一个背景去除帧的处理函数
        private void BackgroundRemovedFrameReady(object sender,BackgroundRemovedColorFrameReadyEventArgs e)
        {
            using (var backgroundRemovedFrame = e.OpenBackgroundRemovedColorFrame())
            {
                if(backgroundRemovedFrame != null)
                {
                    BackgroundRemoved backgroundRemoved = new BackgroundRemoved();
                    int width = backgroundRemovedFrame.Width;
                    int height = backgroundRemovedFrame.Height;
                    byte[] rawPixelData = backgroundRemovedFrame.GetRawPixelData();

                    backgroundRemoved.ParseRawData(width, height, rawPixelData);

                    backgroundReady(backgroundRemoved.imageSource);
                    if (this.record)
                    {
                        backgroundBuffer.Add(backgroundRemoved);
                    }
                }
               
            }
        }

        // 收到一个所有帧的处理就函数
        private void AllFramesReady(object sender,AllFramesReadyEventArgs e)
        {
            using (var depthFrame = e.OpenDepthImageFrame())
            {
                if (null != depthFrame)
                {
                    this.backgroundRemovedColorStream.ProcessDepth(depthFrame.GetRawPixelData(), depthFrame.Timestamp);
                }
            }
            using (var colorFrame = e.OpenColorImageFrame())
            {
                if (null != colorFrame)
                {
                    this.backgroundRemovedColorStream.ProcessColor(colorFrame.GetRawPixelData(), colorFrame.Timestamp);
                }
            }
            using (var skeletonFrame = e.OpenSkeletonFrame())
            {
                if (null != skeletonFrame)
                {
                    HandleSkeletonFrame(skeletonFrame);
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
                ColorImageFormat colorImageFormat= ColorImageFormat.RgbResolution640x480Fps30;
                DepthImageFormat depthImageFormat= DepthImageFormat.Resolution320x240Fps30;
                this.sensor.ColorStream.Enable(colorImageFormat);
                this.sensor.DepthStream.Enable(depthImageFormat);
                // 打开骨骼数据流
                TransformSmoothParameters parameters = new TransformSmoothParameters();
                parameters.Smoothing = 0.2f;
                parameters.Correction = 0.8f;
                parameters.Prediction = 0.0f;
                parameters.JitterRadius = 0.5f;
                this.sensor.SkeletonStream.Enable(parameters);
                // 打开背景去除图像流
                backgroundRemovedColorStream = new BackgroundRemovedColorStream(this.sensor);
                backgroundRemovedColorStream.Enable(colorImageFormat, depthImageFormat);

                backgroundRemovedColorStream.BackgroundRemovedFrameReady += this.BackgroundRemovedFrameReady;
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
            if(null != this.sensor)
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

        // 从文件加载
        public void loadFromFile(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);
            LZ4Stream lz4s = new LZ4Stream(fs, LZ4StreamMode.Decompress);

            Feature featureTemp = new Feature();

            try
            {
                while (true)
                {
                    featureTemp.parseFromStream(lz4s);
                    // 记住要注意深拷贝！！！
                    featureBuffer.Add(featureTemp.clone());
                }
            } catch(EndOfStreamException exception)
            {
                lz4s.Close();
                fs.Close();
            }

            // 计算角度
            for(int i = 0;i < featureBuffer.Count; i++)
            {
                featureBuffer[i].caculateAngle();
            }
        }

        // 保存到文件
        public void saveToFile(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Create);
            LZ4Stream lz4s = new LZ4Stream(fs, LZ4StreamMode.Compress);

            for (int i = 0;i < featureBuffer.Count; i++)
            {
                if(i < backgroundBuffer.Count)
                {
                    featureBuffer[i].backgroundRemoved = backgroundBuffer[i];
                }
                byte[] btData = featureBuffer[i].getByte();
                lz4s.Write(btData, 0, btData.Length);
            }
            lz4s.Close();
            fs.Close();
        }
        
        // 清空缓存
        public void emptyBuffer()
        {
            this.featureBuffer.Clear();
        }
    }
}
