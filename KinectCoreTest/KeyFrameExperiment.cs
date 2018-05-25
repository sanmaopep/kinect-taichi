using System;
using System.Collections.Generic;
using KinectCore.core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KinectCoreTest
{
    // 基于运动量积分改进的动作关键帧提取
    [TestClass]
    public class KeyFrameExperiment
    {
        private KinectControl kcTpl = new KinectControl();
        private KinectControl kcPerson = new KinectControl();
        private KinectControl kcUnrelated = new KinectControl();

        // GIUuQOpeDuo
        private const string MOTION_PATH1 = @"../../../MotionDataSet/test2.dat";
        //  private const string MOTION_PATH2 = @"../../../MotionDataSet/test3.dat";
        private const string MOTION_PATH2 = @"../../../MotionDataSet/EyyNAneWyWU.dat";
        private const string MOTION_PATH3 = @"../../../MotionDataSet/test.dat";


        [TestInitialize]
        public void beforeExperiment()
        {
            kcTpl.loadFromFile(MOTION_PATH2, false);
            kcPerson.loadFromFile(MOTION_PATH1, false);
            kcUnrelated.loadFromFile(MOTION_PATH3, false);

        }

        [TestMethod]
        public void experimentCaculus()
        {
            MotionQuality motionQuality = new MotionQuality(kcTpl.featureBuffer);
            motionQuality.DIFF = 30;
            const double LEAST = 20;
            const double KEY_FRAME_CACULUS = 30 * 30; //运动量60做30帧动作
            List<double> motionQualityList = new List<double>();
            List<int> resultIndexs = new List<int>();

            for (int i = 0; i < kcTpl.featureBuffer.Count; i++)
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


            // 实验结果分析
            for (int i = 0; i < resultIndexs.Count; i++)
            {
                //cConsole.Write(resultIndexs[i] + " ");
                if (i != 0)
                {
                    Feature before = kcTpl.featureBuffer[resultIndexs[i-1]];
                    Feature after = kcTpl.featureBuffer[resultIndexs[i]];
                    Console.Write(Feature.EDistance(before, after));
                }
                Console.Write("\n");
            }
        }

    }
}
