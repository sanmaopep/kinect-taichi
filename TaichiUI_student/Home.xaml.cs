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
using TaichiUI_student.Components;
using KinectCore.model;
using KinectCore.util;

namespace TaichiUI_student
{
    /* 首页交互逻辑 */
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
        }

        private void UserControlLoaded(object sender, RoutedEventArgs e)
        {
            string motionLibPath = @"../../../MotionDataSet";
            SingleMotionModel[] singleMotionModel = MotionLibsUtil.parseFromFile(motionLibPath);
            for (int i = 0;i < singleMotionModel.Length; i++)
            {
                wpKungfuList.Children.Add(new KungfuMoveCard(singleMotionModel[i], motionLibPath));
            }
        }
    }
}
