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
        // 起势：WwGgCSw5PhQ.dat
        // 云手：EyyNAneWyWU.dat
        private const string MOTION_PATH = @"../../../MotionDataSet/EyyNAneWyWU.dat";

        [TestInitialize]
        public void beforeExperiment()
        {
            // kc.loadFromFile(MOTION_PATH);

            // /MSRC/P1_1_1_p06.csv
            kc.loadFromCSV(@"../../../MSRC/P2_2_2A_p25.csv");
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

        //// 30帧差异实验
        //[TestMethod]
        //public void moreFrameExperiment()
        //{
        //    for (int i = 30; i < kc.featureBuffer.Count; i++)
        //    {
        //        Feature before = kc.featureBuffer[i - 30];
        //        Feature after = kc.featureBuffer[i];
        //        Console.WriteLine(Feature.EDistance(before, after));
        //    }
        //}

        // DIFF帧内平均值差异
        [TestMethod]
        public void moreFrameAverageExperiment()
        {
            const int DIFF = 30;
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
