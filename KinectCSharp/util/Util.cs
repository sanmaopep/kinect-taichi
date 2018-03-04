﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectCSharp.util
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

        public static CoordinateMapper GetCoordinateMapper()
        {
            byte[] parameters = new byte[12604];
            CoordinateMapper coordinateMapper = new CoordinateMapper(parameters);
            return coordinateMapper;
        }

        /// <summary> 
        /// 将一个object对象序列化，返回一个byte[]         
        /// </summary> 
        /// <param name="obj">能序列化的对象</param>         
        /// <returns></returns> 
        public static byte[] ObjectToBytes(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                return ms.GetBuffer();
            }
        }

        /// <summary> 
        /// 将一个序列化后的byte[]数组还原         
        /// </summary>
        /// <param name="Bytes"></param>         
        /// <returns></returns> 
        public static object BytesToObject(byte[] Bytes)
        {
            using (MemoryStream ms = new MemoryStream(Bytes))
            {
                IFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(ms);
            }
        }
    }
}
