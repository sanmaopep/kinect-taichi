using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KinectCoreTest
{
    using KinectCore.core;

    [TestClass]
    public class MSRC_DB_Experiment
    {

        KinectControl seqA = new KinectControl();
        KinectControl seqB = new KinectControl();

        string[,] filenames = new string[5, 2]
        {
            { "",""},
            { "",""},
            { "",""},
            { "",""},
            { "",""}
        };

        [TestInitialize]
        public void init()
        {

        }

        [TestMethod]
        public void compareSimilarity(string filename1,string filename2)
        {

        }


        
        private bool match(string filename1,string filename2)
        {
            seqA.loadFromCSV(filename1);
            seqB.loadFromCSV(filename2);
            return false;
        }

    }
}
