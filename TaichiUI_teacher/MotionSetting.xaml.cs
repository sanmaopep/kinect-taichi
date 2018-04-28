using KinectCore.core;
using KinectCore.model;
using KinectCore.util;
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
using TaichiUI_teacher.Components;
using TaichiUI_teacher.ViewModels;

namespace TaichiUI_teacher
{
    /// <summary>
    /// MotionSetting.xaml 的交互逻辑
    /// </summary>
    public partial class MotionSetting : UserControl
    {
        private EditModel editModel;
        private string motionPath;
        private KinectControl kcTeacher = new KinectControl();
        private bool isPlaying = false;
        private int delay = 30;
        private int currFrame = 0;

        public MotionSetting()
        {
            InitializeComponent();
        }

        private void UserControlLoaded(object sender, RoutedEventArgs e)
        {
            editModel = ((MainWindowModel)DataContext).editModel;
            motionPath = MainWindowModel.MOTION_LIB_PATH +
                "/" + editModel.currSingleMotionModel.data;

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

            renderUI();
        }

        // 保存数据
        private void saveData()
        {
            SingleDetailDescription[] details= new SingleDetailDescription[spDetails.Children.Count];
            for (int i = 0;i < spDetails.Children.Count; i++)
            {
                SingleMotionDetailEditor curr = (SingleMotionDetailEditor)spDetails.Children[i];
                details[i] = curr.getValue();
            }
            editModel.currSingleMotionModel.details = details;
            editModel.currSingleMotionModel.title = tbName.Text;
            editModel.currSingleMotionModel.description = tbDescription.Text;

            SingleMotionModel[] models = ((MainWindowModel)DataContext).homeModel.singleMotionModels;

            for(int i = 0;i < models.Length; i++)
            {
                if(models[i].data == editModel.currSingleMotionModel.data)
                {
                    models[i] = editModel.currSingleMotionModel;
                    break;
                }
            }

            MotionLibsUtil.saveToFile(models, MainWindowModel.MOTION_LIB_PATH);
            MessageBox.Show("保存成功");
        }

        // 渲染数据
        private void renderUI()
        {
            tbName.Text = editModel.currSingleMotionModel.title;
            tbDescription.Text = editModel.currSingleMotionModel.description;

            if(editModel.currSingleMotionModel.details != null)
            {
                for (int i = 0; i < editModel.currSingleMotionModel.details.Length; i++)
                {
                    spDetails.Children.Add(new SingleMotionDetailEditor(editModel.currSingleMotionModel.details[i]));
                }
            }
            
        }

        // 显示TEACHER对应的帧
        private void displayFrame(int frameNum)
        {
            List<Feature> list = kcTeacher.featureBuffer;
            tbCurrFrame.Text = frameNum + "";
            currFrame = frameNum;
            progress.Value = frameNum;
            imgTeacher.Source = list[frameNum].rgbImage.imageSource;
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

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void btnDescClick(object sender, RoutedEventArgs e)
        {
            spDetails.Children.Add(new SingleMotionDetailEditor());
        }

        private void btnSaveClick(object sender, RoutedEventArgs e)
        {
            saveData();
        }
    }
}
