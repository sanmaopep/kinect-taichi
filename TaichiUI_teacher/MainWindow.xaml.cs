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
using TaichiUI_teacher.ViewModels;

namespace TaichiUI_teacher
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            switchHome();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void BtnHomeBackClick(object sender, RoutedEventArgs e)
        {
            switchHome();
        }

        private void switchHome()
        {
            MainWindowModel mainWindowModel = (MainWindowModel)DataContext;
            mainWindowModel.MainContent = new Home();
            mainWindowModel.Title = "动作列表";
            mainWindowModel.HomeBackVisible = false;
        }

        private void BtnNewClick(object sender, RoutedEventArgs e)
        {
            MainWindowModel mainWindowModel = (MainWindowModel)DataContext;
            mainWindowModel.MainContent = new Record();
            mainWindowModel.Title = "动作录制";
            mainWindowModel.HomeBackVisible = true;
        }
    }
}
