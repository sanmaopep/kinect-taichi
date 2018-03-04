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

namespace KinectCSharp
{
    using core;
    using System.Threading;

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectControl kinectControl;
        private FeaturePainter featurePainter;

        // 多线程访问UI控件
        private readonly TaskScheduler _syncContextTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
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

            // 初始化渲染
            featurePainter = new FeaturePainter(kinectControl);
            Image.Source = featurePainter.getImageSource();
        }

        private void featureReady(Feature feature)
        {
            feature.caculateAngle();
            TextConsole.Text = feature.print();
            tbRecordState.Text = "缓存帧数：" + kinectControl.featureBuffer.Count;
            featurePainter.paint(feature);
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
                playFeatureBuffer();
            }

            catch (Exception exception)
            {
                MessageBox.Show("读取文件发生错误\n" + exception.ToString());
            }
        }

        private async void playFeatureBuffer()
        {
            // 播放一个帧的延迟
            const int DELAY = 1000/30;
            // 关闭Kinect设备
            if (kinectControl.isSensorOpen())
            {
                kinectControl.stopFaculty();
                btnKinectControl.Content = "启动Kinect";
            }
            int len = kinectControl.featureBuffer.Count;
            Console.WriteLine(len);
            for (int i = 0;i < len; i++)
            {
                Feature curr = kinectControl.featureBuffer[i];
                await Task.Run(() => Thread.Sleep(DELAY));
                featureReady(curr);
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
    }
}
