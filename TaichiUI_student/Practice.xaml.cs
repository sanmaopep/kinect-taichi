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

        public Practice()
        {
            InitializeComponent();
        }

        bool firstReceived = false;

        // 收到一个帧
        private void getStudentFrame(Feature feature)
        {
            imgDisplay.Source = feature.rgbImage.imageSource;

            // 先匹配第一帧
            if (realTimeDTW.firstFrameReceive(feature) && !firstReceived)
            {
                firstReceived = true;
                tbNotice.Text = "评分已经开始";
            }

            if (firstReceived)
            {
                caculateScore(feature);
            }
        }

        // 计算分数与进度
        private double currScore = 0;
        private bool caculateLock = false;

        private async void caculateScore(Feature feature)
        {
            realTimeDTW.handleNewFrame(feature);
            tbProgess.Text = 100 * realTimeDTW.getProgressInt() / tplFeatures.Count + "%";
            progress.Value = realTimeDTW.getProgressInt();

            Console.WriteLine(realTimeDTW.getProgressInt());

            if (realTimeDTW.getProgressInt() == tplFeatures.Count)
            {
                kcStudent.stopFaculty();
                tbNotice.Text = "动作结束，评分结束";
                tbSimilar.Text = realTimeDTW.getScore() + "";
            }

            if (!caculateLock)
            {
                await Task.Run(() =>
                {
                    caculateLock = true;
                    currScore = realTimeDTW.getScore();
                    caculateLock = false;
                });
            }

            tbSimilar.Text = currScore + "";
        }

        // 初始化
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            practiceModel = ((MainWindowModel)DataContext).practiceModel;
            motionPath = MainWindowModel.MOTION_LIB_PATH +
                "/" + practiceModel.currSingleMotionModel.data;


            tbNotice.Text = "摆好启动动作会坐自动开启评分";
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
            kcStudent.featureReady += this.getStudentFrame;
            kcStudent.InitializeFaculty();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            kcStudent.featureReady -= this.getStudentFrame;
            kcStudent.stopFaculty();
            kcStudent = null;
        }
    }
}
