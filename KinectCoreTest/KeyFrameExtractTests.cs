using Microsoft.VisualStudio.TestTools.UnitTesting;
using KinectCore.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectCoreTest
{
    [TestClass()]
    public class KeyFrameExtractTests
    {
        private const string FILE_PATH = "../../../MotionDataSet/";

        [TestMethod()]
        public void getDetailData()
        {
            // 获取详细数据
            KinectControl kinectControl = new KinectControl();
            kinectControl.loadFromFile(FILE_PATH + "test3.dat");
            Assert.AreNotEqual(kinectControl.featureBuffer.Count, 0);
            for(int i = 0;i < kinectControl.featureBuffer.Count; i++)
            {
                Console.WriteLine(kinectControl.featureBuffer[i].jointAngle.printSimple());
            }
        }

        [TestMethod()]
        public void KeyFrameDiffTest()
        {
            // 最大迭代时间
            const int MAX_INTERATION_COUNT = 10;

            // 测试两个关键帧之间的差异程度
            KinectControl kinectControlA = new KinectControl();
            kinectControlA.loadFromFile(FILE_PATH + "test3.dat");
            List<Feature> res;
            List<int> centers;
            KeyFrameExtract keyFrameExtract = new KeyFrameExtract(kinectControlA);

            for (int i = 0; i <= MAX_INTERATION_COUNT; i++)
            {
                Console.WriteLine("迭代" + i.ToString() + "次的结果");
                res = keyFrameExtract.caculateResultBuffer();
                centers = keyFrameExtract.kmeansCenterIndex;

                Assert.AreEqual(res.Count, centers.Count);

                Console.Write("关键帧为：");
                for(int j = 0;j < res.Count; j++)
                {
                    Console.Write(centers[j] + " ");
                }
                Console.WriteLine("差异程度为：");
                for (int j = 1; j < res.Count; j++)
                {
                    Console.Write(centers[j - 1] + " vs " + centers[j] + " : ");
                    Console.Write(JointAngle.diffAngle(res[j - 1].jointAngle, res[j].jointAngle));
                    Console.Write("\n");
                }


                keyFrameExtract.OneInterationKMeans();
            }
        }


    }
}