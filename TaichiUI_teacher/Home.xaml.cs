using KinectCore.model;
using KinectCore.util;
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
using TaichiUI_teacher.Components;
using TaichiUI_teacher.ViewModels;

namespace TaichiUI_teacher
{
    /// <summary>
    /// Home.xaml 的交互逻辑
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
        }

        private void UserControlLoaded(object sender, RoutedEventArgs e)
        {
            // 清空原有数据
            wpKungfuList.Children.Clear();

            string motionLibPath = MainWindowModel.MOTION_LIB_PATH;
            SingleMotionModel[] singleMotionModel = MotionLibsUtil.parseFromFile(motionLibPath);
            ((MainWindowModel)DataContext).homeModel.singleMotionModels = singleMotionModel;

            for (int i = 0; i < singleMotionModel.Length; i++)
            {
                wpKungfuList.Children.Add(new KungfuMoveCard(singleMotionModel[i], motionLibPath));
            }
        }
    }
}
