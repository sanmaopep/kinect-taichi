#pragma once

#include<NuiApi.h>
#include<cstdio>

namespace taiji {

	// 特征提取
	class Feature {
	public:
		// 构造函数，获取一个feature
		Feature(NUI_SKELETON_FRAME skeletonFrame) {
			// 判断是否有数据
			NUI_SKELETON_TRACKING_STATE trackingState = skeletonFrame.SkeletonData[i].eTrackingState;
			if (trackingState != NUI_SKELETON_TRACKED)
			{
				this->foundSkeleton = false;
				return;
			}
			this->foundSkeleton = true;
			// 平滑骨骼帧，消除抖动
			NuiTransformSmooth(&skeletonFrame, NULL);
			// 获取骨骼数据和四元数
			for (int i = 0; i < NUI_SKELETON_COUNT; i++) {
				this->skeletonData[i] = skeletonFrame.SkeletonData[i];
				NuiSkeletonCalculateBoneOrientations(this->skeletonData + i, this->skeletonOrientation + i);
				//断定是否是一个正确骨骼的条件：骨骼被跟踪到并且肩部中心（颈部位置）必须跟踪到。   
				if (skeletonFrame.SkeletonData[i].eTrackingState == NUI_SKELETON_TRACKED &&
					skeletonFrame.SkeletonData[i].eSkeletonPositionTrackingState[NUI_SKELETON_POSITION_SHOULDER_CENTER] != NUI_SKELETON_POSITION_NOT_TRACKED)
				{
				}
			}

			
			
			
			
		}
		// 打印结果
		void print() {
			printf("Frame############################################################");
			printf("#################################################################");
		}

	private:
		// Joint Angle

		// skeletonOriginData
		NUI_SKELETON_DATA skeletonData[NUI_SKELETON_COUNT];
		// NUI_SKELETON_BONE_ORIENTATION -> hierarchicalRotation 相对人体朝向的四元数
		NUI_SKELETON_BONE_ORIENTATION skeletonOrientation[NUI_SKELETON_COUNT];
		bool foundSkeleton;
	};


}