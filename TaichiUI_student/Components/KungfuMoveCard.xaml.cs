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

namespace TaichiUI_student.Components
{
    /// <summary>
    /// KungfuMoveCard.xaml 的交互逻辑
    /// </summary>
    public partial class KungfuMoveCard : UserControl
    {
        private string _title;
        private string _description;
        private ImageSource _picSource;

        public KungfuMoveCard()
        {
            InitializeComponent();
        }

        public string title
        {
            set
            {
                _title = value;
                tbTitle.Text = _title;
            }
            get
            {
                return _title;
            }
        }

        public string description
        {
            set
            {
                _description = value;
                tbDescription.Text = _description;
            }
            get
            {
                return _description;
            }
        }
    }
}
