using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectCore.core
{
    //TODO 模板动作角度的存在也要进行判断哦
    public class TrainAnalysis
    {
        private List<Feature> tplFeatures;
        private float THRESHOLD = 20;

        public TrainAnalysis(List<Feature> tplFeatures)
        {
            this.tplFeatures = tplFeatures;
        }

        // 模板帧与人进行对比
        public string Analysis(int currFrame, Feature person)
        {
            string res = "";
            if (person.skeleton == null)
            {
                return "没有检测到人体";
            }
            if (currFrame < 0 || currFrame >= tplFeatures.Count)
            {
                return res;
            }
            Feature curr = tplFeatures[currFrame];

            res += Elbow(curr, person);
            res += Shoulder(curr, person);
            res += Rotate(curr, person);
            res += Hip(curr, person);
            res += Knee(curr, person);

            if(res != "")
            {
                return res.Substring(0, res.IndexOf(','));
            }
            return res;
        }

        // 肘关节分析
        private string Elbow(Feature tpl, Feature person)
        {
            string res = "";

            if (person.skeleton.Joints[JointType.ElbowLeft].TrackingState == JointTrackingState.Tracked)
            {
                if (person.jointAngle.ElbowLeft - tpl.jointAngle.ElbowLeft > THRESHOLD)
                {
                    res += "左肘张得太开,";
                }
                if (person.jointAngle.ElbowLeft - tpl.jointAngle.ElbowLeft < -THRESHOLD)
                {
                    res += "左肘缩得太紧,";
                }
            }

            if (person.skeleton.Joints[JointType.ElbowRight].TrackingState == JointTrackingState.Tracked)
            {
                if (person.jointAngle.ElbowRight - tpl.jointAngle.ElbowRight > THRESHOLD)
                {
                    res += "右肘张得太开,";
                }
                if (person.jointAngle.ElbowRight - tpl.jointAngle.ElbowRight < -THRESHOLD)
                {
                    res += "右肘缩得太紧,";
                }
            }

            return res;
        }


        // 肩关节分析
        private string Shoulder(Feature tpl, Feature person)
        {
            string res = "";

            if (person.skeleton.Joints[JointType.ShoulderLeft].TrackingState == JointTrackingState.Tracked)
            {
                if (person.jointAngle.ShoulderLeft.Y - tpl.jointAngle.ShoulderLeft.Y > THRESHOLD)
                {
                    res += "左臂位置下降点,";
                }
                if (person.jointAngle.ShoulderLeft.Y - tpl.jointAngle.ShoulderLeft.Y < -THRESHOLD)
                {
                    res += "左臂位置上升点,";
                }
                //if (person.jointAngle.ShoulderLeft.X - tpl.jointAngle.ShoulderLeft.X > THRESHOLD)
                //{
                //    res += "左臂位置后面点,";
                //}
                //if (person.jointAngle.ShoulderLeft.X - tpl.jointAngle.ShoulderLeft.X < -THRESHOLD)
                //{
                //    res += "左臂位置前面点,";
                //}
            }

            if (person.skeleton.Joints[JointType.ShoulderRight].TrackingState == JointTrackingState.Tracked)
            {
                if (person.jointAngle.ShoulderRight.Y - tpl.jointAngle.ShoulderRight.Y > THRESHOLD)
                {
                    res += "右臂位置下降点,";
                }
                if (person.jointAngle.ShoulderRight.Y - tpl.jointAngle.ShoulderRight.Y < -THRESHOLD)
                {
                    res += "右臂位置上升点,";
                }
                //if (person.jointAngle.ShoulderRight.X - tpl.jointAngle.ShoulderRight.X > THRESHOLD)
                //{
                //    res += "右臂位置后面点,";
                //}
                //if (person.jointAngle.ShoulderRight.X - tpl.jointAngle.ShoulderRight.X < -THRESHOLD)
                //{
                //    res += "右臂位置前面点,";
                //}
            }

            return res;
        }

        // 身体旋转分析
        private string Rotate(Feature tpl, Feature person)
        {
            string res = "";

            if (person.skeleton.Joints[JointType.ShoulderLeft].TrackingState == JointTrackingState.Tracked
                && person.skeleton.Joints[JointType.ShoulderRight].TrackingState == JointTrackingState.Tracked)
            {
                if (person.jointAngle.ShoulderLeft.X - tpl.jointAngle.ShoulderLeft.X > THRESHOLD
                    && person.jointAngle.ShoulderRight.X - tpl.jointAngle.ShoulderRight.X > THRESHOLD)
                {
                    res += "身体左转一点,";
                }
                if (person.jointAngle.ShoulderLeft.X - tpl.jointAngle.ShoulderLeft.X < -THRESHOLD
                    && person.jointAngle.ShoulderRight.X - tpl.jointAngle.ShoulderRight.X < -THRESHOLD)
                {
                    res += "身体右转一点,";
                }
            }

            return res;
        }

        // 髋关节分析
        private string Hip(Feature tpl, Feature person)
        {
            string res = "";
            return res;
        }

        // 膝关节分析
        private string Knee(Feature tpl, Feature person)
        {
            string res = "";

            if (person.skeleton.Joints[JointType.KneeLeft].TrackingState == JointTrackingState.Tracked
                && person.skeleton.Joints[JointType.KneeRight].TrackingState == JointTrackingState.Tracked)
            {
                if (person.jointAngle.KneeLeft - tpl.jointAngle.KneeLeft > 15
                    || person.jointAngle.KneeRight - tpl.jointAngle.KneeRight > 15)
                {
                    res += "膝盖架子太高,";
                }
                if (person.jointAngle.KneeLeft - tpl.jointAngle.KneeLeft < -THRESHOLD
                    || person.jointAngle.KneeRight - tpl.jointAngle.KneeRight < -THRESHOLD)
                {
                    res += "膝盖架子太低,";
                }
            }
            return res;
        }
    }
}
