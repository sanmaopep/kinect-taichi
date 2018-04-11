using KinectCore.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaichiUI_student.ViewModels
{
    /* 训练模式ViewModel */
    public class TrainModel: PropertyChange
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
