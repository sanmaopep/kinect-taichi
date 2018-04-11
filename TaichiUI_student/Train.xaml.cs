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
    // 练习模式
    public partial class Train : UserControl
    {
        public Train()
        {
            InitializeComponent();
            test.Text = ((MainWindowModel)DataContext).trainModel.currSingleMotionModel.title;
        }
    }
}
