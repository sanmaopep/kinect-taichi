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

namespace TaichiUI_student
{
    // 训练模式
    public partial class Practice : UserControl
    {
        public Practice()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            for(int i = 0;i < 10; i++)
            {
                await Task.Run(() => Thread.Sleep(1000));
                progress.Value = i*10;
            }
        }
    }
}
