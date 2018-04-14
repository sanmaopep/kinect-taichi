using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KinectCoreTest
{
    using KinectCore.core;

    //[TestClass]
    public class DTWTest
    {
        private const string FILE_PATH = "../../../MotionDataSet/";

        // 冲拳动作直接对比测试
        [TestMethod]
        public void TestPunch()
        {
            KinectControl kinectControlA = new KinectControl();
            KinectControl kinectControlB = new KinectControl();
            double res;

            Console.WriteLine("比较Motion1和Motion3");
            kinectControlA.loadFromFile(FILE_PATH + "motion1.dat");
            kinectControlB.loadFromFile(FILE_PATH + "motion3.dat");
            DTW dtw = new DTW(kinectControlA.featureBuffer, kinectControlB.featureBuffer);
            dtw.dynamicWarp();
            res = dtw.getFinalShortest();
            Assert.AreNotEqual(double.IsNaN(res), true);
            Console.WriteLine(dtw.getFinalShortest());

            Console.WriteLine("比较Motion1和Motion1");
            kinectControlB.emptyBuffer();
            kinectControlB.loadFromFile(FILE_PATH + "motion1.dat");
            dtw = new DTW(kinectControlA.featureBuffer, kinectControlB.featureBuffer);
            dtw.dynamicWarp();
            res = dtw.getFinalShortest();
            Assert.AreNotEqual(double.IsNaN(res), true);
            Console.WriteLine(res);

            Console.WriteLine("比较Motion1和Motion2");
            kinectControlB.emptyBuffer();
            kinectControlB.loadFromFile(FILE_PATH + "motion2.dat");
            dtw = new DTW(kinectControlA.featureBuffer, kinectControlB.featureBuffer);
            dtw.dynamicWarp();
            res = dtw.getFinalShortest();
            Assert.AreNotEqual(double.IsNaN(res), true);
            Console.WriteLine(res);

            Console.WriteLine("比较Motion1和Test");
            kinectControlB.emptyBuffer();
            kinectControlB.loadFromFile(FILE_PATH + "test.dat");
            dtw = new DTW(kinectControlA.featureBuffer, kinectControlB.featureBuffer);
            dtw.dynamicWarp();
            res = dtw.getFinalShortest();
            Assert.AreNotEqual(double.IsNaN(res), true);
            Console.WriteLine(res);
        }

        // 测试Kmeans迭代次数对相似度的影响
        [TestMethod]
        public void TestKeyMeansOrientationInfluenceSimilarityCase1()
        {
            // 最大迭代次数
            const int MAX_INTERATION_COUNT = 10;
            KinectControl kinectControlA = new KinectControl();
            KinectControl kinectControlB = new KinectControl();
            kinectControlA.loadFromFile(FILE_PATH + "motion1.dat");
            kinectControlB.loadFromFile(FILE_PATH + "motion3.dat");
            KeyFrameExtract keyFrameExtract = new KeyFrameExtract(kinectControlA);
            double res;

            Console.WriteLine("比较Motion1和Motion3");
            Console.WriteLine("直接对比结果：");
            DTW dtw = new DTW(kinectControlA.featureBuffer, kinectControlB.featureBuffer);
            dtw.dynamicWarp();
            res = dtw.getFinalShortest();
            Assert.AreNotEqual(double.IsNaN(res), true);
            Console.WriteLine(res);

            Console.WriteLine("迭代0次的结果");
            dtw = new DTW(kinectControlB.featureBuffer, keyFrameExtract.caculateResultBuffer());
            dtw.dynamicWarp();
            res = dtw.getFinalShortest();
            Assert.AreNotEqual(double.IsNaN(res), true);
            Console.WriteLine(res);

            for(int i = 1;i <= MAX_INTERATION_COUNT; i++)
            {
                Console.WriteLine("迭代"+i.ToString()+"次的结果");
                keyFrameExtract.OneInterationKMeans();
                dtw = new DTW(kinectControlB.featureBuffer, keyFrameExtract.caculateResultBuffer());
                dtw.dynamicWarp();
                res = dtw.getFinalShortest();
                Assert.AreNotEqual(double.IsNaN(res), true);
                Console.WriteLine(res);
            }
        }

        [TestMethod]
        public void TestKeyMeansOrientationInfluenceSimilarityCase2()
        {
            // 最大迭代次数
            const int MAX_INTERATION_COUNT = 10;
            KinectControl kinectControlA = new KinectControl();
            KinectControl kinectControlB = new KinectControl();
            kinectControlA.loadFromFile(FILE_PATH + "motion1.dat");
            kinectControlB.loadFromFile(FILE_PATH + "motion2.dat");
            KeyFrameExtract keyFrameExtract = new KeyFrameExtract(kinectControlA);
            double res;

            Console.WriteLine("比较Motion1和Motion2");
            Console.WriteLine("直接对比结果：");
            DTW dtw = new DTW(kinectControlA.featureBuffer, kinectControlB.featureBuffer);
            dtw.dynamicWarp();
            res = dtw.getFinalShortest();
            Assert.AreNotEqual(double.IsNaN(res), true);
            Console.WriteLine(res);

            Console.WriteLine("迭代0次的结果");
            dtw = new DTW(kinectControlB.featureBuffer, keyFrameExtract.caculateResultBuffer());
            dtw.dynamicWarp();
            res = dtw.getFinalShortest();
            Assert.AreNotEqual(double.IsNaN(res), true);
            Console.WriteLine(res);

            for (int i = 1; i <= MAX_INTERATION_COUNT; i++)
            {
                Console.WriteLine("迭代" + i.ToString() + "次的结果");
                keyFrameExtract.OneInterationKMeans();
                dtw = new DTW(kinectControlB.featureBuffer, keyFrameExtract.caculateResultBuffer());
                dtw.dynamicWarp();
                res = dtw.getFinalShortest();
                Assert.AreNotEqual(double.IsNaN(res), true);
                Console.WriteLine(res);
            }
        }

        [TestMethod]
        public void TestKeyMeansOrientationInfluenceMinSum()
        {
            // 最大迭代次数
            const int MAX_INTERATION_COUNT = 10;
            KinectControl kinectControlA = new KinectControl();
            KinectControl kinectControlB = new KinectControl();
            kinectControlA.loadFromFile(FILE_PATH + "motion1.dat");
            kinectControlB.loadFromFile(FILE_PATH + "motion2.dat");
            KeyFrameExtract keyFrameExtract = new KeyFrameExtract(kinectControlA);
            double res;

            Console.WriteLine("比较Motion1和Motion2");
            Console.WriteLine("直接对比结果：");
            DTW dtw = new DTW(kinectControlA.featureBuffer, kinectControlB.featureBuffer);
            dtw.dynamicWarp();
            res = dtw.getFinalShortest();
            Assert.AreNotEqual(double.IsNaN(res), true);
            Console.WriteLine(dtw.getFinalShortest());

            Console.WriteLine("迭代0次的结果");
            dtw = new DTW(kinectControlB.featureBuffer, keyFrameExtract.caculateResultBuffer());
            res = dtw.getMinSum();
            Assert.AreNotEqual(double.IsNaN(res), true);
            Console.WriteLine(res);

            for (int i = 1; i <= MAX_INTERATION_COUNT; i++)
            {
                Console.WriteLine("迭代" + i.ToString() + "次的结果");
                keyFrameExtract.OneInterationKMeans();
                dtw = new DTW(kinectControlB.featureBuffer, keyFrameExtract.caculateResultBuffer());
                res = dtw.getMinSum();
                Assert.AreNotEqual(double.IsNaN(res), true);
                Console.WriteLine(res);
            }
        }

        // 测试关键帧提取对识别动作的效果
        [TestMethod]
        public void TestKeyFrameSelect()
        {
            KinectControl kinectControlA = new KinectControl();
            KinectControl kinectControlB = new KinectControl();
            KinectControl kinectControlC = new KinectControl();
            KinectControl kinectControlD = new KinectControl();

            kinectControlA.loadFromFile(FILE_PATH + "motion1.dat");
            kinectControlB.loadFromFile(FILE_PATH + "motion2.dat");
            kinectControlC.loadFromFile(FILE_PATH + "motion3.dat");
            kinectControlD.loadFromFile(FILE_PATH + "test.dat");

            KeyFrameExtract keyFrameExtract = new KeyFrameExtract(kinectControlA);
            double res;
            DTW dtw;

            Console.WriteLine("motion2和motion1关键帧对比");
            dtw = new DTW(kinectControlB.featureBuffer, keyFrameExtract.caculateResultBuffer());
            res = dtw.getMinSum();
            Assert.AreNotEqual(double.IsNaN(res), true);
            Console.WriteLine(res);

            Console.WriteLine("motion3和motion1关键帧对比");
            dtw = new DTW(kinectControlC.featureBuffer, keyFrameExtract.caculateResultBuffer());
            res = dtw.getMinSum();
            Assert.AreNotEqual(double.IsNaN(res), true);
            Console.WriteLine(res);


            Console.WriteLine("test和motion1关键帧对比");
            dtw = new DTW(kinectControlD.featureBuffer, keyFrameExtract.caculateResultBuffer());
            res = dtw.getMinSum();
            Assert.AreNotEqual(double.IsNaN(res), true);
            Console.WriteLine(res);
        }



        // end
    }
}
