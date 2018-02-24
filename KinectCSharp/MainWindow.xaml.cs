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

            // 初始化渲染
            featurePainter = new FeaturePainter(kinectControl);
            Image.Source = featurePainter.getImageSource();
            featurePainter.drawBackground();
        }
    }
}
