using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectCore.util
{
    using System.IO;
    using Microsoft.Kinect;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    class Util
    {
        // 相互转换
        public static float toFloat(double a)
        {
            return float.Parse(a.ToString());
        }

        public static double toDouble(float a)
        {
            return double.Parse(a.ToString());
        }

        // 计算差的平方
        public static double diffSquare(double a, double b)
        {
            return (a - b) * (a - b);
        }
        public static double diffSquare(int a, int b)
        {
            return (a - b) * (a - b);
        }
        public static double diffSquare(float a, float b)
        {
            return util.Util.toDouble((a - b) * (a - b));
        }
    }
}
