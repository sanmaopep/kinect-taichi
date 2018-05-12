using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectCore.core
{
    // 基于积分的关键帧提取
    public class CaculusKeyFrameExtract
    {
        private List<Feature> featureBuffer;
        private List<int> resultIndexs = new List<int>();
        private List<Feature> resultFeatures = new List<Feature>();
        private const double KEY_FRAME_CACULUS = 60 * 20; //运动量60做30帧动作
        private const double LEAST = 20;    // 小于该运动量忽略加入积分中


        public CaculusKeyFrameExtract(List<Feature> features)
        {
            featureBuffer = features;
        }

        public List<int> getResultIndexs()
        {
            return resultIndexs;
        }

        public List<Feature> getResultFeatures()
        {
            return resultFeatures;
        }

        public void caculate()
        {
            MotionQuality motionQuality = new MotionQuality(featureBuffer);
            List<double> motionQualityList = new List<double>();

            for (int i = 0; i < featureBuffer.Count; i++)
            {
                motionQualityList.Add(motionQuality.getMotionQualityAtFrame(i));
            }

            double refrenceMotionQuality = motionQualityList[motionQuality.DIFF + 1];
            double refrenceDiff = refrenceMotionQuality / (motionQuality.DIFF + 1);
            double curr = 0;

            // 前面几帧渐变上升
            for (int i = 0; i < motionQuality.DIFF + 1; i++)
            {
                motionQualityList[i] = curr;
                curr += refrenceDiff;
            }

            // 第0帧应该加入观察
            resultIndexs.Add(0);

            double caculus = 0;
            for (int i = 0; i < motionQualityList.Count; i++)
            {
                caculus += motionQualityList[i] > LEAST ? motionQualityList[i] : 0;
                if (caculus > KEY_FRAME_CACULUS)
                {
                    resultIndexs.Add(i);
                    caculus = 0;
                }
            }

            // 最后一帧也要加入观察
            resultIndexs.Add(motionQualityList.Count - 1);

            // 实验结果分析
            for (int i = 0; i < resultIndexs.Count; i++)
            {
                resultFeatures.Add(featureBuffer[resultIndexs[i]]);
            }
        }
    }
}
