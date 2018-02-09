#pragma once
#include"stadfx.h"
#include"angle.hpp"
#include"keyframes.hpp"

using namespace std;
using namespace cv;

namespace taiji {

	typedef vector<float> FEATURE_VEC;

	typedef struct _EULER_ANGLE{
		int startJoint;
		int endJoint;
		Vector4 angles;
	} EULER_ANGLE;

	// 特征提取
	class Feature {
	public:
		Feature() {}
		Feature(NUI_SKELETON_FRAME& skeletonFrame) {
			int SKELETON_INDEX = 0;
			this->frameNum = skeletonFrame.dwFrameNumber;
			this->foundSkeleton = false;
			// 判断是否有数据
			//NUI_SKELETON_COUNT是检测到的骨骼数（即，跟踪到的人数）  
			for (int i = 0; i < NUI_SKELETON_COUNT; i++)
			{
				NUI_SKELETON_TRACKING_STATE trackingState = skeletonFrame.SkeletonData[i].eTrackingState;
				if (trackingState == NUI_SKELETON_TRACKED)
				{
					SKELETON_INDEX = i;
					this->foundSkeleton = true;
				}
			}
			if (!this->foundSkeleton) {
				return;
			}
			// 平滑骨骼帧，消除抖动
			NuiTransformSmooth(&skeletonFrame, NULL);
			// 获取骨骼数据和四元数，只计算第一个人的数据
			this->skeletonData = skeletonFrame.SkeletonData[SKELETON_INDEX];
			//断定是否是一个正确骨骼的条件：骨骼被跟踪到并且肩部中心（颈部位置）必须跟踪到。   
			if (this->skeletonData.eTrackingState == NUI_SKELETON_TRACKED &&
				this->skeletonData.eSkeletonPositionTrackingState[NUI_SKELETON_POSITION_SHOULDER_CENTER] != NUI_SKELETON_POSITION_NOT_TRACKED)
			{
				// 计算朝向
				HRESULT hr = NuiSkeletonCalculateBoneOrientations(&this->skeletonData, this->skeletonOrientation);
				if (hr == S_FALSE) {
					return;
				}
				for (int i = 0; i < NUI_SKELETON_POSITION_COUNT; i++) {
					// 计算关节角度
					this->jointAngles[i] = getJointCosAngle(this->skeletonData, i);
					// 计算欧拉角
					EULER_ANGLE eulerAngle;
					eulerAngle.startJoint = this->skeletonOrientation[i].startJoint;
					eulerAngle.endJoint = this->skeletonOrientation[i].endJoint;
					eulerAngle.angles = getEulerAngle(this->skeletonOrientation[i].hierarchicalRotation);
					this->eulerAngles[i] = eulerAngle;
				}
			}	
		}
		void print() {
			printf("#################################################################\n");
			cout << "NO. "<<this->frameNum << endl;
			if (!this->foundSkeleton) {
				printf("没有发现任何人\n");
			}
			else {
				printf("关节角\n");
				for (int i = 0; i < NUI_SKELETON_POSITION_COUNT; i++) {
					if (this->jointAngles[i] > 0) {
						cout << SKELETON_POSITION_TRANSLATE[i] << ": " << (int)this->jointAngles[i] << endl;
					}
				}
				printf("欧拉角\n");
				for (int i = 0; i < NUI_SKELETON_POSITION_COUNT; i++) {
					EULER_ANGLE curr = this->eulerAngles[i];
					if (curr.startJoint<0 || curr.startJoint >20
						|| curr.endJoint<0 || curr.endJoint > 20) {
						continue;
					}
					cout << SKELETON_POSITION_TRANSLATE[curr.startJoint]
						<< "<->"
						<< SKELETON_POSITION_TRANSLATE[curr.endJoint]
						<< ": "
						<< (int)curr.angles.x << " "
						<< (int)curr.angles.y << " "
						<< (int)curr.angles.z << " "
						<< endl;
				}
			}
			printf("#################################################################\n");
		}
		// 获得欧氏空间上的向量，用于计算相似度
		FEATURE_VEC getVector() {
			FEATURE_VEC vRet;
			vRet.push_back(this->jointAngles[NUI_SKELETON_POSITION_KNEE_LEFT]);
			vRet.push_back(this->jointAngles[NUI_SKELETON_POSITION_KNEE_RIGHT]);
			vRet.push_back(this->jointAngles[NUI_SKELETON_POSITION_ELBOW_LEFT]);
			vRet.push_back(this->jointAngles[NUI_SKELETON_POSITION_ELBOW_RIGHT]);

			return vRet;
		}
		FEATURE_VEC getSimpleVector() {
			FEATURE_VEC vRet;
			vRet.push_back(this->jointAngles[NUI_SKELETON_POSITION_KNEE_LEFT]);
			vRet.push_back(this->jointAngles[NUI_SKELETON_POSITION_KNEE_RIGHT]);
			vRet.push_back(this->jointAngles[NUI_SKELETON_POSITION_ELBOW_LEFT]);
			vRet.push_back(this->jointAngles[NUI_SKELETON_POSITION_ELBOW_RIGHT]);
			
			return vRet;
		}
		// 计算欧氏距离
		static float E_Distance(const FEATURE_VEC& vec1,const FEATURE_VEC& vec2) {
			int size = vec1.size();
			if (size != vec2.size()) {
				return -1;
			}
			
			float sum = 0,diff;
			for (int i = 0; i < size; i++) {
				diff = vec1[i] - vec2[i];
				sum += diff * diff;
			}
			return sqrt(sum);
		}
		Mat getSkeletonImage() {
			Mat skeletonImage;
			skeletonImage.create(240, 320, CV_8UC3);  
			float fx, fy;
			CvPoint skeletonPoint[NUI_SKELETON_POSITION_COUNT] = { cvPoint(0,0) };
			//拿到所有跟踪到的关节点的坐标，并转换为我们的深度空间的坐标，因为我们是在深度图像中  
			//把这些关节点标记出来的  
			//NUI_SKELETON_POSITION_COUNT为跟踪到的一个骨骼的关节点的数目，为20  
			for (int i = 0; i < NUI_SKELETON_POSITION_COUNT; i++)
			{
				NuiTransformSkeletonToDepthImage(this->skeletonData.SkeletonPositions[i], &fx, &fy);
				skeletonPoint[i].x = (int)fx;
				skeletonPoint[i].y = (int)fy;
			}

			for (int i = 0; i<NUI_SKELETON_POSITION_COUNT; i++)
			{
				//跟踪点一用有三种状态：1没有被跟踪到，2跟踪到，3根据跟踪到的估计到     
				if (this->skeletonData.eSkeletonPositionTrackingState[i] != NUI_SKELETON_POSITION_NOT_TRACKED)
				{
					circle(skeletonImage, skeletonPoint[i], 3, cvScalar(0, 255, 255), 1, 8, 0);
				}
			}

			drawSkeleton(skeletonImage, skeletonPoint, 0);
			return skeletonImage;
		}
		bool ok() {
			return foundSkeleton;
		}

