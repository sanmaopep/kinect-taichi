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
    using System.Threading;

    // TODO 多台Kinect可考虑用OpenMultiSourceFrameReader 
    // https://msdn.microsoft.com/en-us/library/microsoft.kinect.kinectsensor.openmultisourceframereader.aspx

    // Kinect控制
    public class KinectControl
    {
        private KinectSensor sensor;    // sensor
        public bool recordToBuffer = false; // 控制是否存储到buffer（不保存rgb图像）

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
                readyImage.ParsePixelData(pixelData);
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
                if(this.featureReady != null)
                {
                    this.featureReady(feature);
                }

                // 如果录制到内存缓存
                if (this.recordToBuffer && feature.ok)
                {
                    Feature cloneFeature = feature.clone();
                    cloneFeature.rgbImage = null;
                    this.featureBuffer.Add(cloneFeature);
                }

                // 如果录制到文件
                if(this.recordFile && feature.ok)
                {
                    recordToFile.pushFeature(feature.clone());
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
            if (null != this.sensor && sensorOpen)
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
        public void loadFromFile(string filePath, bool parseJpeg = true)
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

        // 录制到文件
        private RecordToFile recordToFile;
        private bool recordFile = false;
        public void startRecordToFile(string filePath)
        {
            recordToFile = new RecordToFile(filePath);
            recordFile = true;
            recordToFile.start();
        }

        public void stopRecordToFile()
        {
            recordToFile.stop();
        }


        // 清空缓存
        public void emptyBuffer()
        {
            this.featureBuffer.Clear();
        }


        // 从MSRC数据库CSV文件加载
        public void loadFromCSV(string filename)
        {
            var reader = new StreamReader(File.OpenRead(filename));
            Feature featureTemp = new Feature();

            while (!reader.EndOfStream)
            {
                featureTemp.parseMSRCLine(reader.ReadLine());
                featureBuffer.Add(featureTemp.clone());
            }

            // 计算角度
            for (int i = 0; i < featureBuffer.Count; i++)
            {
                featureBuffer[i].caculateAngle();
            }
        }
    }


    public class RecordToFile
    {
        Thread saveFileThread;
        FileStream fs;
        LZ4Stream lz4s;
        Queue<Feature> featureQueue = new Queue<Feature>();

        public RecordToFile(string filePath)
        {
            fs = new FileStream(filePath, FileMode.Create);
            lz4s = new LZ4Stream(fs, LZ4StreamMode.Compress);
            saveFileThread = new Thread(new ThreadStart(startRun));
            saveFileThread.IsBackground = true;
        }

        public void pushFeature(Feature feature)
        {
            featureQueue.Enqueue(feature);
        }

        public void startRun()
        {
            while (true)
            {
                if (featureQueue.Count == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }
                byte[] btData = featureQueue.Dequeue().getByte();
                lz4s.Write(btData, 0, btData.Length);
            }
        }

        public void start()
        {
            saveFileThread.Start();
        }

        public async void stop()
        {
            await Task.Run(() =>
            {
                // 等待队列为空
                while (featureQueue.Count != 0)
                {
                    Thread.Sleep(100);
                }
            });
            saveFileThread.Abort();
            lz4s.Close();
            fs.Close();
        }
    }
}
