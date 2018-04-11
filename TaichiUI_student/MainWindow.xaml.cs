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

namespace TaichiUI_student
{

    public partial class MainWindow : Window
    {

        Home home = new Home();

        public MainWindow()
        {
            InitializeComponent();
            switchHome();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void btnHomeBackClick(object sender, RoutedEventArgs e)
        {
            switchHome();
        }

        private void switchHome()
        {
            MainWindowModel mainWindowModel = (MainWindowModel)DataContext;
            mainWindowModel.MainContent = home;
            mainWindowModel.Title = "动作列表";
            mainWindowModel.HomeBackVisible = false;
        }
    }
}
