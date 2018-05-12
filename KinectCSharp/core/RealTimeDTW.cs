using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectCore.core
{
    // From Direction
    public enum FDirection
    {
        NOTHING = -1,
        FROM_DOWN = 0,
        FROM_LEFT = 1,
        FROM_DOWN_LEFT = 2
    }

    // 实时DTW计算
    public class RealTimeDTW
    {
        public int PROGRESS_THRESHOLD = 70; // 小于该值上升一个模板帧
        private List<Feature> seqTepl;  // 模板动作
        private List<double[]> dynamic;
        private List<double[]> map;
        private List<FDirection[]> road;
        private int progess = 0;

        public RealTimeDTW(List<Feature> seqTepl)
        {
            this.seqTepl = seqTepl;
            dynamic = new List<double[]>();
            map = new List<double[]>();
            road = new List<FDirection[]>();
        }


        public bool firstFrameReceive(Feature feature)
        {
            return Feature.EDistance(feature, seqTepl[0]) < PROGRESS_THRESHOLD;
        }

        // 处理一个新的帧
        public void handleNewFrame(Feature newFrame)
        {
            double[] dynamicColumn = new double[seqTepl.Count];
            double[] mapColumn = new double[seqTepl.Count];
            FDirection[] directionColumn = new FDirection[seqTepl.Count];

            int lastIndex = dynamic.Count - 1;

            for (int i = 0; i < seqTepl.Count; i++)
            {
                mapColumn[i] = Feature.EDistance(seqTepl[i], newFrame);
            }

            // 进度推进
            if (progess < seqTepl.Count && mapColumn[progess] < PROGRESS_THRESHOLD)
            {
                progess++;
            }


            // 如果是第一列
            if (dynamic.Count == 0)
            {
                dynamicColumn[0] = mapColumn[0];
                directionColumn[0] = FDirection.NOTHING;
                for (int i = 1; i < seqTepl.Count; i++)
                {
                    double currDiff = mapColumn[i];
                    dynamicColumn[i] = currDiff + dynamicColumn[i - 1];
                    directionColumn[i] = FDirection.FROM_DOWN;
                }
            }
            else
            {
                dynamicColumn[0] = dynamic[lastIndex][0] + mapColumn[0];
                directionColumn[0] = FDirection.FROM_LEFT;

                for (int i = 1; i < seqTepl.Count; i++)
                {
                    double currDiff = mapColumn[i];

                    double[] dynamics = new double[3];
                    dynamics[(int)FDirection.FROM_LEFT] = dynamic[lastIndex][i];
                    dynamics[(int)FDirection.FROM_DOWN] = dynamicColumn[i - 1];
                    dynamics[(int)FDirection.FROM_DOWN_LEFT] = dynamic[lastIndex][i - 1];

                    int which = MinIndex(dynamics);

                    dynamicColumn[i] = currDiff + dynamics[which];
                    directionColumn[i] = (FDirection)which;
                }
            }

            map.Add(mapColumn);
            dynamic.Add(dynamicColumn);
            road.Add(directionColumn);
        }

        public List<FDirection[]> getRoad()
        {
            return road;
        }

        // 获取进度的帧
        public int getProgressInt()
        {
            return progess;
        }

        // 清空缓存数据
        public void clear()
        {
            progess = 0;
            map.Clear();
            dynamic.Clear();
            road.Clear();
        }

        // 获取得分
        private int SCORE_THRESHOLD = 25;
        public double getScore()
        {
            if (getAverageSimilarity() > 100)
            {
                return 0;
            }
            if (getAverageSimilarity() < SCORE_THRESHOLD)
            {
                return 100;
            }
            // 25以内都算正常的动作
            return Math.Floor(100 - getAverageSimilarity()) + SCORE_THRESHOLD;
        }

        // 获取平均相似度
        public double getAverageSimilarity()
        {
            double sum = 0;
            int counter = 0;
            // 计算散点图
            int x = road.Count - 1;
            int y = progess;
            if (y >= seqTepl.Count)
            {
                y--;
            }
            while (x > 0 || y > 0)
            {
                switch (road[x][y])
                {
                    case FDirection.FROM_DOWN:
                        y--;
                        break;
                    case FDirection.FROM_LEFT:
                        sum += map[x][y];
                        counter++;
                        x--;
                        break;
                    case FDirection.FROM_DOWN_LEFT:
                        sum += map[x][y];
                        counter++;
                        x--;
                        y--;
                        break;
                    default:
                        break;
                }
            }

            return sum / counter;
        }

        // 获取最小值所在的索引
        private int MinIndex(double[] numList)
        {
            double min = numList[0];
            int which = 0;
            for (int i = 1; i < numList.Length; i++)
            {
                if (numList[i] < min)
                {
                    min = numList[i];
                    which = i;
                }
            }
            return which;
        }

    }
}
