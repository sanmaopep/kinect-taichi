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


    class KinectControl
    {
        private KinectSensor sensor;
        private bool record = false;
        private static bool sensorOpen = false;

        public List<Feature> featureBuffer;

        public delegate void FeatureReady(Feature feature);


        public KinectControl()
        {
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
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }
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
        private void InitializeFaculty()
        {
            // 确保设备只会被启动一次
            if (KinectControl.sensorOpen)
            {
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
                if (null != this.sensor)
                {
                    // Turn on the skeleton stream to receive skeleton frames
                    this.sensor.SkeletonStream.Enable();
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
            }
        }

        // TODO 从文件加载
        public void loadFromFile(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);

            fs.Close();
            Console.WriteLine("文件读取成功");
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
            Console.WriteLine("文件保存成功");
        }

        // 清空缓存
        public void emptyBuffer()
        {
            this.featureBuffer.Clear();
        }

    }
}
