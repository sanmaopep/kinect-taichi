using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectCSharp.core
{
    using Microsoft.Kinect;
    using System.IO;

    class KinectControl
    {
        private KinectSensor sensor;

        public KinectControl()
        {
            this.Initialize();
        }

        // 初始化Kinect
        private void Initialize()
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
                if (null != this.sensor)
                {
                    // Turn on the skeleton stream to receive skeleton frames
                    this.sensor.SkeletonStream.Enable();


                    // Start the sensor!
                    try
                    {
                        this.sensor.Start();
                    }
                    catch (IOException)
                    {
                        this.sensor = null;
                    }
                }
            }
        }
    }
}
