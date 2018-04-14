using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TaichiUI_teacher.ViewModels
{
    class MainWindowModel : PropertyChange
    {
        public const string MOTION_LIB_PATH = @"../../../MotionDataSet";
        // 组合子ViewModels
        public HomeModel homeModel = new HomeModel();
        
        // 主窗口相关的类
        private UserControl _MainContent;
        private string _Title;
        private Boolean _HomeBackVisible;

        public MainWindowModel()
        {
            HomeBackVisible = false;
        }

        public Boolean HomeBackVisible{
            get
            {
                return _HomeBackVisible;
            }
            set
            {
                UpdateProperty(ref _HomeBackVisible, value);
            }
        }

        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                UpdateProperty(ref _Title,value);
            }
        }

        public UserControl MainContent
        {
            get
            {
                return _MainContent;
            }
            set
            {
                UpdateProperty(ref _MainContent, value);
            }
        }

    }
}
