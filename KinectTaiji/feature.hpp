#pragma once

#include<NuiApi.h>
#include<cstdio>

namespace taiji {

	// ������ȡ
	class Feature {
	public:
		// ���캯������ȡһ��feature
		Feature(NUI_SKELETON_FRAME skeletonFrame) {
			// �ж��Ƿ�������
			NUI_SKELETON_TRACKING_STATE trackingState = skeletonFrame.SkeletonData[i].eTrackingState;
			if (trackingState != NUI_SKELETON_TRACKED)
			{
				this->foundSkeleton = false;
				return;
			}
			this->foundSkeleton = true;
			// ƽ������֡����������
			NuiTransformSmooth(&skeletonFrame, NULL);
			// ��ȡ�������ݺ���Ԫ��
			for (int i = 0; i < NUI_SKELETON_COUNT; i++) {
				this->skeletonData[i] = skeletonFrame.SkeletonData[i];
				NuiSkeletonCalculateBoneOrientations(this->skeletonData + i, this->skeletonOrientation + i);
				//�϶��Ƿ���һ����ȷ���������������������ٵ����Ҽ粿���ģ�����λ�ã�������ٵ���   
				if (skeletonFrame.SkeletonData[i].eTrackingState == NUI_SKELETON_TRACKED &&
					skeletonFrame.SkeletonData[i].eSkeletonPositionTrackingState[NUI_SKELETON_POSITION_SHOULDER_CENTER] != NUI_SKELETON_POSITION_NOT_TRACKED)
				{
				}
			}

			
			
			
			
		}
		// ��ӡ���
		void print() {
			printf("Frame############################################################");
			printf("#################################################################");
		}

	private:
		// Joint Angle

		// skeletonOriginData
		NUI_SKELETON_DATA skeletonData[NUI_SKELETON_COUNT];
		// NUI_SKELETON_BONE_ORIENTATION -> hierarchicalRotation ������峯�����Ԫ��
		NUI_SKELETON_BONE_ORIENTATION skeletonOrientation[NUI_SKELETON_COUNT];
		bool foundSkeleton;
	};


}