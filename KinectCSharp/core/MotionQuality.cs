using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectCore.core
{
    // 运动量计算
    class MotionQuality
    {
        public List<Feature> featureBuffer;

        MotionQuality(List<Feature> featureBuffer)
        {
            this.featureBuffer = featureBuffer;
        }

        public double getMotionQualityAtFrame(int frameNum)
        {
            // 越界判断
            if(frameNum <= 0 || frameNum >= featureBuffer.Count)
            {
                return 0;
            }
            return Feature.EDistance(featureBuffer[frameNum-1],featureBuffer[frameNum]);
        }
    }
}
