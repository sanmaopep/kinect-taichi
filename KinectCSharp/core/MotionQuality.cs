using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectCore.core
{
    // 运动量计算
    public class MotionQuality
    {
        public List<Feature> featureBuffer;
        public int DIFF = 60;

        public MotionQuality(List<Feature> featureBuffer)
        {
            this.featureBuffer = featureBuffer;
        }

        // 获取最新的运动量
        public double getLatestQuality()
        {
            int count = featureBuffer.Count;
            return getMotionQualityAtFrame(count - 1);
        }

        public double getMotionQualityAtFrame(int frameNum)
        {
            
            // 越界判断
            if (frameNum <= DIFF || frameNum >= featureBuffer.Count)
            {
                return 9999;
            }

            double sum = 0;
            Feature curr = featureBuffer[frameNum];
            if(curr.jointAngle == null)
            {
                curr.caculateAngle();
            }
            for (int i = frameNum - DIFF; i < frameNum; i++)
            {
                Feature before = featureBuffer[i];
                if(before.jointAngle == null)
                {
                    before.caculateAngle();
                }
                sum += Feature.EDistance(before, curr);
            }
            return sum/DIFF;
        }
    }
}
