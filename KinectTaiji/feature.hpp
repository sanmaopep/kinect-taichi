#pragma once

#include<NuiApi.h>


namespace taiji {

	// ������ȡ
	class Feature {
	public:
		static Feature extractFeature() {

		}
		// constructor
		Feature(NUI_SKELETON_FRAME skeletonFrame) {

			
		}

	private:
		// Joint Angle

		// skeletonOriginData
		NUI_SKELETON_FRAME skeletonFrame;
		bool foundSkeleton;
	};


}