using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectCore.core
{
    // 实时DTW计算
    class RealTimeDTW
    {
        private List<Feature> seqTepl;  // 模板动作
        private List<double[]> map;
        private List<double[]> dynamic;
        private List<Direction[]> road;

        public RealTimeDTW(List<Feature> seqTepl)
        {
            this.seqTepl = seqTepl;
            map = new List<double[]>();
        }

        public void handleNewFrame()
        {

        }
    }
}
