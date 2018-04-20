using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using KinectCore.core;
using TaichiUI_student.ViewModels;

namespace TaichiUI_student
{
    // 训练模式
    public partial class Practice : UserControl
    {
        private PracticeModel practiceModel;
        private string motionPath;
        private KinectControl kcStudent = new KinectControl();
        private KinectControl kcTeacher = new KinectControl();
        private List<Feature> tplFeatures;
        private CaculusKeyFrameExtract keyFrameExtract;
        private RealTimeDTW realTimeDTW;
        private int currFrame = 0;

        public Practice()
        {
            InitializeComponent();
        }

        // 收到一个帧
        private void getStudentFrame(Feature feature)
        {
            imgDisplay.Source = feature.rgbImage.imageSource;
            caculateScore(feature);
        }

        // 计算分数与进度
        private const int INTERVAL = 100;
        private int intervalCounter = 0;
        private async void caculateScore(Feature feature)
        {
            await Task.Run(() => realTimeDTW.handleNewFrame(feature));
            tbProgess.Text = 100 * realTimeDTW.getProgressInt() / tplFeatures.Count + "%";
            progress.Value = realTimeDTW.getProgressInt();

            if (realTimeDTW.getProgressInt() == tplFeatures.Count)
            {
                kcStudent.stopFaculty();
                tbSimilar.Text = realTimeDTW.getScore() + "";
            }

            if (intervalCounter % INTERVAL == 0)
            {
                await Task.Run(() =>
                {
                    tbSimilar.Text = realTimeDTW.getScore() + "";
                });

            }
            intervalCounter++;
        }

        // 初始化
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            practiceModel = ((MainWindowModel)DataContext).practiceModel;
            motionPath = MainWindowModel.MOTION_LIB_PATH +
                "/" + practiceModel.currSingleMotionModel.data;

            // 读取数据
            try
            {
                kcTeacher.loadFromFile(motionPath, false);
                keyFrameExtract = new CaculusKeyFrameExtract(kcTeacher.featureBuffer);
                keyFrameExtract.caculate();
                tplFeatures = keyFrameExtract.getResultFeatures();
                realTimeDTW = new RealTimeDTW(tplFeatures);
                progress.Maximum = tplFeatures.Count;
            }
            catch (Exception exception)
            {
                MessageBox.Show("读取动作数据发生错误\n" + exception.ToString());
            }

            InitializeFaculty();
        }

        private void InitializeFaculty()
        {
            kcStudent.InitializeFaculty();
            kcStudent.featureReady += this.getStudentFrame;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            kcStudent.stopFaculty();
        }
    }
}
