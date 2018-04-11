using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KinectCore
{
    using core;
    using util;
    using System.Threading;

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectControl kinectControl;
        private FeaturePainter featurePainter;
        private KeyFrameExtract keyFrameExtract;
        private BitmapSource foregroundBitmap;

        // 多线程访问UI控件
        private readonly TaskScheduler _syncContextTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // 做实验
            InitializeKinect();
            if (!kinectControl.isSensorOpen())
            {
                btnKinectControl.Content = "启动Kinect";
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            kinectControl.stopFaculty();
        }


        private void InitializeKinect()
        {
            // 初始化Kinect
            kinectControl = new KinectControl();
            kinectControl.InitializeFaculty();
            kinectControl.featureReady += this.featureReady;
            kinectControl.backgroundReady += this.backgroundReady;

            // 初始化渲染
            featurePainter = new FeaturePainter(kinectControl);
            Image.Source = featurePainter.getImageSource();
        }

        private void featureReady(Feature feature)
        {
            TextConsole.Text = feature.print();
            tbRecordState.Text = "缓存帧数：" + kinectControl.featureBuffer.Count;
            featurePainter.paint(feature);
        }

        private void backgroundReady(BitmapSource writeableBitmap)
        {
            this.foregroundBitmap = writeableBitmap;
            PeopleImage.Source = writeableBitmap;
            // PeopleImage.Stretch = Stretch.None;
        }

        // Kinect开启关闭控制
        private void btnKinectControlClick(object sender, RoutedEventArgs e)
        {
           if (kinectControl.isSensorOpen())
            {
                kinectControl.stopFaculty();
                btnKinectControl.Content = "启动Kinect";
            }
            else
            {
                kinectControl.InitializeFaculty();
                btnKinectControl.Content = "关闭Kinect";
            }
        }

        // 保存
        private void btnSaveClick(object sender, RoutedEventArgs e)
        {
            // 关闭Kinect设备
            if (kinectControl.isSensorOpen())
            {
                kinectControl.stopFaculty();
                btnKinectControl.Content = "启动Kinect";
            }

            string filePath = tbFilePath.Text;
            try
            {
                kinectControl.saveToFile(filePath);
                MessageBox.Show("保存文件成功");
            }catch(Exception exception)
            {
                MessageBox.Show("保存文件发生错误\n"+exception.ToString());
            }
        }

        // 读取
        private void btnReadClick(object sender, RoutedEventArgs e)
        {
            // 关闭Kinect设备
            if (kinectControl.isSensorOpen())
            {
                kinectControl.stopFaculty();
                btnKinectControl.Content = "启动Kinect";
            }

            string filePath = tbFilePath.Text;
            kinectControl.emptyBuffer();
            try
            {
                kinectControl.loadFromFile(filePath);
                playFeatureBuffer(kinectControl.featureBuffer);
            }

            catch (Exception exception)
            {
                MessageBox.Show("读取文件发生错误\n" + exception.ToString());
            }
        }

        private async void playFeatureBuffer(List<Feature> list,int DELAY = 1000/60)
        {
            // 关闭Kinect设备
            if (kinectControl.isSensorOpen())
            {
                kinectControl.stopFaculty();
                btnKinectControl.Content = "启动Kinect";
            }
            int len = list.Count;
            for (int i = 0;i < len; i++)
            {
                Feature curr = list[i];
                await Task.Run(() => Thread.Sleep(DELAY));
                featureReady(curr);
                backgroundReady(curr.backgroundRemoved.imageSource);
            }
        }

        // 录制按钮按下
        private void btnRecordControlClick(object sender, RoutedEventArgs e)
        {
            if (kinectControl.record)
            {
                btnRecordControl.Content = "开始录制";
            }
            else
            {
                btnRecordControl.Content = "停止录制";
            }
            kinectControl.record = !kinectControl.record;
        }

        private void btnKmeansClick(object sender, RoutedEventArgs e)
        {
            if (keyFrameExtract == null)
            {
                keyFrameExtract = new KeyFrameExtract(this.kinectControl);
            }
            keyFrameExtract.OneInterationKMeans();
            playFeatureBuffer(keyFrameExtract.caculateResultBuffer(),500);
        }

        private void btnClearBufferClick(object sender, RoutedEventArgs e)
        {
            this.kinectControl.featureBuffer.Clear();
            tbRecordState.Text = "缓存帧数：" + kinectControl.featureBuffer.Count;
        }
    }
}
