using System;
using System.Collections.Generic;
using KinectCore.core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KinectCoreTest
{
    [TestClass]
    public class RealTimeDTWExperiment
    {

        private KinectControl kcTpl = new KinectControl();
        private KinectControl kcPerson = new KinectControl();
        private KinectControl kcUnrelated = new KinectControl();
        private List<Feature> tplKeyframes;
        private CaculusKeyFrameExtract keyFrameExtract;
        private RealTimeDTW realTimeDTW;

        private const string MOTION_PATH1 = @"../../../MotionDataSet/test2.dat";
        private const string MOTION_PATH2 = @"../../../MotionDataSet/test3.dat";
        private const string MOTION_PATH3 = @"../../../MotionDataSet/test.dat";

        private int personProgress = 0;

        [TestInitialize]
        public void beforeExperiment()
        {
            kcPerson.loadFromFile(MOTION_PATH1, false);
            kcTpl.loadFromFile(MOTION_PATH2, false);
            kcUnrelated.loadFromFile(MOTION_PATH3, false);

            keyFrameExtract = new CaculusKeyFrameExtract(kcTpl.featureBuffer);
            keyFrameExtract.caculate();
            tplKeyframes = keyFrameExtract.getResultFeatures();

            realTimeDTW = new RealTimeDTW(tplKeyframes);

            // TODO 实验数据
            personProgress = 800;
        }


        // 实验一：同样动作的相似度曲线图
        [TestMethod]
        public void Experiement1()
        {
            List<Feature> features = kcPerson.featureBuffer;
            for (int i = 0; i < features.Count; i++)
            {
                for (int j = 0; j < tplKeyframes.Count; j++)
                {
                    double diff = Feature.EDistance(tplKeyframes[j], features[i]);
                    Console.Write(diff + "  ");
                }
                Console.Write("\n");
            }
        }

        // 实验二：自己和自己比的相似度曲线图
        [TestMethod]
        public void Experiement2()
        {
            List<Feature> features = kcTpl.featureBuffer;
            for (int i = 0; i < features.Count; i++)
            {
                for (int j = 0; j < tplKeyframes.Count; j++)
                {
                    double diff = Feature.EDistance(tplKeyframes[j], features[i]);
                    Console.Write(diff + "  ");
                }
                Console.Write("\n");
            }
        }

        // 实验三：不相干动作对比曲线图
        [TestMethod]
        public void Experiment3()
        {
            List<Feature> features = kcUnrelated.featureBuffer;
            for (int i = 0; i < features.Count; i++)
            {
                for (int j = 0; j < tplKeyframes.Count; j++)
                {
                    double diff = Feature.EDistance(tplKeyframes[j], features[i]);
                    Console.Write(diff + "  ");
                }
                Console.Write("\n");
            }
        }


        [TestMethod]
        public void RealTimeDTWTest()
        {
            realTimeDTW.clear();
            List<Feature> features = kcPerson.featureBuffer;

            for (int i = 0; i < personProgress; i++)
            {
                realTimeDTW.handleNewFrame(features[i]);
            }

            List<FDirection[]> road = realTimeDTW.getRoad();
            // 计算散点图
            int x = road.Count - 1;
            int y = road[0].Length - 1;
            while (x > 0 || y > 0)
            {
                Console.WriteLine(x + " " + y);
                switch (road[x][y])
                {
                    case FDirection.FROM_DOWN:
                        y--;
                        break;
                    case FDirection.FROM_LEFT:
                        x--;
                        break;
                    case FDirection.FROM_DOWN_LEFT:
                        x--;
                        y--;
                        break;
                    default:
                        break;
                }
            }
        }

        [TestMethod]
        public void ProgressIntTest()
        {
            realTimeDTW.clear();
            List<Feature> features = kcPerson.featureBuffer;

            for (int i = 0; i < personProgress; i++)
            {
                realTimeDTW.handleNewFrame(features[i]);
                Console.WriteLine(i + "  " + realTimeDTW.getProgressInt());
            }
        }


        [TestMethod]
        public void RealTimeDTW_ProgressIntTest()
        {
            realTimeDTW.clear();
            List<Feature> features = kcPerson.featureBuffer;

            for (int i = 0; i < personProgress; i++)
            {
                realTimeDTW.handleNewFrame(features[i]);
            }

            List<FDirection[]> road = realTimeDTW.getRoad();
            // 计算散点图
            int x = road.Count - 1;
            int y = realTimeDTW.getProgressInt() - 1;
            if (y < 0) y = 0;
            while (x > 0 || y > 0)
            {
                Console.WriteLine(x + " " + y);
                switch (road[x][y])
                {
                    case FDirection.FROM_DOWN:
                        y--;
                        break;
                    case FDirection.FROM_LEFT:
                        x--;
                        break;
                    case FDirection.FROM_DOWN_LEFT:
                        x--;
                        y--;
                        break;
                    default:
                        break;
                }
            }
        }


        [TestMethod]
        public void AverageSimilarityTest()
        {
            realTimeDTW.clear();
            List<Feature> features = kcPerson.featureBuffer;

            for (int i = 0; i < personProgress; i++)
            {
                realTimeDTW.handleNewFrame(features[i]);
                Console.WriteLine(realTimeDTW.getAverageSimilarity());
            }
        }
    }
}
