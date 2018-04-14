using System;
using KinectCore.core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KinectCoreTest
{
    // 运动量相关实验测试
    [TestClass]
    public class MotionQualityExperiment
    {
        private KinectControl kc = new KinectControl();
        private const string MOTION_PATH = @"../../../MotionDataSet/test2.dat";


        [TestInitialize]
        public void beforeExperiment()
        {
            kc.loadFromFile(MOTION_PATH);
        }

        // 单帧前后差异计算的运动量
        [TestMethod]
        public void singleFrameExperiment()
        {
            for (int i = 1; i < kc.featureBuffer.Count; i++)
            {
                Feature before = kc.featureBuffer[i - 1];
                Feature after = kc.featureBuffer[i];
                Console.WriteLine(Feature.EDistance(before, after));
            }

        }

        // 30帧差异实验
        [TestMethod]
        public void moreFrameExperiment()
        {
            for (int i = 30; i < kc.featureBuffer.Count; i++)
            {
                Feature before = kc.featureBuffer[i - 30];
                Feature after = kc.featureBuffer[i];
                Console.WriteLine(Feature.EDistance(before, after));
            }
        }

        // 30帧内平均值差异
        [TestMethod]
        public void moreFrameAverageExperiment()
        {
            const int DIFF = 100;
            for (int i = DIFF; i < kc.featureBuffer.Count; i++)
            {
                double sum = 0;
                Feature curr = kc.featureBuffer[i];
                for(int j = i - DIFF; j < i; j++)
                {
                    Feature before = kc.featureBuffer[j];
                    sum += Feature.EDistance(before,curr);
                }

                Console.WriteLine(sum/ DIFF);
            }
        }
    }
}
