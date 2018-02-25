using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectCSharp.core
{
    using Microsoft.Kinect;
    using Microsoft.Kinect.Toolkit;
    using System.IO;
    using System.Collections;
    using KinectCSharp.util;
    using System.Windows;


    class KinectControl
    {
        private KinectSensor sensor;
        private bool record = false;
        private static bool sensorOpen = false;

        public List<Feature> featureBuffer;

        public delegate void FeatureReadyDelegate(Feature feature);
        public FeatureReadyDelegate featureReady;

        public KinectControl()
        {
        }

        // 提供sensor访问
        public KinectSensor getSensor()
        {
            return this.sensor;
        }

        // 收到一个帧的事件处理函数
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];
            Feature feature = new Feature();

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    feature.frameNum = skeletonFrame.FrameNumber;
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
                }
            }
            feature.ok = (feature.skeleton != null);

            this.featureReady(feature);
            // 如果开启录制
            if (this.record)
            {
                this.featureBuffer.Add(feature);
            }
        }

        // 开始存储到Buffer
        public bool openBuffer()
        {
            this.record = true;
            return this.record;
        }

        // 停止存储到Buffer
        public bool stopBuffer()
        {
            this.record = false;
            return this.record;
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
            KinectControl.sensorOpen = true;

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
                // Turn on the skeleton stream to receive skeleton frames
                TransformSmoothParameters parameters = new TransformSmoothParameters();
                parameters.Smoothing = 0.2f;
                parameters.Correction = 0.8f;
                parameters.Prediction = 0.0f;
                parameters.JitterRadius = 0.5f;
                this.sensor.SkeletonStream.Enable(parameters);
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

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

        // TODO 从文件加载
        public void loadFromFile(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);

            fs.Close();
        }

        // TODO 保存到文件
        public void saveToFile(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Create);
            for(int i = 0;i < featureBuffer.Count; i++)
            {
                byte[] btData = Util.ObjectToBytes(featureBuffer[i]);
                fs.Write(btData, 0, btData.Length);
            }
            fs.Close();
        }

        // TODO 播放缓存内容（调用featureReady的委托）
        public void playBuffer()
        {

        }

        // 清空缓存
        public void emptyBuffer()
        {
            this.featureBuffer.Clear();
        }

    }
}
