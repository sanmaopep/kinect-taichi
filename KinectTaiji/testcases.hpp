#pragma once

#include"angle.hpp"
#include"feature.hpp"
#include"IO.hpp"

using namespace std;
using namespace cv;

int testFeature() {
	Mat skeletonImage;
	skeletonImage.create(240, 320, CV_8UC3);
	IO io(".//seq.dat");
	io.clear();

	//1、初始化NUI，注意这里是USES_SKELETON  
	HRESULT hr = NuiInitialize(NUI_INITIALIZE_FLAG_USES_SKELETON);
	if (FAILED(hr))
	{
		cout << "NuiInitialize failed" << endl;
		return hr;
	}

	//2、定义骨骼信号事件句柄   
	HANDLE skeletonEvent = CreateEvent(NULL, TRUE, FALSE, NULL);

	//3、打开骨骼跟踪事件  
	hr = NuiSkeletonTrackingEnable(skeletonEvent, 0);
	if (FAILED(hr))
	{
		cout << "Could not open color image stream video" << endl;
		NuiShutdown();
		return hr;
	}
	namedWindow("skeletonImage", CV_WINDOW_AUTOSIZE);

	//4、开始读取骨骼跟踪数据   
	while (1)
	{
		NUI_SKELETON_FRAME skeletonFrame = { 0 };  //骨骼帧的定义   

												   //4.1、无限等待新的数据，等到后返回  
		if (WaitForSingleObject(skeletonEvent, INFINITE) == 0)
		{
			//4.2、从刚才打开数据流的流句柄中得到该帧数据，读取到的数据地址存于skeletonFrame  
			hr = NuiSkeletonGetNextFrame(0, &skeletonFrame);
			if (!SUCCEEDED(hr))
			{
				continue;
			}
			taiji::Feature feature(skeletonFrame);
			system("cls");
			feature.print();
			if (!feature.ok()) {
				continue;
			}
			io.appendFeature(feature);
			skeletonImage = feature.getSkeletonImage();
			imshow("skeletonImage", skeletonImage); //显示图像   
		}
		else
		{
			cout << "Buffer length of received texture is bogus\r\n" << endl;
		}

		if (cvWaitKey(20) == 27)
			break;
	}
	//5、关闭NUI链接   
	NuiShutdown();
	return 0;
}

int testIORead() {
	Mat skeletonImage;
	skeletonImage.create(240, 320, CV_8UC3);
	IO io(".//seq.dat");
	namedWindow("skeletonImage", CV_WINDOW_AUTOSIZE);
	
	cout << "正在读取文件\n";
	vector<taiji::Feature> vFeature = io.readFeatures();
	cout << "文件读取完毕\n";
	int size = vFeature.size();
	cout << "帧数：" << size<<endl;
	while (1) {
		for (int i = 0; i < size; i++) {
			taiji::Feature feature = vFeature[i];
			system("cls");
			feature.print();
			if (!feature.ok()) {
				continue;
			}
			skeletonImage = feature.getSkeletonImage();
			imshow("skeletonImage", skeletonImage); //显示图像 
			if (cvWaitKey(20) == 27)
				break;

			Sleep(1000/30);
		}
	}
	
	return 0;

}