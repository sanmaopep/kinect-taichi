using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectCSharp.core
{
    // 动态时间规整算法
    public class DTW
    {
        private List<Feature> seqA;
        private List<Feature> seqB;
        private double[,] map;
        private double[,] dynamic;  // 动态规划，dynamic[i,j]代表到达i,j所用的最短路长度
        private int[,] road; // road[i,j]代表到达i,j之前的点(0斜对角，1->i-1,2->j-1)

        public DTW(List<Feature> seqA,List<Feature> seqB)
        {
            this.seqA = seqA;
            this.seqB = seqB;
            map = new double[seqA.Count, seqB.Count];
            dynamic = new double[seqA.Count, seqB.Count];
            road = new int[seqA.Count, seqB.Count];
        }

        // 计算相似度对比图
        private void cacualteMap()
        {
            for (int i =0;i < seqA.Count; i++)
            {
                for(int j = 0;j < seqB.Count; j++)
                {
                    map[i,j] = EDistance(seqA[i], seqB[j]);
                }
            }
        }

        // 直接计算关键帧最小值的总和,规定seqB为关键帧序列
        public double getMinSum()
        {
            cacualteMap();
            double sum = 0;
            for (int i = 0; i < seqB.Count; i++)
            {
                double min = map[0, i];
                for (int j = 1; j < seqA.Count; j++)
                {
                    if (map[j, i] < min)
                    {
                        min = map[j, i];
                    }
                }
                sum += min;
            }

            return sum;
        }

        // 动态规划最短路
        public void dynamicWarp()
        {
            cacualteMap();
            for (int i = 0; i < seqA.Count; i++)
            {
                for (int j = 0; j < seqB.Count; j++)
                {
                    if(i == 0 && j == 0)
                    {
                        dynamic[i,j] = map[i,j];
                        road[i, j] = -1;
                    }
                    else if(i == 0)
                    {
                        dynamic[i, j] = dynamic[i, j - 1] + map[i, j];
                        road[i, j] = 2;
                    }
                    else if(j == 0)
                    {
                        dynamic[i, j] = dynamic[i - 1, j] + map[i, j];
                        road[i, j] = 1;
                    }
                    else
                    {
                        goodMinResult result = goodMin(dynamic[i - 1, j - 1],dynamic[i - 1, j], dynamic[i, j - 1]);
                        road[i, j] = result.which;
                        dynamic[i, j] = result.min + map[i,j];
                    }
                }
            }
        }

        // 获得最终的最短路
        public double getFinalShortest()
        {
            return dynamic[seqA.Count - 1, seqB.Count - 1];
        }

        // 返回最大的数的下标和数字
        struct goodMinResult
        {
            public int which;
            public double min;
        }
        private goodMinResult goodMin(params double[] numList)
        {
            double min = numList[0];
            int which = 0;
            for(int i = 1;i < numList.Length; i++)
            {
                if(numList[i] < min)
                {
                    min = numList[i];
                    which = i;
                }
            }
            goodMinResult ret = new goodMinResult();
            ret.which = which;
            ret.min = min;
            return ret;
        }

        private double EDistance(Feature a,Feature b)
        {
            double sum = JointAngle.diffAngle(a.jointAngle,b.jointAngle);
            // 不用计算平方就可以比大小，所以不用Math.sqrt
            if(sum == 0)
            {
                return sum;
            }
            return Math.Sqrt(sum);
        }

        private double diffSquare(float a, float b)
        {
            double ret = util.Util.toDouble((a - b) * (a - b));
            return ret;
        }
    }
}
