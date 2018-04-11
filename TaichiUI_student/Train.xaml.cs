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
        private bool isPlaying = false;
        private int delay = 30;
        private int currFrame = 0;


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
            imgTeacher.Source = kcTeacher.featureBuffer[0].backgroundRemoved.imageSource;
            progress.Maximum = kcTeacher.featureBuffer.Count - 1;
            progress.Value = currFrame = 0;

            // 文字信息
            tbTeacher.Text = getMotionDescription(0);
            tbStudent.Text = "右腿架子过高";
        }

        // 获取描述信息
        private string getMotionDescription(int frameNum)
        {
            SingleDetailDescription[] list = trainModel.currSingleMotionModel.details;
            for(int i = 0;i < list.Length; i++)
            {
                if (frameNum >= list[i].from && frameNum <= list[i].to)
                {
                    return list[i].description;
                }
            }

            return "";
        }

        // 显示对应的帧
        private void displayFrame(int frameNum)
        {
            List<Feature> list = kcTeacher.featureBuffer;
            currFrame = frameNum;
            progress.Value = frameNum;
            imgTeacher.Source = list[frameNum].backgroundRemoved.imageSource;
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
            for (int i = currFrame;i < list.Count; i++)
            {
                if (!isPlaying)
                {
                    break;
                }
                await Task.Run(() => Thread.Sleep(delay));

                displayFrame(i);
            }
            currFrame = 0;
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
    }
}
