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
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectControl kinectControl;
        private FeaturePainter featurePainter;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            InitializeKinect();
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

        // feature准备好的委托
        private void featureReady(Feature feature)
        {
            // 获得信息
            TextConsole.Text = feature.print();
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
    }
}
