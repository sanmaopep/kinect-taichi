#pragma once

#include<cmath>
#include<NuiApi.h>

namespace taiji {

	// 获得一个关节的前后角度
	void getJointRelation(const int& joint,int* jointBefore,int* jointAfter) {
		switch (joint)
		{
		// 髋关节
		case NUI_SKELETON_POSITION_HIP_CENTER:
			*jointBefore = NUI_SKELETON_POSITION_HIP_LEFT;
			*jointAfter = NUI_SKELETON_POSITION_HIP_RIGHT;
		// 膝关节
		case NUI_SKELETON_POSITION_KNEE_LEFT:
			*jointBefore = NUI_SKELETON_POSITION_HIP_LEFT;
			*jointAfter = NUI_SKELETON_POSITION_ANKLE_LEFT;
			break;
		case NUI_SKELETON_POSITION_KNEE_RIGHT:
			*jointBefore = NUI_SKELETON_POSITION_HIP_RIGHT;
			*jointAfter = NUI_SKELETON_POSITION_ANKLE_RIGHT;
			break;
		// 肘关节
		case NUI_SKELETON_POSITION_ELBOW_LEFT:
			*jointBefore = NUI_SKELETON_POSITION_SHOULDER_LEFT;
			*jointAfter = NUI_SKELETON_POSITION_WRIST_LEFT;
			break;
		case NUI_SKELETON_POSITION_ELBOW_RIGHT:
			*jointBefore = NUI_SKELETON_POSITION_SHOULDER_RIGHT;
			*jointAfter = NUI_SKELETON_POSITION_WRIST_RIGHT;
			break;
		// 手腕
		case NUI_SKELETON_POSITION_WRIST_LEFT:
			*jointBefore = NUI_SKELETON_POSITION_ELBOW_LEFT;
			*jointAfter = NUI_SKELETON_POSITION_HAND_LEFT;
			break;
		case NUI_SKELETON_POSITION_WRIST_RIGHT:
			*jointBefore = NUI_SKELETON_POSITION_ELBOW_RIGHT;
			*jointAfter = NUI_SKELETON_POSITION_HAND_RIGHT;
			break;
		// 脚腕
		case NUI_SKELETON_POSITION_ANKLE_LEFT:
			*jointBefore = NUI_SKELETON_POSITION_KNEE_LEFT;
			*jointAfter = NUI_SKELETON_POSITION_FOOT_LEFT;
			break;
		case NUI_SKELETON_POSITION_ANKLE_RIGHT:
			*jointBefore = NUI_SKELETON_POSITION_KNEE_RIGHT;
			*jointAfter = NUI_SKELETON_POSITION_FOOT_RIGHT;
			break;
		default:
			*jointBefore = *jointAfter =-1;
			break;
		}
	}

	// 获取joint关节所在的余弦角
	float getJointCosAngle(const NUI_SKELETON_DATA& data,int joint) {
		int jointBefore, jointAfter;
		Vector4 vectorBefore,vectorAfter;
		getJointRelation(joint, &jointBefore, &jointAfter);
		// 如果没有Map关系
		if (jointBefore == -1) {
			return -1;
		}
		vectorBefore.x = data.SkeletonPositions[joint].x - data.SkeletonPositions[jointBefore].x;
		vectorBefore.y = data.SkeletonPositions[joint].y - data.SkeletonPositions[jointBefore].y;
		vectorBefore.z = data.SkeletonPositions[joint].z - data.SkeletonPositions[jointBefore].z;
		vectorAfter.x = data.SkeletonPositions[joint].x - data.SkeletonPositions[jointAfter].x;
		vectorAfter.y = data.SkeletonPositions[joint].y - data.SkeletonPositions[jointAfter].y;
		vectorAfter.z = data.SkeletonPositions[joint].z - data.SkeletonPositions[jointAfter].z;
		// 计算向量角度
		float len1 = vectorBefore.x * vectorBefore.x 
			+ vectorBefore.y * vectorBefore.y 
			+ vectorBefore.z * vectorBefore.z;
		float len2 = vectorAfter.x * vectorAfter.x 
			+ vectorAfter.y * vectorAfter.y 
			+ vectorAfter.z * vectorAfter.z;
		float mutiple = vectorBefore.x * vectorAfter.x
				+ vectorBefore.y * vectorAfter.y
				+ vectorBefore.z * vectorAfter.z;
		float cos = mutiple / sqrt(len1*len2);
		return acos(cos)* 180.0 / 3.14159;
	}

}
