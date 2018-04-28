using KinectCore.model;
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

namespace TaichiUI_teacher.Components
{
    /// <summary>
    /// SingleMotionDetailEditor.xaml 的交互逻辑
    /// </summary>
    public partial class SingleMotionDetailEditor : UserControl
    {
        public SingleMotionDetailEditor()
        {
            InitializeComponent();
        }

        public SingleMotionDetailEditor(SingleDetailDescription singleDetailDescription)
        {
            InitializeComponent();
            setValue(singleDetailDescription);
        }

        public void setValue(SingleDetailDescription singleDetailDescription)
        {
            tbStart.Text = singleDetailDescription.from + "";
            tbEnd.Text = singleDetailDescription.to + "";
            tbDescription.Text = singleDetailDescription.description + "";
        }

        public SingleDetailDescription getValue()
        {
            SingleDetailDescription singleDetailDescription = new SingleDetailDescription();
            singleDetailDescription.from = int.Parse(tbStart.Text);
            singleDetailDescription.to = int.Parse(tbEnd.Text);
            singleDetailDescription.description = tbDescription.Text;
            return singleDetailDescription;
        }

        private void btnDeleteClick(object sender, RoutedEventArgs e)
        {
            StackPanel parentPanel =  (StackPanel)this.Parent;
            parentPanel.Children.Remove(this);
        }
    }
}
