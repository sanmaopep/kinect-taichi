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
using KinectCore.model;
using KinectCore.util;
using TaichiUI_student.ViewModels;

namespace TaichiUI_student.Components
{
    /// <summary>
    /// KungfuMoveCard.xaml 的交互逻辑
    /// </summary>
    public partial class KungfuMoveCard : UserControl
    {
        private const int DESCRIPTION_MAX_LEN = 30;

        private SingleMotionModel singleMotionModel;
        private string motionLibPath;

        public KungfuMoveCard()
        {
            InitializeComponent();
        }

        public KungfuMoveCard(SingleMotionModel singleMotionModel,string motionLibPath)
        {
            InitializeComponent();

            this.singleMotionModel = singleMotionModel;
            this.motionLibPath = motionLibPath;

            this.title = singleMotionModel.title;
            // 字符串长度控制
            if (singleMotionModel.description.Length > DESCRIPTION_MAX_LEN)
            {
                this.description = singleMotionModel.description.Substring(0, DESCRIPTION_MAX_LEN) + "...";
            }
            else
            {
                this.description = singleMotionModel.description;
            }

            // 设置ImageSource
            this.picSource = MotionLibsUtil.getPicSource(motionLibPath, singleMotionModel);
        }

        public string title
        {
            set
            {
                tbTitle.Text = value;
            }
            get
            {
                return tbTitle.Text;
            }
        }
        
        public ImageSource picSource
        {
            set
            {
                imgKunfu.Source = value;
            }
            get
            {
                return imgKunfu.Source;
            }
        }

        public string description
        {
            set
            {
                tbDescription.Text = value;
            }
            get
            {
                return tbDescription.Text;
            }
        }

        private void btnPracticeModeClick(object sender, RoutedEventArgs e)
        {
            MainWindowModel mainWindowModel = (MainWindowModel)DataContext;
            mainWindowModel.Title = singleMotionModel.title + "  实战中";
            mainWindowModel.HomeBackVisible = true;
            mainWindowModel.practiceModel.currSingleMotionModel = singleMotionModel;

            mainWindowModel.MainContent = new Practice();
        }

        private void btnTrainModeClick(object sender, RoutedEventArgs e)
        {
            MainWindowModel mainWindowModel = (MainWindowModel)DataContext;
            mainWindowModel.Title = singleMotionModel.title + "  练习中";
            mainWindowModel.HomeBackVisible = true;
            mainWindowModel.trainModel.currSingleMotionModel = singleMotionModel;

            mainWindowModel.MainContent = new Train();
        }
    }
}
