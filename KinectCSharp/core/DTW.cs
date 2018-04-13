using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectCore.core
{
    using util;

    // 在Map上行走的方向
    public enum Direction
    {
        NOTHING = -1,
        UP = 0,
        RIGHT = 1,
        UP_RIGHT = 2
    }

    // TODO 动态时间规整算法
    public class DTW
    {
        private List<Feature> seqA;
        private List<Feature> seqB;
        private double[,] map;
        private double[,] dynamic;  // 动态规划，dynamic[i,j]代表到达i,j所用的最短路长度
        private Direction[,] road; // road[i,j]代表到达i,j之前的点(0斜对角，1->i-1,2->j-1)
        private int[,] roadLen; // 代表到达i,j的路径长度
        
        public DTW(List<Feature> seqA,List<Feature> seqB)
        {
            this.seqA = seqA;
            this.seqB = seqB;
            map = new double[seqA.Count, seqB.Count];
            dynamic = new double[seqA.Count, seqB.Count];
            road = new Direction[seqA.Count, seqB.Count];
            roadLen = new int[seqA.Count, seqB.Count];
        }

        // 计算相似度对比图
        public void cacualteMap()
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

            return sum/seqB.Count;
        }

        public void dynamicWarpOneRow(int i)
        {
            for (int j = 0; j < seqB.Count; j++)
            {
                map[i, j] = EDistance(seqA[i], seqB[j]);
            }
            for (int j = 0; j < seqB.Count; j++)
            {
                if (i == 0 && j == 0)
                {
                    dynamic[i, j] = map[i, j];
                    roadLen[i, j] = 1;
                    road[i, j] = Direction.NOTHING;
                }
                else if (i == 0)
                {
                    dynamic[i, j] = dynamic[i, j - 1] + map[i, j];
                    roadLen[i, j] = roadLen[i, j - 1] + 1;
                    road[i, j] = Direction.RIGHT;
                }
                else if (j == 0)
                {
                    dynamic[i, j] = dynamic[i - 1, j] + map[i, j];
                    roadLen[i, j] = roadLen[i - 1, j] + 1;
                    road[i, j] = Direction.UP;
                }
                else
                {
                    goodMinResult result = goodMin(dynamic[i - 1, j - 1],
                        dynamic[i - 1, j],
                        dynamic[i, j - 1]);
                    road[i, j] = (Direction)(result.which);
                    dynamic[i, j] = result.min + map[i, j];
                    switch (road[i, j])
                    {
                        case Direction.UP_RIGHT:
                            roadLen[i, j] = roadLen[i - 1, j - 1] + 1;
                            break;
                        case Direction.UP:
                            roadLen[i, j] = roadLen[i - 1, j] + 1;
                            break;
                        case Direction.RIGHT:
                            roadLen[i, j] = roadLen[i, j - 1] + 1;
                            break;
                    }
                }
            }
         }

        // 动态规划最短路
        public void dynamicWarp()
        {
            cacualteMap();
            for (int i = 0; i < seqA.Count; i++)
            {
                dynamicWarpOneRow(i);
            }
        }

        // 获得最终的最短路
        public double getFinalShortest()
        {
            return dynamic[seqA.Count - 1, seqB.Count - 1]/roadLen[seqA.Count - 1, seqB.Count - 1];
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

        public double EDistance(Feature a,Feature b)
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
            double ret = Util.toDouble((a - b) * (a - b));
            return ret;
        }
    }
}
