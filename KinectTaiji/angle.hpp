#pragma once

#include"stadfx.h"

namespace taiji {
	// ���һ���ؽڵ�ǰ��ؽ�
	void getJointRelation(const int& joint,int* jointBefore,int* jointAfter) {
		switch (joint)
		{
		// �Źؽ�
		case NUI_SKELETON_POSITION_HIP_CENTER:
			*jointBefore = NUI_SKELETON_POSITION_HIP_LEFT;
			*jointAfter = NUI_SKELETON_POSITION_HIP_RIGHT;
		// ϥ�ؽ�
		case NUI_SKELETON_POSITION_KNEE_LEFT:
			*jointBefore = NUI_SKELETON_POSITION_HIP_LEFT;
			*jointAfter = NUI_SKELETON_POSITION_ANKLE_LEFT;
			break;
		case NUI_SKELETON_POSITION_KNEE_RIGHT:
			*jointBefore = NUI_SKELETON_POSITION_HIP_RIGHT;
			*jointAfter = NUI_SKELETON_POSITION_ANKLE_RIGHT;
			break;
		// ��ؽ�
		case NUI_SKELETON_POSITION_ELBOW_LEFT:
			*jointBefore = NUI_SKELETON_POSITION_SHOULDER_LEFT;
			*jointAfter = NUI_SKELETON_POSITION_WRIST_LEFT;
			break;
		case NUI_SKELETON_POSITION_ELBOW_RIGHT:
			*jointBefore = NUI_SKELETON_POSITION_SHOULDER_RIGHT;
			*jointAfter = NUI_SKELETON_POSITION_WRIST_RIGHT;
			break;
		// ����
		case NUI_SKELETON_POSITION_WRIST_LEFT:
			*jointBefore = NUI_SKELETON_POSITION_ELBOW_LEFT;
			*jointAfter = NUI_SKELETON_POSITION_HAND_LEFT;
			break;
		case NUI_SKELETON_POSITION_WRIST_RIGHT:
			*jointBefore = NUI_SKELETON_POSITION_ELBOW_RIGHT;
			*jointAfter = NUI_SKELETON_POSITION_HAND_RIGHT;
			break;
		// ����
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

	// ��ȡjoint�ؽ����ڵ����ҽ�
	float getJointCosAngle(const NUI_SKELETON_DATA& data,int joint) {
		int jointBefore, jointAfter;
		Vector4 vectorBefore,vectorAfter;
		getJointRelation(joint, &jointBefore, &jointAfter);
		// ���û��Map��ϵ
		if (jointBefore == -1) {
			return -1;
		}
		vectorBefore.x = data.SkeletonPositions[joint].x - data.SkeletonPositions[jointBefore].x;
		vectorBefore.y = data.SkeletonPositions[joint].y - data.SkeletonPositions[jointBefore].y;
		vectorBefore.z = data.SkeletonPositions[joint].z - data.SkeletonPositions[jointBefore].z;
		vectorAfter.x = data.SkeletonPositions[joint].x - data.SkeletonPositions[jointAfter].x;
		vectorAfter.y = data.SkeletonPositions[joint].y - data.SkeletonPositions[jointAfter].y;
		vectorAfter.z = data.SkeletonPositions[joint].z - data.SkeletonPositions[jointAfter].z;
		// ���������Ƕ�
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

	// ����Orientation���ŷ����
	Vector4 getEulerAngle(const NUI_SKELETON_BONE_ROTATION& rotation) {
		Vector4 ret;
		Matrix4 m = rotation.rotationMatrix;

		float sy = sqrt(m.M32*m.M32 + m.M33*m.M33);


		if (!(sy < 1e-6)) {
			ret.x = atan2(m.M32, m.M33);
			ret.y = atan2(-m.M31, sy);
			ret.z = atan2(m.M21, m.M11);
		}
		else
		{
			ret.x = atan2(-m.M23, m.M22);
			ret.y = atan2(-m.M31, sy);
			ret.z = 0;
		}
		ret.x = ret.x * 180 / 3.14159;
		ret.y = ret.y * 180 / 3.14159;
		ret.z = ret.z * 180 / 3.14159;
		return ret;
	}

}
