using KinectCore.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaichiUI_student.ViewModels
{
    /* 实践模式ViewModel */
    class PracticeModel : PropertyChange
    {
        private SingleMotionModel _currSingleMotionModel;

        public SingleMotionModel currSingleMotionModel
        {
            get
            {
                return _currSingleMotionModel;
            }
            set
            {
                UpdateProperty(ref _currSingleMotionModel, value);
            }
        }
    }
}
