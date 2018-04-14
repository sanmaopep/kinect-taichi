using KinectCore.core;
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

namespace TaichiUI_teacher
{
    /// <summary>
    /// Record.xaml 的交互逻辑
    /// </summary>
    public partial class Record : UserControl
    {

        private KinectControl kcRecorder = new KinectControl();
        private FeaturePainter featurePainter;

        public Record() => InitializeComponent();

        // 收到一个帧
        private void getRecordFeature(Feature feature)
        {
            imgRgb.Source = feature.rgbImage.imageSource;
            featurePainter.paint(feature);
        }

        private void UserControlLoaded(object sender, RoutedEventArgs e)
        {
            // 初始化设备
            kcRecorder.InitializeFaculty();
            kcRecorder.featureReady += getRecordFeature;

            // 渲染骨骼
            featurePainter = new FeaturePainter(kcRecorder);
            imgSkeleton.Source = featurePainter.getImageSource();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            kcRecorder.stopFaculty();
        }
    }
}
