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
using TaichiUI_student.Models;

namespace TaichiUI_student
{
    public enum ViewItemEnum
    {
        HOME = 0,       // 首页
        TRAIN = 1,      // 练习模式
        PRACTICE = 2    // 实战模式
    }


    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowModel();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            setContent(ViewItemEnum.HOME);
        }

        public void setContent(ViewItemEnum viewItemEnum)
        {
            switch (viewItemEnum)
            {
                case ViewItemEnum.HOME:
                    ctlMainContent.Content = new Home();
                    break;
                default:
                    break;
            }
        }

        
    }
}
