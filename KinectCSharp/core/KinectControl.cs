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
    using System.IO;
    using KinectCSharp.util;
    using System.Windows;

    // TODO 多台Kinect可考虑用OpenMultiSourceFrameReader 
    // https://msdn.microsoft.com/en-us/library/microsoft.kinect.kinectsensor.openmultisourceframereader.aspx

    // Kinect控制
    public class KinectControl
    {
        private KinectSensor sensor;
        // 控制是否存储到buffer
        public bool record = false;
        private static bool sensorOpen = false;
        public ReadOnlyCollection<byte> parameters;

        public List<Feature> featureBuffer = new List<Feature>();

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
            feature.caculateAngle();
            this.featureReady(feature);
            // 如果开启录制
            if (this.record)
            {
                this.featureBuffer.Add(feature);
            }
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

        // 从文件加载
        public void loadFromFile(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);
            Feature featureTemp = new Feature();
            byte[] temp = featureTemp.getByte();

            while (fs.Read(temp,0,temp.Length) == temp.Length)
            {
                featureTemp.parseByte(temp);
                // 记住要注意深拷贝！！！
                featureBuffer.Add(featureTemp.clone());
            }

            fs.Close();

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
            for(int i = 0;i < featureBuffer.Count; i++)
            {
                byte[] btData = featureBuffer[i].getByte();
                fs.Write(btData, 0, btData.Length);
            }
            fs.Close();
        }

        

        // 清空缓存
        public void emptyBuffer()
        {
            this.featureBuffer.Clear();
        }


        

    }
}
