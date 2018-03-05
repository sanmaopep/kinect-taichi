using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectCSharp.core
{
    using util;

    class KeyFrameExtract
    {
        private List<Feature> originBuffer;
        public List<Feature> resultBuffer = new List<Feature>();
        public List<int> kmeansCenterIndex = new List<int>(); // 质心的集合（迭代的结果）
        public List<List<int>> kmeansSet = new List<List<int>>();
        public readonly int CENTER_GAP = 30; // 每隔几帧提取一个初始化质心关键帧
        public float timeRatio = 1;    // 时间的伸缩比例（时间影响因子）
        private int k;  // 聚心个数

        public KeyFrameExtract(KinectControl kinectControl)
        {
            this.originBuffer = kinectControl.featureBuffer;
            InitializeKmeansCenters();
        }

        // 初始化质心
        private void InitializeKmeansCenters()
        {
            k = 0;
            for(int i = 0;i < originBuffer.Count;i+=CENTER_GAP)
            {
                kmeansCenterIndex.Add(i);
                kmeansSet.Add(new List<int>());
                k++;
            }
            // 把最后一个点也加入质心
            if(originBuffer.Count % CENTER_GAP != 0)
            {
                kmeansCenterIndex.Add(originBuffer.Count -1);
                kmeansSet.Add(new List<int>());
                k++;
            }
        }
        
        // 进行一次迭代
        public void OneInterationKMeans()
        {
            // 清空kmeansSet
            for(int i = 0;i < kmeansSet.Count; i++)
            {
                kmeansSet[i].Clear();
            }
            // 对于每一个点，找到最近的质心
            for(int i = 0;i < originBuffer.Count; i++)
            {
                int minClass = 0;
                double minValue = EDistanceWithTimeAxis(i,kmeansCenterIndex[0]);
                for(int j = 1;j < kmeansCenterIndex.Count; j++)
                {
                    double currValue = EDistanceWithTimeAxis(i, kmeansCenterIndex[j]);
                    if(currValue < minValue)
                    {
                        minValue = currValue;
                        minClass = j;
                    }
                }
                // 该点加入对应的类
                kmeansSet[minClass].Add(i);
            }
            // TODO 计算每一个集合新的质心(目前简单计算时间维度的中心)
            for(int i = 0;i < kmeansSet.Count; i++)
            {
                int sum = 0;
                for(int j = 0;j < kmeansSet[i].Count; j++)
                {
                    sum += kmeansSet[i][j];
                }
                if(sum != 0)
                {
                    kmeansCenterIndex[i] = sum / kmeansSet[i].Count;
                }
            }
            kmeansCenterIndex.Sort();
        }

        public List<Feature> caculateResultBuffer()
        {
            resultBuffer.Clear();
            Console.WriteLine("Kmeans帧数：");
            for (int i = 0; i < kmeansCenterIndex.Count; i++)
            {
                Console.WriteLine(kmeansCenterIndex[i]);
                resultBuffer.Add(originBuffer[kmeansCenterIndex[i]]);
            }
            return resultBuffer;
        }

        // 计算距离
        public double EDistanceWithTimeAxis(int aIndex,int bIndex)
        {
            double sum = 0.0;
            Feature a = originBuffer[aIndex];
            Feature b = originBuffer[bIndex];

            // TODO 等待添加
            sum += diffSquare(aIndex * timeRatio, bIndex * timeRatio);
            sum += diffSquare(a.jointAngle.ElobwLeft, b.jointAngle.ElobwLeft);
            sum += diffSquare(a.jointAngle.ElobwRight, b.jointAngle.ElobwRight);

            // 不用计算平方就可以比大小，所以不用Math.sqrt
            return sum;
        }


        // 计算差的平方
        private double diffSquare(double a,double b)
        {
            return (a - b) * (a - b);
        }
        private double diffSquare(int a, int b)
        {
            return (a - b) * (a - b);
        }
        private double diffSquare(float a, float b)
        {
            return util.Util.toDouble((a - b) * (a - b));
        }
    }
}
