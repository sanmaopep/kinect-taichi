using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KinectCore.util;
using KinectCore.model;

namespace KinectCoreTest
{

    [TestClass]
    public class MotionLibsUtilTest
    {
        [TestMethod]
        public void testParseJson()
        {
            string testStr = @"[     {         'title': '金刚捣碓',         'description': '金刚捣碓是太极拳基本动作之一。在陈式太极拳中，作为太极起势后的第一个动作，有平心静气、提振声势的作用。',         'data': './test.dat',         'details': [             {                 'from': 1,                 'to': 30,                 'description': '双掌左逆缠，右顺缠，左掤，左手为叼手，腕与肩同高，右手为托手，置于左手右下方，双手距一肘，双肘松沉。松左胯，重心移至右腿。身体左转45度。'             }         ]     },     {         'title': '懒扎衣',         'description': '金刚捣碓是太极拳基本动作之一。在陈式太极拳中，作为太极起势后的第一个动作，有平心静气、提振声势的作用。',         'data': './test.dat',         'details': [             {                 'from': 1,                 'to': 30,                 'description': '双掌左逆缠，右顺缠，左掤，左手为叼手，腕与肩同高，右手为托手，置于左手右下方，双手距一肘，双肘松沉。松左胯，重心移至右腿。身体左转45度。'             }         ]     } ]";
            SingleMotionModel[] models = MotionLibsUtil.parseJson(testStr);
            Assert.AreEqual(models.Length, 2);
        }

        [TestMethod]
        public void testParseFromFile()
        {
            string filePath = @"../../../MotionDataSet/motions.json";
            SingleMotionModel[] models = MotionLibsUtil.parseFromFile(filePath);
            Assert.AreEqual(models.Length, 2);
        }
    }
}
