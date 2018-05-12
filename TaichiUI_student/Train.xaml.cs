using KinectCore.model;
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
using TaichiUI_student.ViewModels;
using KinectCore.core;
using System.Threading;

namespace TaichiUI_student
{
    // 练习模式
    public partial class Train : UserControl
    {
        private TrainModel trainModel;
        private string motionPath;
        private KinectControl kcTeacher = new KinectControl();
        private KinectControl kcStudent = new KinectControl();
        private TrainAnalysis trainAnalysis;
        private bool isPlaying = false;
        private int delay = 30;
        private int currFrame = 0;

        private BitmapSource currStudentImageSource;


        public Train()
        {
            InitializeComponent();
        }

        private void userControlLoaded(object sender, RoutedEventArgs e)
        {
            trainModel = ((MainWindowModel)DataContext).trainModel;
            motionPath = MainWindowModel.MOTION_LIB_PATH +
                "/" + trainModel.currSingleMotionModel.data;

            // 读取数据
            try
            {
                kcTeacher.loadFromFile(motionPath);
            }
            catch (Exception exception)
            {
                MessageBox.Show("读取动作数据发生错误\n" + exception.ToString());
            }

            // 将数据更新到UI组件上
            imgTeacher.Source = kcTeacher.featureBuffer[0].rgbImage.imageSource;
            progress.Maximum = kcTeacher.featureBuffer.Count - 1;
            progress.Value = currFrame = 0;

            // 文字信息
            tbTeacher.Text = getMotionDescription(0);

            // 初始化训练分析器
            trainAnalysis = new TrainAnalysis(kcTeacher.featureBuffer);

            InitializeFaculty();
        }

        // 收到一个帧
        private void getStudentFrame(Feature feature)
        {
            imgStudent.Source = feature.rgbImage.imageSource;
            tbStudent.Text = trainAnalysis.Analysis(currFrame, feature);
        }




        // 初始化Kinect设备
        private void InitializeFaculty()
        {
            kcStudent.InitializeFaculty();
            kcStudent.featureReady += getStudentFrame;
        }

        // 获取描述信息
        private string getMotionDescription(int frameNum)
        {
            SingleDetailDescription[] list = trainModel.currSingleMotionModel.details;
            if(list == null)
            {
                return "";
            }
            for (int i = 0; i < list.Length; i++)
            {
                if (frameNum >= list[i].from && frameNum <= list[i].to)
                {
                    return list[i].description;
                }
            }

            return "";
        }

        // 显示TEACHER对应的帧
        private void displayFrame(int frameNum)
        {
            List<Feature> list = kcTeacher.featureBuffer;
            currFrame = frameNum;
            progress.Value = frameNum;
            imgTeacher.Source = list[frameNum].rgbImage.imageSource;
            tbTeacher.Text = getMotionDescription(frameNum);
        }

        // 停止播放
        private void stopPlay()
        {
            isPlaying = false;
            playBtnIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
        }

        // 开始播放
        private async void startPlay()
        {
            isPlaying = true;
            playBtnIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;

            // UI Update
            List<Feature> list = kcTeacher.featureBuffer;
            for (int i = currFrame; i < list.Count; i++)
            {
                if (!isPlaying)
                {
                    break;
                }
                await Task.Run(() => Thread.Sleep(delay));

                displayFrame(i);
            }
            if (currFrame == list.Count)
            {
                currFrame = 0;
            }
            stopPlay();
        }

        // UI事件绑定
        private void btnPlayClick(object sender, RoutedEventArgs e)
        {
            if (!isPlaying)
            {
                startPlay();
            }
            else
            {
                stopPlay();
            }
        }

        private void progress_MouseDown(object sender, MouseButtonEventArgs e)
        {
            stopPlay();
        }


        private void progress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            currFrame = (int)e.NewValue;
            displayFrame(currFrame);
        }

        private void lbSpeed_Selected(object sender, RoutedEventArgs e)
        {
            int index = lbSpeed.SelectedIndex;
            int[] speedTable = new int[3] { 100, 30, 10 };
            delay = speedTable[index];
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            kcStudent.stopFaculty();
        }
    }
}
