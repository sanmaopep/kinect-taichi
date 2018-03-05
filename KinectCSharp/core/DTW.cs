using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectCSharp.core
{
    // 动态时间规整算法
    class DTW
    {
        private List<Feature> seqA;
        private List<Feature> seqB;
        double[,] map;

        DTW(List<Feature> seqA,List<Feature> seqB)
        {
            this.seqA = seqA;
            this.seqB = seqB;
            map = new double[seqA.Count, seqB.Count];
        }

        // 计算相似度对比图
        private void cacualteMap()
        {

        }

        // 运行计算
        private void run()
        {

        }

        private double EDistance(Feature a,Feature b)
        {
            double sum = 0.0;
            // TODO 等待添加
            sum += diffSquare(a.jointAngle.ElobwLeft, b.jointAngle.ElobwLeft);
            sum += diffSquare(a.jointAngle.ElobwRight, b.jointAngle.ElobwRight);

            // 不用计算平方就可以比大小，所以不用Math.sqrt
            return sum;
        }

        private double diffSquare(float a, float b)
        {
            return util.Util.toDouble((a - b) * (a - b));
        }
    }
}
