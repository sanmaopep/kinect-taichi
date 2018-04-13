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
        private int currFrame = 0;

        public Practice()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            practiceModel = ((MainWindowModel)DataContext).practiceModel;
            motionPath = MainWindowModel.MOTION_LIB_PATH +
                "/" + practiceModel.currSingleMotionModel.data;

            // 读取数据
            try
            {
                kcTeacher.loadFromFile(motionPath);
            }
            catch (Exception exception)
            {
                MessageBox.Show("读取动作数据发生错误\n" + exception.ToString());
            }

            for (int i = 0;i < 10; i++)
            {
                await Task.Run(() => Thread.Sleep(1000));
                progress.Value = i*10;
            }
        }
    }
}