	private:
		DWORD frameNum;
		// Joint Angle
		float jointAngles[NUI_SKELETON_POSITION_COUNT] = {0};
		EULER_ANGLE eulerAngles[NUI_SKELETON_POSITION_COUNT];
		// skeletonOriginData
		NUI_SKELETON_DATA skeletonData;
		// NUI_SKELETON_BONE_ORIENTATION -> hierarchicalRotation 相对人体朝向的四元数
		NUI_SKELETON_BONE_ORIENTATION skeletonOrientation[NUI_SKELETON_POSITION_COUNT];
		bool foundSkeleton;

		//通过传入关节点的位置，把骨骼画出来  
		void drawSkeleton(Mat &image, CvPoint pointSet[], int whichone)
		{
			CvScalar color;
			switch (whichone) //跟踪不同的人显示不同的颜色   
			{
			case 0:
				color = cvScalar(255);
				break;
			case 1:
				color = cvScalar(0, 255);
				break;
			case 2:
				color = cvScalar(0, 0, 255);
				break;
			case 3:
				color = cvScalar(255, 255, 0);
				break;
			case 4:
				color = cvScalar(255, 0, 255);
				break;
			case 5:
				color = cvScalar(0, 255, 255);
				break;
			}

			if ((pointSet[NUI_SKELETON_POSITION_HEAD].x != 0 || pointSet[NUI_SKELETON_POSITION_HEAD].y != 0) &&
				(pointSet[NUI_SKELETON_POSITION_SHOULDER_CENTER].x != 0 || pointSet[NUI_SKELETON_POSITION_SHOULDER_CENTER].y != 0))
				line(image, pointSet[NUI_SKELETON_POSITION_HEAD], pointSet[NUI_SKELETON_POSITION_SHOULDER_CENTER], color, 2);
			if ((pointSet[NUI_SKELETON_POSITION_SHOULDER_CENTER].x != 0 || pointSet[NUI_SKELETON_POSITION_SHOULDER_CENTER].y != 0) &&
				(pointSet[NUI_SKELETON_POSITION_SPINE].x != 0 || pointSet[NUI_SKELETON_POSITION_SPINE].y != 0))
				line(image, pointSet[NUI_SKELETON_POSITION_SHOULDER_CENTER], pointSet[NUI_SKELETON_POSITION_SPINE], color, 2);
			if ((pointSet[NUI_SKELETON_POSITION_SPINE].x != 0 || pointSet[NUI_SKELETON_POSITION_SPINE].y != 0) &&
				(pointSet[NUI_SKELETON_POSITION_HIP_CENTER].x != 0 || pointSet[NUI_SKELETON_POSITION_HIP_CENTER].y != 0))
				line(image, pointSet[NUI_SKELETON_POSITION_SPINE], pointSet[NUI_SKELETON_POSITION_HIP_CENTER], color, 2);

			//左上肢   
			if ((pointSet[NUI_SKELETON_POSITION_SHOULDER_CENTER].x != 0 || pointSet[NUI_SKELETON_POSITION_SHOULDER_CENTER].y != 0) &&
				(pointSet[NUI_SKELETON_POSITION_SHOULDER_LEFT].x != 0 || pointSet[NUI_SKELETON_POSITION_SHOULDER_LEFT].y != 0))
				line(image, pointSet[NUI_SKELETON_POSITION_SHOULDER_CENTER], pointSet[NUI_SKELETON_POSITION_SHOULDER_LEFT], color, 2);
			if ((pointSet[NUI_SKELETON_POSITION_SHOULDER_LEFT].x != 0 || pointSet[NUI_SKELETON_POSITION_SHOULDER_LEFT].y != 0) &&
				(pointSet[NUI_SKELETON_POSITION_ELBOW_LEFT].x != 0 || pointSet[NUI_SKELETON_POSITION_ELBOW_LEFT].y != 0))
				line(image, pointSet[NUI_SKELETON_POSITION_SHOULDER_LEFT], pointSet[NUI_SKELETON_POSITION_ELBOW_LEFT], color, 2);
			if ((pointSet[NUI_SKELETON_POSITION_ELBOW_LEFT].x != 0 || pointSet[NUI_SKELETON_POSITION_ELBOW_LEFT].y != 0) &&
				(pointSet[NUI_SKELETON_POSITION_WRIST_LEFT].x != 0 || pointSet[NUI_SKELETON_POSITION_WRIST_LEFT].y != 0))
				line(image, pointSet[NUI_SKELETON_POSITION_ELBOW_LEFT], pointSet[NUI_SKELETON_POSITION_WRIST_LEFT], color, 2);
			if ((pointSet[NUI_SKELETON_POSITION_WRIST_LEFT].x != 0 || pointSet[NUI_SKELETON_POSITION_WRIST_LEFT].y != 0) &&
				(pointSet[NUI_SKELETON_POSITION_HAND_LEFT].x != 0 || pointSet[NUI_SKELETON_POSITION_HAND_LEFT].y != 0))
				line(image, pointSet[NUI_SKELETON_POSITION_WRIST_LEFT], pointSet[NUI_SKELETON_POSITION_HAND_LEFT], color, 2);

			//右上肢   
			if ((pointSet[NUI_SKELETON_POSITION_SHOULDER_CENTER].x != 0 || pointSet[NUI_SKELETON_POSITION_SHOULDER_CENTER].y != 0) &&
				(pointSet[NUI_SKELETON_POSITION_SHOULDER_RIGHT].x != 0 || pointSet[NUI_SKELETON_POSITION_SHOULDER_RIGHT].y != 0))
				line(image, pointSet[NUI_SKELETON_POSITION_SHOULDER_CENTER], pointSet[NUI_SKELETON_POSITION_SHOULDER_RIGHT], color, 2);
			if ((pointSet[NUI_SKELETON_POSITION_SHOULDER_RIGHT].x != 0 || pointSet[NUI_SKELETON_POSITION_SHOULDER_RIGHT].y != 0) &&
				(pointSet[NUI_SKELETON_POSITION_ELBOW_RIGHT].x != 0 || pointSet[NUI_SKELETON_POSITION_ELBOW_RIGHT].y != 0))
				line(image, pointSet[NUI_SKELETON_POSITION_SHOULDER_RIGHT], pointSet[NUI_SKELETON_POSITION_ELBOW_RIGHT], color, 2);
			if ((pointSet[NUI_SKELETON_POSITION_ELBOW_RIGHT].x != 0 || pointSet[NUI_SKELETON_POSITION_ELBOW_RIGHT].y != 0) &&
				(pointSet[NUI_SKELETON_POSITION_WRIST_RIGHT].x != 0 || pointSet[NUI_SKELETON_POSITION_WRIST_RIGHT].y != 0))
				line(image, pointSet[NUI_SKELETON_POSITION_ELBOW_RIGHT], pointSet[NUI_SKELETON_POSITION_WRIST_RIGHT], color, 2);
			if ((pointSet[NUI_SKELETON_POSITION_WRIST_RIGHT].x != 0 || pointSet[NUI_SKELETON_POSITION_WRIST_RIGHT].y != 0) &&
				(pointSet[NUI_SKELETON_POSITION_HAND_RIGHT].x != 0 || pointSet[NUI_SKELETON_POSITION_HAND_RIGHT].y != 0))
				line(image, pointSet[NUI_SKELETON_POSITION_WRIST_RIGHT], pointSet[NUI_SKELETON_POSITION_HAND_RIGHT], color, 2);

			//左下肢   
			if ((pointSet[NUI_SKELETON_POSITION_HIP_CENTER].x != 0 || pointSet[NUI_SKELETON_POSITION_HIP_CENTER].y != 0) &&
				(pointSet[NUI_SKELETON_POSITION_HIP_LEFT].x != 0 || pointSet[NUI_SKELETON_POSITION_HIP_LEFT].y != 0))
				line(image, pointSet[NUI_SKELETON_POSITION_HIP_CENTER], pointSet[NUI_SKELETON_POSITION_HIP_LEFT], color, 2);
			if ((pointSet[NUI_SKELETON_POSITION_HIP_LEFT].x != 0 || pointSet[NUI_SKELETON_POSITION_HIP_LEFT].y != 0) &&
				(pointSet[NUI_SKELETON_POSITION_KNEE_LEFT].x != 0 || pointSet[NUI_SKELETON_POSITION_KNEE_LEFT].y != 0))
				line(image, pointSet[NUI_SKELETON_POSITION_HIP_LEFT], pointSet[NUI_SKELETON_POSITION_KNEE_LEFT], color, 2);
			if ((pointSet[NUI_SKELETON_POSITION_KNEE_LEFT].x != 0 || pointSet[NUI_SKELETON_POSITION_KNEE_LEFT].y != 0) &&
				(pointSet[NUI_SKELETON_POSITION_ANKLE_LEFT].x != 0 || pointSet[NUI_SKELETON_POSITION_ANKLE_LEFT].y != 0))
				line(image, pointSet[NUI_SKELETON_POSITION_KNEE_LEFT], pointSet[NUI_SKELETON_POSITION_ANKLE_LEFT], color, 2);
			if ((pointSet[NUI_SKELETON_POSITION_ANKLE_LEFT].x != 0 || pointSet[NUI_SKELETON_POSITION_ANKLE_LEFT].y != 0) &&
				(pointSet[NUI_SKELETON_POSITION_FOOT_LEFT].x != 0 || pointSet[NUI_SKELETON_POSITION_FOOT_LEFT].y != 0))
				line(image, pointSet[NUI_SKELETON_POSITION_ANKLE_LEFT], pointSet[NUI_SKELETON_POSITION_FOOT_LEFT], color, 2);

			//右下肢   
			if ((pointSet[NUI_SKELETON_POSITION_HIP_CENTER].x != 0 || pointSet[NUI_SKELETON_POSITION_HIP_CENTER].y != 0) &&
				(pointSet[NUI_SKELETON_POSITION_HIP_RIGHT].x != 0 || pointSet[NUI_SKELETON_POSITION_HIP_RIGHT].y != 0))
				line(image, pointSet[NUI_SKELETON_POSITION_HIP_CENTER], pointSet[NUI_SKELETON_POSITION_HIP_RIGHT], color, 2);
			if ((pointSet[NUI_SKELETON_POSITION_HIP_RIGHT].x != 0 || pointSet[NUI_SKELETON_POSITION_HIP_RIGHT].y != 0) &&
				(pointSet[NUI_SKELETON_POSITION_KNEE_RIGHT].x != 0 || pointSet[NUI_SKELETON_POSITION_KNEE_RIGHT].y != 0))
				line(image, pointSet[NUI_SKELETON_POSITION_HIP_RIGHT], pointSet[NUI_SKELETON_POSITION_KNEE_RIGHT], color, 2);
			if ((pointSet[NUI_SKELETON_POSITION_KNEE_RIGHT].x != 0 || pointSet[NUI_SKELETON_POSITION_KNEE_RIGHT].y != 0) &&
				(pointSet[NUI_SKELETON_POSITION_ANKLE_RIGHT].x != 0 || pointSet[NUI_SKELETON_POSITION_ANKLE_RIGHT].y != 0))
				line(image, pointSet[NUI_SKELETON_POSITION_KNEE_RIGHT], pointSet[NUI_SKELETON_POSITION_ANKLE_RIGHT], color, 2);
			if ((pointSet[NUI_SKELETON_POSITION_ANKLE_RIGHT].x != 0 || pointSet[NUI_SKELETON_POSITION_ANKLE_RIGHT].y != 0) &&
				(pointSet[NUI_SKELETON_POSITION_FOOT_RIGHT].x != 0 || pointSet[NUI_SKELETON_POSITION_FOOT_RIGHT].y != 0))
				line(image, pointSet[NUI_SKELETON_POSITION_ANKLE_RIGHT], pointSet[NUI_SKELETON_POSITION_FOOT_RIGHT], color, 2);
		}
	};


}