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

        // 实验用DIFF
        private const int DIFF = 100;


        public Record() => InitializeComponent();

        // 收到一个帧
        private void getRecordFeature(Feature feature)
        {
            imgRgb.Source = feature.rgbImage.imageSource;
            featurePainter.paint(feature);

            // 计算运动量
            double sum = 0;
            int count = kcRecorder.featureBuffer.Count;
            if(count < DIFF + 1)
            {
                tbNotice.Text = "运动量计算 帧数不够";
                return;
            }
            for (int j = count - DIFF; j < count; j++)
            {
                Feature before = kcRecorder.featureBuffer[j];
                before.caculateAngle();
                sum += Feature.EDistance(before, feature);
            }

            Console.WriteLine(sum / DIFF);
            tbNotice.Text = "运动量计算" + sum / DIFF;


            if (kcRecorder.featureBuffer.Count > DIFF * 3)
            {
                kcRecorder.featureBuffer.RemoveRange(0, DIFF);
            }
        }

        private void UserControlLoaded(object sender, RoutedEventArgs e)
        {
            // 初始化设备
            kcRecorder.InitializeFaculty();
            kcRecorder.featureReady += getRecordFeature;
            // 实验用
            kcRecorder.record = true;
            kcRecorder.recordRgb = false;

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
